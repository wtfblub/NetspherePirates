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

        [MessageHandler(typeof(ItemUseItemReqMessage))]
        public void UseItemHandler(GameSession session, ItemUseItemReqMessage message)
        {
            // This is a weird thing since newer seasons
            // The client sends a request with itemid 0 on login
            // and requires an answer to it for equipment to work properly
            if (message.Action == UseItemAction.UnEquip && message.ItemId == 0)
            {
                session.SendAsync(new ItemUseItemAckMessage(message.Action, message.CharacterSlot, message.EquipSlot, message.ItemId));
                return;
            }

            var plr = session.Player;
            var @char = plr.CharacterManager[message.CharacterSlot];
            var item = plr.Inventory[message.ItemId];

            if (@char == null || item == null || (plr.Room != null && plr.RoomInfo.State != PlayerState.Lobby))
            {
                session.SendAsync(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
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
                session.SendAsync(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
            }
        }

        [MessageHandler(typeof(ItemRepairItemReqMessage))]
        public async Task RepairItemHandler(GameSession session, ItemRepairItemReqMessage message)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();

            foreach (var id in message.Items)
            {
                var item = session.Player.Inventory[id];
                if (item == null)
                {
                    Logger.ForAccount(session)
                        .Error("Item {id} not found", id);
                    session.SendAsync(new ItemRepairItemAckMessage { Result = ItemRepairResult.Error0 });
                    return;
                }
                if (item.Durability == -1)
                {
                    Logger.ForAccount(session)
                        .Error("Item {item} can not be repaired", new { item.ItemNumber, item.PriceType, item.PeriodType, item.Period });
                    session.SendAsync(new ItemRepairItemAckMessage { Result = ItemRepairResult.Error1 });
                    return;
                }

                var cost = item.CalculateRepair();
                if (session.Player.PEN < cost)
                {
                    session.SendAsync(new ItemRepairItemAckMessage { Result = ItemRepairResult.NotEnoughMoney });
                    return;
                }

                var price = shop.GetPrice(item);
                if (price == null)
                {
                    Logger.ForAccount(session)
                        .Error("No shop entry found for {item}", new { item.ItemNumber, item.PriceType, item.PeriodType, item.Period });
                    session.SendAsync(new ItemRepairItemAckMessage { Result = ItemRepairResult.Error4 });
                    return;
                }
                if (item.Durability >= price.Durability)
                {
                    await session.SendAsync(new ItemRepairItemAckMessage { Result = ItemRepairResult.OK, ItemId = item.Id });
                    continue;
                }

                item.Durability = price.Durability;
                session.Player.PEN -= cost;

                await session.SendAsync(new ItemRepairItemAckMessage { Result = ItemRepairResult.OK, ItemId = item.Id });
                await session.SendAsync(new MoneyRefreshCashInfoAckMessage { PEN = session.Player.PEN, AP = session.Player.AP });
            }
        }

        [MessageHandler(typeof(ItemRefundItemReqMessage))]
        public void RefundItemHandler(GameSession session, ItemRefundItemReqMessage message)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();

            var item = session.Player.Inventory[message.ItemId];
            if (item == null)
            {
                Logger.ForAccount(session)
                    .Error("Item {itemId} not found", message.ItemId);
                session.SendAsync(new ItemRefundItemAckMessage { Result = ItemRefundResult.Failed });
                return;
            }

            var price = shop.GetPrice(item);
            if (price == null)
            {
                Logger.ForAccount(session)
                    .Error("No shop entry found for {item}", new { item.ItemNumber, item.PriceType, item.PeriodType, item.Period });
                session.SendAsync(new ItemRefundItemAckMessage { Result = ItemRefundResult.Failed });
                return;
            }
            if (!price.CanRefund)
            {
                Logger.ForAccount(session)
                    .Error("Cannot refund {item}", new { item.ItemNumber, item.PriceType, item.PeriodType, item.Period });
                session.SendAsync(new ItemRefundItemAckMessage { Result = ItemRefundResult.Failed });
                return;
            }

            session.Player.PEN += item.CalculateRefund();
            session.Player.Inventory.Remove(item);

            session.SendAsync(new ItemRefundItemAckMessage { Result = ItemRefundResult.OK, ItemId = item.Id });
            session.SendAsync(new MoneyRefreshCashInfoAckMessage { PEN = session.Player.PEN, AP = session.Player.AP });
        }

        [MessageHandler(typeof(ItemDiscardItemReqMessage))]
        public void DiscardItemHandler(GameSession session, ItemRefundItemReqMessage message)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();

            var item = session.Player.Inventory[message.ItemId];
            if (item == null)
            {
                Logger.ForAccount(session)
                    .Error("Item {itemId} not found", message.ItemId);
                session.SendAsync(new ItemDiscardItemAckMessage { Result = 2 });
                return;
            }

            var shopItem = shop.GetItem(item.ItemNumber);
            if (shopItem == null)
            {
                Logger.ForAccount(session)
                    .Error("No shop entry found for item {item}", new { item.ItemNumber, item.PriceType, item.PeriodType, item.Period });
                session.SendAsync(new ItemDiscardItemAckMessage { Result = 2 });
                return;
            }

            if (shopItem.IsDestroyable)
            {
                Logger.ForAccount(session)
                    .Error("Cannot discord {item}", new { item.ItemNumber, item.PriceType, item.PeriodType, item.Period });
                session.SendAsync(new ItemDiscardItemAckMessage { Result = 2 });
                return;
            }

            session.Player.Inventory.Remove(item);
            session.SendAsync(new ItemDiscardItemAckMessage { Result = 0, ItemId = item.Id });
        }

        [MessageHandler(typeof(ItemUseCapsuleReqMessage))]
        public void UseCapsuleReq(GameSession session, ItemUseCapsuleReqMessage message)
        {
            session.SendAsync(new ServerResultAckMessage((ServerResult)1));
            //session.Send(new ItemUseCapsuleAckMessage(new List<CapsuleRewardDto>
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

