using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Services;
using ProudNet;

namespace Netsphere.Server.Game.Handlers
{
    internal class ShopHandler : IHandle<ItemBuyItemReqMessage>
    {
        private readonly GameDataService _gameDataService;
        private readonly ILogger _logger;

        public ShopHandler(GameDataService gameDataService, ILogger<ShopHandler> logger)
        {
            _gameDataService = gameDataService;
            _logger = logger;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ItemBuyItemReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var plrLogger = plr.AddContextToLogger(_logger);
            var logger = plrLogger;

            var itemsTobuy = message.Items.GroupBy(x => x);
            var newItems = new List<PlayerItem>();

            foreach (var group in itemsTobuy)
            {
                logger = plrLogger.ForContext("@ItemToBuy", group.Key);
                var itemToBuy = group.Key;
                var count = group.Count();
                logger.Debug("Trying to buy item");

                var shopItem = _gameDataService.GetShopItem(itemToBuy.ItemNumber);
                if (shopItem == null)
                {
                    logger.Warning("Trying to buy non-existant item");
                    session.Send(new ItemBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                    continue;
                }

                // TODO master level

                if (shopItem.MinLevel > plr.Level || shopItem.MaxLevel != 0 && shopItem.MaxLevel < plr.Level)
                {
                    logger.Warning(
                        "Trying to buy item without meeting level requirements MinLevel={Minlevel} MaxLevel={MaxLevel} PlayerLevel={PlayerLevel}",
                        shopItem.MinLevel,
                        shopItem.MaxLevel,
                        plr.Level
                    );
                    session.Send(new ItemBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                    continue;
                }

                if (itemToBuy.Color > shopItem.ColorGroup)
                {
                    logger.Warning("Trying to buy item with invalid color");
                    session.Send(new ItemBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                    continue;
                }

                var shopItemInfo = shopItem.GetItemInfo(itemToBuy.PriceType);
                if (shopItemInfo == null)
                {
                    logger.Warning("Trying to buy non-existant item");
                    session.Send(new ItemBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                    continue;
                }

                if (!shopItemInfo.IsEnabled)
                {
                    logger.Warning("Trying to buy disabled item");
                    session.Send(new ItemBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                    continue;
                }

                var priceInfo = shopItemInfo.PriceGroup.GetPrice(itemToBuy.PeriodType, itemToBuy.Period);
                if (priceInfo == null)
                {
                    logger.Warning("Trying to buy item with invalid price info");
                    session.Send(new ItemBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                    continue;
                }

                if (!priceInfo.IsEnabled)
                {
                    logger.Warning("Trying to buy item with disabled price info");
                    session.Send(new ItemBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                    continue;
                }

                var cost = (uint)(priceInfo.Price * count);
                switch (itemToBuy.PriceType)
                {
                    case ItemPriceType.PEN:
                        if (plr.PEN < cost)
                        {
                            logger.Warning("Trying to buy item without enough PEN currentPEN={CurrentPEN}", plr.PEN);
                            session.Send(new ItemBuyItemAckMessage(itemToBuy, ItemBuyResult.NotEnoughMoney));
                            return true;
                        }

                        plr.PEN -= cost;

                        break;

                    case ItemPriceType.AP:
                    case ItemPriceType.Premium:
                        if (plr.AP < cost)
                        {
                            logger.Warning("Trying to buy item without enough AP currentAP={CurrentAP}", plr.AP);
                            session.Send(new ItemBuyItemAckMessage(itemToBuy, ItemBuyResult.NotEnoughMoney));
                            return true;
                        }

                        plr.AP -= cost;

                        break;

                    // TODO Implement other price types

                    default:
                        logger.Warning("Trying to buy item with invalid price type");
                        session.Send(new ItemBuyItemAckMessage(itemToBuy, ItemBuyResult.DBError));
                        return true;
                }

                try
                {
                    var effects = Array.Empty<uint>();
                    if (shopItemInfo.EffectGroup.Effects.Count > 0)
                        effects = shopItemInfo.EffectGroup.Effects.Select(x => x.Effect).Where(x => x != 0).ToArray();

                    for (var i = 0; i < count; ++i)
                    {
                        var newItem = plr.Inventory.Create(
                            shopItemInfo,
                            priceInfo,
                            itemToBuy.Color,
                            effects
                        );
                        newItems.Add(newItem);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Unable to create item");
                }

                var newItemIds = newItems.Select(x => x.Id).ToArray();
                session.Send(new ItemBuyItemAckMessage(newItemIds, itemToBuy));
                newItems.Clear();

                plr.SendMoneyUpdate();
            }

            return true;
        }
    }
}
