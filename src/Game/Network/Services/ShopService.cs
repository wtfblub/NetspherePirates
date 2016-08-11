﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.IO;
using BlubLib.Network;
using BlubLib.Network.Pipes;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using NLog;
using NLog.Fluent;

namespace Netsphere.Network.Services
{
    internal class ShopService : MessageHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [MessageHandler(typeof(CNewShopUpdateCheckReqMessage))]
        public void ShopUpdateCheckHandler(IService service, GameSession session, CNewShopUpdateCheckReqMessage message)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();
            var version = shop.Version;
            session.Send(new SNewShopUpdateCheckAckMessage
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

            using (var w = new BinaryWriter(new PooledMemoryStream(service.ArrayPool)))
            {
                w.Serialize(shop.Prices.Values.ToArray());

                session.Send(new SNewShopUpdateInfoAckMessage
                {
                    Type = ShopResourceType.NewShopPrice,
                    Data = w.ToArray(),
                    Date = version
                });
            }

            #endregion

            #region NewShopEffect

            using (var w = new BinaryWriter(new PooledMemoryStream(service.ArrayPool)))
            {
                w.Serialize(shop.Effects.Values.ToArray());

                session.Send(new SNewShopUpdateInfoAckMessage
                {
                    Type = ShopResourceType.NewShopEffect,
                    Data = w.ToArray(),
                    Date = version
                });
            }

            #endregion

            #region NewShopItem

            using (var w = new BinaryWriter(new PooledMemoryStream(service.ArrayPool)))
            {
                w.Serialize(shop.Items.Values.ToArray());

                session.Send(new SNewShopUpdateInfoAckMessage
                {
                    Type = ShopResourceType.NewShopItem,
                    Data = w.ToArray(),
                    Date = version
                });
            }

            #endregion

            // ToDo
            using (var w = new BinaryWriter(new PooledMemoryStream(service.ArrayPool)))
            {
                w.Write(0);

                session.Send(new SNewShopUpdateInfoAckMessage
                {
                    Type = ShopResourceType.NewShopUniqueItem,
                    Data = w.ToArray(),
                    Date = version
                });
            }

            using (var w = new BinaryWriter(new PooledMemoryStream(service.ArrayPool)))
            {
                w.Write(new byte[200]);

                session.Send(new SNewShopUpdateInfoAckMessage
                {
                    Type = (ShopResourceType)16,
                    Data = w.ToArray(),
                    Date = version
                });
            }
        }

        [MessageHandler(typeof(CLicensedReqMessage))]
        public void LicensedHandler(GameSession session, CLicensedReqMessage message)
        {
            try
            {
                session.Player.LicenseManager.Acquire(message.License);
            }
            catch (LicenseNotFoundException ex)
            {
                _logger.Error()
                    .Account(session)
                    .Exception(ex)
                    .Write();
            }
        }

        [MessageHandler(typeof(CExerciseLicenceReqMessage))]
        public void ExerciseLicenseHandler(GameSession session, CExerciseLicenceReqMessage message)
        {
            try
            {
                session.Player.LicenseManager.Acquire(message.License);
            }
            catch (LicenseException ex)
            {
                _logger.Error()
                    .Account(session)
                    .Exception(ex)
                    .Write();
            }
        }

        [MessageHandler(typeof(CBuyItemReqMessage))]
        public void BuyItemHandler(GameSession session, CBuyItemReqMessage message)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();
            var plr = session.Player;

