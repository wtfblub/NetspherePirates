using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Microsoft.Extensions.Options;
using Netsphere.Common.Configuration;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Data;
using Netsphere.Server.Game.Rules;
using Netsphere.Server.Game.Services;
using ProudNet;

namespace Netsphere.Server.Game.Handlers
{
    internal class ShopHandler : IHandle<CLicensedReqMessage>, IHandle<CExerciseLicenceReqMessage>, IHandle<CBuyItemReqMessage>
    {
        private readonly GameDataService _gameDataService;
        private readonly ILogger _logger;
        private readonly GameOptions _gameOptions;

        public ShopHandler(GameDataService gameDataService, IOptions<GameOptions> gameOptions, ILogger<ShopHandler> logger)
        {
            _gameDataService = gameDataService;
            _logger = logger;
            _gameOptions = gameOptions.Value;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        public Task<bool> OnHandle(MessageContext context, CLicensedReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            plr.LicenseManager.Acquire(message.License);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeLoggedIn))]
        public Task<bool> OnHandle(MessageContext context, CExerciseLicenceReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            plr.LicenseManager.Acquire(message.License);
            return Task.FromResult(true);
        }

        public async Task<bool> OnHandle(MessageContext context, CBuyItemReqMessage message)
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
                var itemInfo = _gameDataService.Items.GetValueOrDefault(itemToBuy.ItemNumber);
                var hasLicense = !_gameOptions.EnableLicenseRequirement || plr.LicenseManager.Contains(itemInfo.License);

                logger.Debug("Trying to buy item");

                if (itemInfo.License != ItemLicense.None && !hasLicense)
                {
                    logger.Warning("Trying to buy item without required license");
                    session.Send(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                    continue;
                }

                if (itemInfo.Level > plr.Level)
                {
                    logger.Warning("Trying to buy item without required level playerLevel={PlayerLevel}", plr.Level);
                    session.Send(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                    continue;
                }

                // TODO master level

                var shopItem = _gameDataService.GetShopItem(itemToBuy.ItemNumber);
                if (shopItem == null)
                {
                    logger.Warning("Trying to buy non-existant item");
                    session.Send(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                    continue;
                }

                if (itemToBuy.Color > shopItem.ColorGroup)
                {
                    logger.Warning("Trying to buy item with invalid color");
                    session.Send(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                    continue;
                }

                var shopItemInfo = shopItem.GetItemInfo(itemToBuy.PriceType);
                if (shopItemInfo == null)
                {
                    logger.Warning("Trying to buy non-existant item");
                    session.Send(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                    continue;
                }

                if (!shopItemInfo.IsEnabled)
                {
                    logger.Warning("Trying to buy disabled item");
                    session.Send(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                    continue;
                }

                var priceInfo = shopItemInfo.PriceGroup.GetPrice(itemToBuy.PeriodType, itemToBuy.Period);
                if (priceInfo == null)
                {
                    logger.Warning("Trying to buy item with invalid price info");
                    session.Send(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                    continue;
                }

                if (!priceInfo.IsEnabled)
                {
                    logger.Warning("Trying to buy item with disabled price info");
                    session.Send(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                    continue;
                }

                ShopEffect effectInfo = null;
                if (itemToBuy.Effect != 0)
                    effectInfo = shopItemInfo.EffectGroup.GetEffectByEffect(itemToBuy.Effect);

                var cost = (uint)(priceInfo.Price * count);
                switch (itemToBuy.PriceType)
                {
                    case ItemPriceType.PEN:
                        if (plr.PEN < cost)
                        {
                            logger.Warning("Trying to buy item without enough PEN currentPEN={CurrentPEN}", plr.PEN);
                            session.Send(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.NotEnoughMoney));
                            return true;
                        }

                        plr.PEN -= cost;

                        break;

                    case ItemPriceType.AP:
                    case ItemPriceType.Premium:
                        if (plr.AP < cost)
                        {
                            logger.Warning("Trying to buy item without enough AP currentAP={CurrentAP}", plr.AP);
                            session.Send(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.NotEnoughMoney));
                            return true;
                        }

                        plr.AP -= cost;

                        break;

                    // TODO Implement other price types

                    default:
                        logger.Warning("Trying to buy item with invalid price type");
                        session.Send(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.DBError));
                        return true;
                }

                try
                {
                    for (var i = 0; i < count; ++i)
                    {
                        var newItem = plr.Inventory.Create(shopItemInfo, priceInfo, itemToBuy.Color,
                            effectInfo?.Effect ?? 0, 0);
                        newItems.Add(newItem);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Unable to create item");
                }

                var newItemIds = newItems.Select(x => x.Id).ToArray();
                session.Send(new SBuyItemAckMessage(newItemIds, itemToBuy));
                newItems.Clear();

                plr.SendMoneyUpdate();
            }

            return true;
        }
    }
}
