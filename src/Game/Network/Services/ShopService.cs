using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using NLog;
using NLog.Fluent;
using ProudNet.Handlers;

namespace Netsphere.Network.Services
{
    internal class ShopService : ProudMessageHandler
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [MessageHandler(typeof(NewShopUpdateCheckReqMessage))]
        public void ShopUpdateCheckHandler(GameSession session, NewShopUpdateCheckReqMessage message)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();
            var version = shop.Version;
            session.SendAsync(new NewShopUpdateCheckAckMessage
            {
                Date01 = version,
                Date02 = version,
                Date03 = version,
                Date04 = version,
                Unk = 0
            });
            //session.Send(new SRandomShopInfoAckMessage
            //{
            //    Info = new RandomShopDto
            //    {
            //        ItemNumbers = new List<ItemNumber> { 2000001, 2000002, 2000003 },
            //        Effects = new List<uint> { 0, 0, 0 },
            //        Colors = new List<uint> { 2, 0, 0 },
            //        PeriodTypes = new List<ItemPeriodType> { ItemPeriodType.Hours, ItemPeriodType.Hours, ItemPeriodType.Hours },
            //        Periods = new List<ushort> { 2, 4, 10 },
            //        Unk6 = 10000,
            //    }
            //});

            if (message.Date01 == version &&
                message.Date02 == version &&
                message.Date03 == version &&
                message.Date04 == version)
            {
                return;
            }


            #region NewShopPrice

            using (var w = new BinaryWriter(new MemoryStream()))
            {
                w.Serialize(shop.Prices.Values.ToArray());

                session.SendAsync(new NewShopUpdataInfoAckMessage
                {
                    Type = ShopResourceType.NewShopPrice,
                    Data = w.ToArray(),
                    Date = version
                });
            }

            #endregion

            #region NewShopEffect

            using (var w = new BinaryWriter(new MemoryStream()))
            {
                w.Serialize(shop.Effects.Values.ToArray());

                session.SendAsync(new NewShopUpdataInfoAckMessage
                {
                    Type = ShopResourceType.NewShopEffect,
                    Data = w.ToArray(),
                    Date = version
                });
            }

            #endregion

            #region NewShopItem

            using (var w = new BinaryWriter(new MemoryStream()))
            {
                w.Serialize(shop.Items.Values.ToArray());

                session.SendAsync(new NewShopUpdataInfoAckMessage
                {
                    Type = ShopResourceType.NewShopItem,
                    Data = w.ToArray(),
                    Date = version
                });
            }

            #endregion

            // ToDo
            using (var w = new BinaryWriter(new MemoryStream()))
            {
                w.Write(0);

                session.SendAsync(new NewShopUpdataInfoAckMessage
                {
                    Type = ShopResourceType.NewShopUniqueItem,
                    Data = w.ToArray(),
                    Date = version
                });
            }

