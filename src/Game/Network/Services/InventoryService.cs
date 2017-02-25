﻿using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using Netsphere.Network.Message.Game;
using NLog;
using NLog.Fluent;
using ProudNet.Handlers;

namespace Netsphere.Network.Services
{
    internal class InventoryService : ProudMessageHandler
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [MessageHandler(typeof(CUseItemReqMessage))]
        public async Task UseItemHandler(GameSession session, CUseItemReqMessage message)
        {
            var plr = session.Player;
            var @char = plr.CharacterManager[message.CharacterSlot];
            var item = plr.Inventory[message.ItemId];

            if (@char == null || item == null || (plr.Room != null && plr.RoomInfo.State != PlayerState.Lobby))
            {
                await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask))
                    .ConfigureAwait(false);
                return;
            }

            try
            {
                switch (message.Action)
                {
                    case UseItemAction.Equip:
                        @char.Equip(item, message.EquipSlot);
                        break;

                    case UseItemAction.UnEquip:
                        @char.UnEquip(item.ItemNumber.Category, message.EquipSlot);
                        break;
                }
            }
            catch (CharacterException ex)
            {
                Logger.Error()
                    .Account(session)
                    .Exception(ex)
                    .Write();
                await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask))
                    .ConfigureAwait(false);
            }
        }

        [MessageHandler(typeof(CRepairItemReqMessage))]
        public async Task RepairItemHandler(GameSession session, CRepairItemReqMessage message)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();

            foreach (var id in message.Items)
            {
                var item = session.Player.Inventory[id];
                if (item == null)
                {
                    Logger.Error()
                        .Account(session)
                        .Message($"Item {id} not found")
                        .Write();
                    await session.SendAsync(new SRepairItemAckMessage { Result = ItemRepairResult.Error0 })
                        .ConfigureAwait(false);
                    return;
                }
                if (item.Durability == -1)
                {
                    Logger.Error()
                        .Account(session)
                        .Message($"Item {item.ItemNumber} {item.PriceType} {item.PeriodType} {item.Period} can not be repaired")
                        .Write();
                    await session.SendAsync(new SRepairItemAckMessage { Result = ItemRepairResult.Error1 })
                        .ConfigureAwait(false);
                    return;
                }

                var cost = item.CalculateRepair();
                if (session.Player.PEN < cost)
                {
                    await session.SendAsync(new SRepairItemAckMessage { Result = ItemRepairResult.NotEnoughMoney })
                        .ConfigureAwait(false);
                    return;
                }

                var price = shop.GetPrice(item);
                if (price == null)
                {
                    Logger.Error()
                        .Account(session)
                        .Message($"No shop entry found for {item.ItemNumber} {item.PriceType} {item.PeriodType} {item.Period}")
                        .Write();
                    await session.SendAsync(new SRepairItemAckMessage { Result = ItemRepairResult.Error4 })
                        .ConfigureAwait(false);
                    return;
                }
                if (item.Durability >= price.Durability)
                {
                    await session.SendAsync(new SRepairItemAckMessage { Result = ItemRepairResult.OK, ItemId = item.Id })
                        .ConfigureAwait(false);
                    continue;
                }

                item.Durability = price.Durability;
                session.Player.PEN -= cost;

                await session.SendAsync(new SRepairItemAckMessage { Result = ItemRepairResult.OK, ItemId = item.Id })
                        .ConfigureAwait(false);
                await session.SendAsync(new SRefreshCashInfoAckMessage { PEN = session.Player.PEN, AP = session.Player.AP })
                        .ConfigureAwait(false);
            }
        }

        [MessageHandler(typeof(CRefundItemReqMessage))]
        public async Task RefundItemHandler(GameSession session, CRefundItemReqMessage message)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();

            var item = session.Player.Inventory[message.ItemId];
            if (item == null)
            {
                Logger.Error()
                    .Account(session)
                    .Message($"Item {message.ItemId} not found")
                    .Write();
                await session.SendAsync(new SRefundItemAckMessage { Result = ItemRefundResult.Failed })
                    .ConfigureAwait(false);
                return;
            }

            var price = shop.GetPrice(item);
            if (price == null)
            {
                Logger.Error()
                    .Account(session)
                    .Message($"No shop entry found for {item.ItemNumber} {item.PriceType} {item.PeriodType} {item.Period}")
                    .Write();
                await session.SendAsync(new SRefundItemAckMessage { Result = ItemRefundResult.Failed })
                    .ConfigureAwait(false);
                return;
            }
            if (!price.CanRefund)
            {
                Logger.Error()
                    .Account(session)
                    .Message($"Cannot refund {item.ItemNumber} {item.PriceType} {item.PeriodType} {item.Period}")
                    .Write();
                await session.SendAsync(new SRefundItemAckMessage { Result = ItemRefundResult.Failed })
                    .ConfigureAwait(false);
                return;
            }

            session.Player.PEN += item.CalculateRefund();
            session.Player.Inventory.Remove(item);

            await session.SendAsync(new SRefundItemAckMessage { Result = ItemRefundResult.OK, ItemId = item.Id })
                    .ConfigureAwait(false);
            await session.SendAsync(new SRefreshCashInfoAckMessage { PEN = session.Player.PEN, AP = session.Player.AP })
                    .ConfigureAwait(false);
        }

        [MessageHandler(typeof(CDiscardItemReqMessage))]
        public async Task DiscardItemHandler(GameSession session, CRefundItemReqMessage message)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();

            var item = session.Player.Inventory[message.ItemId];
            if (item == null)
            {
                Logger.Error()
                    .Account(session)
                    .Message($"Item {message.ItemId} not found")
                    .Write();
                await session.SendAsync(new SDiscardItemAckMessage { Result = 2 })
                    .ConfigureAwait(false);
                return;
            }

            var shopItem = shop.GetItem(item.ItemNumber);
            if (shopItem == null)
            {
                Logger.Error()
                    .Account(session)
                    .Message($"No shop entry found for {item.ItemNumber} {item.PriceType} {item.PeriodType} {item.Period}")
                    .Write();
                await session.SendAsync(new SDiscardItemAckMessage { Result = 2 })
                    .ConfigureAwait(false);
                return;
            }
            if (shopItem.IsDestroyable)
            {
                Logger.Error()
                    .Account(session)
                    .Message($"Cannot discard {item.ItemNumber} {item.PriceType} {item.PeriodType} {item.Period}")
                    .Write();
                await session.SendAsync(new SDiscardItemAckMessage { Result = 2 })
                    .ConfigureAwait(false);
                return;
            }

            session.Player.Inventory.Remove(item);
            await session.SendAsync(new SDiscardItemAckMessage { Result = 0, ItemId = item.Id })
                .ConfigureAwait(false);
        }

        [MessageHandler(typeof(CUseCapsuleReqMessage))]
        public async Task UseCapsuleReq(GameSession session, CUseCapsuleReqMessage message)
        {
            await session.SendAsync(new SServerResultInfoAckMessage((ServerResult)1))
                .ConfigureAwait(false);
            //session.Send(new SUseCapsuleAckMessage(new List<CapsuleRewardDto>
            //{
            //    new CapsuleRewardDto(CapsuleRewardType.Item, 0, 64, 0),
            //    new CapsuleRewardDto(CapsuleRewardType.Item, 0, 154, 123),
            //    new CapsuleRewardDto(CapsuleRewardType.PEN, 9999, 0, 0),
            //    //new CapsuleRewardDto(CapsuleRewardType.PEN, 2, 0, 0),
            //    //new CapsuleRewardDto(CapsuleRewardType.PEN, 3, 0, 0),
            //}, 3));
        }
    }
}
