using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly ILoggerFactory _loggerFactory;
        private readonly GameOptions _gameOptions;

        public ShopHandler(GameDataService gameDataService, IOptions<GameOptions> gameOptions, ILoggerFactory loggerFactory)
        {
            _gameDataService = gameDataService;
            _loggerFactory = loggerFactory;
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

            var itemsTobuy = message.Items.GroupBy(x => x);
            var newItems = new List<PlayerItem>();

            var logger = _loggerFactory.CreateLogger<ShopHandler>();
            using (plr.AddContextToLogger(logger))
            {
                foreach (var group in itemsTobuy)
                {
                    using (logger.BeginScope("ItemToBuy={@ItemToBuy}", @group.Key))
                    {
                        var itemToBuy = group.Key;
                        var count = group.Count();
                        var itemInfo = _gameDataService.Items.GetValueOrDefault(itemToBuy.ItemNumber);
                        var hasLicense = !_gameOptions.EnableLicenseRequirement || plr.LicenseManager.Contains(itemInfo.License);

                        logger.LogDebug("Trying to buy item");

                        if (itemInfo.License != ItemLicense.None && !hasLicense)
                        {
                            logger.LogWarning("Trying to buy item without required license");
                            await session.SendAsync(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                            continue;
                        }

                        if (itemInfo.Level > plr.Level)
                        {
                            logger.LogWarning("Trying to buy item without required level playerLevel={PlayerLevel}", plr.Level);
                            await session.SendAsync(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                            continue;
                        }

                        // TODO master level

                        var shopItem = _gameDataService.GetShopItem(itemToBuy.ItemNumber);
                        if (shopItem == null)
                        {
                            logger.LogWarning("Trying to buy non-existant item");
                            await session.SendAsync(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                            continue;
                        }

                        if (itemToBuy.Color > shopItem.ColorGroup)
                        {
                            logger.LogWarning("Trying to buy item with invalid color");
                            await session.SendAsync(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                            continue;
                        }

                        var shopItemInfo = shopItem.GetItemInfo(itemToBuy.PriceType);
                        if (shopItemInfo == null)
                        {
                            logger.LogWarning("Trying to buy non-existant item");
                            await session.SendAsync(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                            continue;
                        }

                        if (!shopItemInfo.IsEnabled)
                        {
                            logger.LogWarning("Trying to buy disabled item");
                            await session.SendAsync(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                            continue;
                        }

                        var priceInfo = shopItemInfo.PriceGroup.GetPrice(itemToBuy.PeriodType, itemToBuy.Period);
                        if (priceInfo == null)
                        {
                            logger.LogWarning("Trying to buy item with invalid price info");
                            await session.SendAsync(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
                            continue;
                        }

                        if (!priceInfo.IsEnabled)
                        {
                            logger.LogWarning("Trying to buy item with disabled price info");
                            await session.SendAsync(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.UnkownItem));
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
                                    logger.LogWarning("Trying to buy item without enough PEN currentPEN={CurrentPEN}", plr.PEN);
                                    await session.SendAsync(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.NotEnoughMoney));
                                    return true;
                                }

                                plr.PEN -= cost;

                                break;

                            case ItemPriceType.AP:
                            case ItemPriceType.Premium:
                                if (plr.AP < cost)
                                {
                                    logger.LogWarning("Trying to buy item without enough AP currentAP={CurrentAP}", plr.AP);
                                    await session.SendAsync(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.NotEnoughMoney));
                                    return true;
                                }

                                plr.AP -= cost;

                                break;

                            // TODO Implement other price types

                            default:
                                logger.LogWarning("Trying to buy item with invalid price type");
                                await session.SendAsync(new SBuyItemAckMessage(itemToBuy, ItemBuyResult.DBError));
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
                            logger.LogError(ex, "Unable to create item");
                        }

                        var newItemIds = newItems.Select(x => x.Id).ToArray();
                        await session.SendAsync(new SBuyItemAckMessage(newItemIds, itemToBuy));
                        newItems.Clear();
                    }
                }

                await plr.SendMoneyUpdate();
            }

            return true;
        }
    }
}