            using (var w = new BinaryWriter(new MemoryStream()))
            {
                w.Write(new byte[200]);

                session.SendAsync(new NewShopUpdataInfoAckMessage
                {
                    Type = (ShopResourceType)16,
                    Data = w.ToArray(),
                    Date = version
                });
            }
        }

        [MessageHandler(typeof(LicenseGainReqMessage))]
        public void LicensedHandler(GameSession session, LicenseGainReqMessage message)
        {
            try
            {
                session.Player.LicenseManager.Acquire(message.License);
            }
            catch (LicenseNotFoundException ex)
            {
                Logger.Error()
                    .Account(session)
                    .Exception(ex)
                    .Write();
            }
        }

        [MessageHandler(typeof(LicenseExerciseReqMessage))]
        public void ExerciseLicenseHandler(GameSession session, LicenseExerciseReqMessage message)
        {
            try
            {
                session.Player.LicenseManager.Acquire(message.License);
            }
            catch (LicenseException ex)
            {
                Logger.Error()
                    .Account(session)
                    .Exception(ex)
                    .Write();
            }
        }

        [MessageHandler(typeof(ItemBuyItemReqMessage))]
        public async Task BuyItemHandler(GameSession session, ItemBuyItemReqMessage message)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();
            var plr = session.Player;

            foreach (var item in message.Items)
            {
                var shopItemInfo = shop.GetItemInfo(item.ItemNumber, item.PriceType);
                if (shopItemInfo == null)
                {
                    Logger.Error()
                        .Account(session)
                        .Message($"No shop entry found for {item.ItemNumber} {item.PriceType} {item.Period}{item.PeriodType}")
                        .Write();

                    session.SendAsync(new ItemBuyItemAckMessage(ItemBuyResult.UnkownItem));
                    return;
                }
                if (!shopItemInfo.IsEnabled)
                {
                    Logger.Error()
                        .Account(session)
                        .Message($"No shop entry {item.ItemNumber} {item.PriceType} {item.Period}{item.PeriodType} is not enabled")
                        .Write();

                    session.SendAsync(new ItemBuyItemAckMessage(ItemBuyResult.UnkownItem));
                    return;
                }

                var priceGroup = shopItemInfo.PriceGroup;
                var price = priceGroup.GetPrice(item.PeriodType, item.Period);
                if (price == null)
                {
                    Logger.Error()
                        .Account(session)
                        .Message($"Invalid price group for shop entry {item.ItemNumber} {item.PriceType} {item.Period}{item.PeriodType}")
                        .Write();

                    session.SendAsync(new ItemBuyItemAckMessage(ItemBuyResult.UnkownItem));
                    return;
                }

                if (!price.IsEnabled)
                {
                    Logger.Error()
                        .Account(session)
                        .Message($"Shop entry {item.ItemNumber} {item.PriceType} {item.Period}{item.PeriodType} is not enabled")
                        .Write();

                    session.SendAsync(new ItemBuyItemAckMessage(ItemBuyResult.UnkownItem));
                    return;
                }

                if (item.Color > shopItemInfo.ShopItem.ColorGroup)
                {
                    Logger.Error()
                        .Account(session)
                        .Message($"Shop entry {item.ItemNumber} {item.PriceType} {item.Period}{item.PeriodType} has no color {item.Color}")
                        .Write();

                    session.SendAsync(new ItemBuyItemAckMessage(ItemBuyResult.UnkownItem));
                    return;
                }

                if (item.Effect != 0)
                {
                    if (shopItemInfo.EffectGroup.Effects.All(effect => effect.Effect != item.Effect))
                    {
                        Logger.Error()
                            .Account(session)
                            .Message($"Shop entry {item.ItemNumber} {item.PriceType} {item.Period}{item.PeriodType} has no effect {item.Effect}")
                            .Write();

                        session.SendAsync(new ItemBuyItemAckMessage(ItemBuyResult.UnkownItem));
                        return;
                    }
                }

                if (shopItemInfo.ShopItem.License != ItemLicense.None &&
                    !plr.LicenseManager.Contains(shopItemInfo.ShopItem.License) &&
                    Config.Instance.Game.EnableLicenseRequirement)
                {
                    Logger.Error()
                        .Account(session)
                        .Message($"Doesn't have license {shopItemInfo.ShopItem.License}")
                        .Write();

                    session.SendAsync(new ItemBuyItemAckMessage(ItemBuyResult.UnkownItem));
                    return;
                }

                // ToDo missing price types
                switch (shopItemInfo.PriceGroup.PriceType)
                {
                    case ItemPriceType.PEN:
                        if (plr.PEN < price.Price)
                        {
                            session.SendAsync(new ItemBuyItemAckMessage(ItemBuyResult.NotEnoughMoney));
                            return;
                        }
                        plr.PEN -= (uint)price.Price;
                        break;

                    case ItemPriceType.AP:
                    case ItemPriceType.Premium:
                        if (plr.AP < price.Price)
                        {
                            session.SendAsync(new ItemBuyItemAckMessage(ItemBuyResult.NotEnoughMoney));
                            return;
                        }
                        plr.AP -= (uint)price.Price;
                        break;

                    default:
                        Logger.Error()
                            .Account(session)
                            .Message($"Unknown PriceType {shopItemInfo.PriceGroup.PriceType}")
                            .Write();
                        return;
                }

                // ToDo
                //var purchaseDto = new PlayerPurchaseDto
                //{
                //    account_id = (int)plr.Account.Id,
                //    shop_item_id = item.ItemNumber,
                //    shop_item_info_id = shopItemInfo.Id,
                //    shop_price_id = price.Id,
                //    time = DateTimeOffset.Now.ToUnixTimeSeconds()
                //};
                //db.player_purchase.Add(purchaseDto);

                var plrItem = session.Player.Inventory.Create(shopItemInfo, price, item.Color, item.Effect, (uint)(price.PeriodType == ItemPeriodType.Units ? price.Period : 0));

                await session.SendAsync(new ItemBuyItemAckMessage(new[] { plrItem.Id }, item));
                await session.SendAsync(new MoneyRefreshCashInfoAckMessage(plr.PEN, plr.AP));
            }
        }

        [MessageHandler(typeof(RandomShopRollingStartReqMessage))]
        public void RandomShopRollHandler(GameSession session, RandomShopRollingStartReqMessage message)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();

            session.SendAsync(new SRandomShopItemInfoAckMessage
            {
                Item = new RandomShopItemDto()
            });
            //session.Send(new SRandomShopItemInfoAckMessage
            //{
            //    Item = new RandomShopItemDto
            //    {
            //        Unk1 = 2000001,
            //        Value = 2000001,
            //        CurrentWeapon = 2000001,
            //        Unk4 = 2000001,
            //        Unk5 = 2000001,
            //        Unk6 = 0,
            //    }
            //});
        }

        [MessageHandler(typeof(CRandomShopItemSaleReqMessage))]
        public void RandomShopItemSaleHandler(GameSession session, CRandomShopItemSaleReqMessage message)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();

            session.SendAsync(new SRandomShopItemInfoAckMessage
            {
                Item = new RandomShopItemDto()
            });
        }
    }
}
