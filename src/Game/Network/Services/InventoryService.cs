using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using Netsphere.Network.Message.Game;
using ProudNet.Handlers;
using Serilog;
using Serilog.Core;

namespace Netsphere.Network.Services
{
    internal class InventoryService : ProudMessageHandler
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ILogger Logger = Log.ForContext(Constants.SourceContextPropertyName, nameof(InventoryService));

        [MessageHandler(typeof(CUseItemReqMessage))]
        public void UseItemHandler(GameSession session, CUseItemReqMessage message)
        {
            var plr = session.Player;
            var @char = plr.CharacterManager[message.CharacterSlot];
            var item = plr.Inventory[message.ItemId];

            if (@char == null || item == null || (plr.Room != null && plr.RoomInfo.State != PlayerState.Lobby))
            {
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
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
                Logger.ForAccount(session)
                    .Error(ex, "Unable to use item");
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
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
                    Logger.ForAccount(session)
                        .Error("Item {id} not found", id);
                    session.SendAsync(new SRepairItemAckMessage { Result = ItemRepairResult.Error0 });
                    return;
                }
                if (item.Durability == -1)
                {
                    Logger.ForAccount(session)
                        .Error("Item {item} can not be repaired", new { item.ItemNumber, item.PriceType, item.PeriodType, item.Period });
                    session.SendAsync(new SRepairItemAckMessage { Result = ItemRepairResult.Error1 });
                    return;
                }

                var cost = item.CalculateRepair();
                if (session.Player.PEN < cost)
                {
                    session.SendAsync(new SRepairItemAckMessage { Result = ItemRepairResult.NotEnoughMoney });
                    return;
                }

                var price = shop.GetPrice(item);
                if (price == null)
                {
                    Logger.ForAccount(session)
                        .Error("No shop entry found for {item}", new { item.ItemNumber, item.PriceType, item.PeriodType, item.Period });
                    session.SendAsync(new SRepairItemAckMessage { Result = ItemRepairResult.Error4 });
                    return;
                }
                if (item.Durability >= price.Durability)
                {
                    await session.SendAsync(new SRepairItemAckMessage { Result = ItemRepairResult.OK, ItemId = item.Id });
                    continue;
                }

                item.Durability = price.Durability;
                session.Player.PEN -= cost;

                await session.SendAsync(new SRepairItemAckMessage { Result = ItemRepairResult.OK, ItemId = item.Id });
                await session.SendAsync(new SRefreshCashInfoAckMessage { PEN = session.Player.PEN, AP = session.Player.AP });
            }
        }

        [MessageHandler(typeof(CRefundItemReqMessage))]
        public void RefundItemHandler(GameSession session, CRefundItemReqMessage message)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();

            var item = session.Player.Inventory[message.ItemId];
            if (item == null)
            {
                Logger.ForAccount(session)
                    .Error("Item {itemId} not found", message.ItemId);
                session.SendAsync(new SRefundItemAckMessage { Result = ItemRefundResult.Failed });
                return;
            }

            var price = shop.GetPrice(item);
            if (price == null)
            {
                Logger.ForAccount(session)
                    .Error("No shop entry found for {item}", new { item.ItemNumber, item.PriceType, item.PeriodType, item.Period });
                session.SendAsync(new SRefundItemAckMessage { Result = ItemRefundResult.Failed });
                return;
            }
            if (!price.CanRefund)
            {
                Logger.ForAccount(session)
                    .Error("Cannot refund {item}", new { item.ItemNumber, item.PriceType, item.PeriodType, item.Period });
                session.SendAsync(new SRefundItemAckMessage { Result = ItemRefundResult.Failed });
                return;
            }

            session.Player.PEN += item.CalculateRefund();
            session.Player.Inventory.Remove(item);

            session.SendAsync(new SRefundItemAckMessage { Result = ItemRefundResult.OK, ItemId = item.Id });
            session.SendAsync(new SRefreshCashInfoAckMessage { PEN = session.Player.PEN, AP = session.Player.AP });
        }

        [MessageHandler(typeof(CDiscardItemReqMessage))]
        public void DiscardItemHandler(GameSession session, CRefundItemReqMessage message)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();

            var item = session.Player.Inventory[message.ItemId];
            if (item == null)
            {
                Logger.ForAccount(session)
                    .Error("Item {itemId} not found", message.ItemId);
                session.SendAsync(new SDiscardItemAckMessage { Result = 2 });
                return;
            }

            var shopItem = shop.GetItem(item.ItemNumber);
            if (shopItem == null)
            {
                Logger.ForAccount(session)
                    .Error("No shop entry found for item {item}", new { item.ItemNumber, item.PriceType, item.PeriodType, item.Period });
                session.SendAsync(new SDiscardItemAckMessage { Result = 2 });
                return;
            }

            if (shopItem.IsDestroyable)
            {
                Logger.ForAccount(session)
                    .Error("Cannot discord {item}", new { item.ItemNumber, item.PriceType, item.PeriodType, item.Period });
                session.SendAsync(new SDiscardItemAckMessage { Result = 2 });
                return;
            }

            session.Player.Inventory.Remove(item);
            session.SendAsync(new SDiscardItemAckMessage { Result = 0, ItemId = item.Id });
        }

        [MessageHandler(typeof(CUseCapsuleReqMessage))]
        public void UseCapsuleReq(GameSession session, CUseCapsuleReqMessage message)
        {
            session.SendAsync(new SServerResultInfoAckMessage((ServerResult)1));
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