            foreach (var item in message.Items)
            {
                var shopItemInfo = shop.GetItemInfo(item.ItemNumber, item.PriceType);
                if (shopItemInfo == null)
                {
                    _logger.Error()
                        .Account(session)
                        .Message("No shop entry found for {0} {1} {3}{2}", item.ItemNumber, item.PriceType, item.PeriodType, item.Period)
                        .Write();

                    session.Send(new SServerResultInfoAckMessage(ServerResult.DBError));
                    return;
                }
                if (!shopItemInfo.IsEnabled)
                {
                    _logger.Error()
                        .Account(session)
                        .Message("No shop entry {0} {1} {3}{2} is not enabled", item.ItemNumber, item.PriceType, item.PeriodType, item.Period)
                        .Write();

                    session.Send(new SServerResultInfoAckMessage(ServerResult.DBError));

                    return;
                }

                var priceGroup = shopItemInfo.PriceGroup;
                var price = priceGroup.GetPrice(item.PeriodType, item.Period);
                if (price == null)
                {
                    _logger.Error()
                        .Account(session)
                        .Message("Invalid price group for shop entry {0} {1} {3}{2}", item.ItemNumber, item.PriceType, item.PeriodType, item.Period)
                        .Write();

                    session.Send(new SServerResultInfoAckMessage(ServerResult.DBError));

                    return;
                }
                if (!price.IsEnabled)
                {
                    _logger.Error()
                        .Account(session)
                        .Message("Shop entry {0} {1} {3}{2} is not enabled", item.ItemNumber, item.PriceType, item.PeriodType, item.Period)
                        .Write();

                    session.Send(new SServerResultInfoAckMessage(ServerResult.DBError));

                    return;
                }

                if (item.Color > shopItemInfo.ShopItem.ColorGroup)
                {
                    _logger.Error()
                        .Account(session)
                        .Message("Shop entry {0} {1} {3}{2} has no color {4}", item.ItemNumber, item.PriceType, item.PeriodType, item.Period, item.Color)
                        .Write();

                    session.Send(new SServerResultInfoAckMessage(ServerResult.DBError));

                    return;
                }

                if (item.Effect != 0)
                {
                    if (shopItemInfo.EffectGroup.Effects.All(effect => effect.Effect != item.Effect))
                    {
                        _logger.Error()
                            .Account(session)
                            .Message("Shop entry {0} {1} {3}{2} has no effect {4}", item.ItemNumber, item.PriceType, item.PeriodType, item.Period, item.Effect)
                            .Write();

                        session.Send(new SServerResultInfoAckMessage(ServerResult.DBError));

                        return;
                    }
                }

                if (shopItemInfo.ShopItem.License != ItemLicense.None &&
                    !plr.LicenseManager.Contains(shopItemInfo.ShopItem.License) &&
                    Config.Instance.Game.EnableLicenseRequirement)
                {
                    _logger.Error()
                        .Account(session)
                        .Message("Doesn't have license {0}", shopItemInfo.ShopItem.License)
                        .Write();

                    session.Send(new SServerResultInfoAckMessage(ServerResult.DBError));

                    return;
                }

                // ToDo missing price types

                switch (shopItemInfo.PriceGroup.PriceType)
                {
                    case ItemPriceType.PEN:
                        if (plr.PEN < price.Price)
                        {
                            session.Send(new SBuyItemAckMessage(ItemBuyResult.NotEnoughMoney));

                            return;
                        }
                        plr.PEN -= (uint)price.Price;
                        break;

                    case ItemPriceType.AP:
                        if (plr.AP < price.Price)
                        {
                            session.Send(new SBuyItemAckMessage(ItemBuyResult.NotEnoughMoney));

                            return;
                        }
                        plr.AP -= (uint)price.Price;
                        break;

                    case ItemPriceType.Premium:
                        if (plr.AP < price.Price)
                        {
                            session.Send(new SBuyItemAckMessage(ItemBuyResult.NotEnoughMoney));

                            return;
                        }
                        plr.AP -= (uint)price.Price;
                        break;

                    default:
                        session.Send(new SBuyItemAckMessage(ItemBuyResult.DBError));
                        _logger.Error()
                            .Account(session)
                            .Message("Unknown PriceType {0}", shopItemInfo.PriceGroup.PriceType)
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


                session.Send(new SBuyItemAckMessage(new[] { plrItem.Id }, item));

                session.Send(new SRefreshCashInfoAckMessage(plr.PEN, plr.AP));
            }
        }

        [MessageHandler(typeof(CRandomShopRollingStartReqMessage))]
        public void RandomShopRollHandler(GameSession session, CRandomShopRollingStartReqMessage message)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();

            session.Send(new SRandomShopItemInfoAckMessage
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

            session.Send(new SRandomShopItemInfoAckMessage
            {
                Item = new RandomShopItemDto()
            });
        }
    }
}
