using System.Threading.Tasks;
using Logging;
using Netsphere.Network;
using Netsphere.Network.Message.Game;
using ProudNet;

namespace Netsphere.Server.Game.Handlers
{
    internal class InventoryHandler
        : IHandle<ItemUseItemReqMessage>, IHandle<ItemRepairItemReqMessage>, IHandle<ItemRefundItemReqMessage>,
          IHandle<ItemDiscardItemReqMessage>
    {
        private readonly ILogger _logger;

        public InventoryHandler(ILogger<InventoryHandler> logger)
        {
            _logger = logger;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ItemUseItemReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var character = plr.CharacterManager[message.CharacterSlot];
            var item = plr.Inventory[message.ItemId];

            if (character == null || item == null || plr.Room != null && plr.State != PlayerState.Lobby)
            {
                session.Send(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
                return true;
            }

            switch (message.Action)
            {
                case UseItemAction.Equip:
                    character.Equip(item, message.EquipSlot);
                    break;

                case UseItemAction.UnEquip:
                    character.UnEquip(item.ItemNumber.Category, message.EquipSlot);
                    break;
            }

            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ItemRepairItemReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var logger = plr.AddContextToLogger(_logger);

            foreach (var id in message.Items)
            {
                var item = session.Player.Inventory[id];
                if (item == null)
                {
                    logger.Warning("Item={ItemId} not found", id);
                    session.Send(new SRepairItemAckMessage(ItemRepairResult.Error0, 0));
                    return true;
                }

                if (item.Durability == -1)
                {
                    logger.Warning("Item={ItemId} can not be repaired", id);
                    session.Send(new SRepairItemAckMessage(ItemRepairResult.Error1, 0));
                    return true;
                }

                var cost = item.CalculateRepair();
                if (plr.PEN < cost)
                {
                    session.Send(new SRepairItemAckMessage(ItemRepairResult.NotEnoughMoney, 0));
                    return true;
                }

                var price = item.GetShopPrice();
                if (price == null)
                {
                    logger.Warning("No shop entry found item={ItemId}", id);
                    session.Send(new SRepairItemAckMessage(ItemRepairResult.Error2, 0));
                    return true;
                }

                if (item.Durability >= price.Durability)
                {
                    session.Send(new SRepairItemAckMessage(ItemRepairResult.OK, item.Id));
                    continue;
                }

                item.Durability = price.Durability;
                plr.PEN -= cost;

                session.Send(new SRepairItemAckMessage(ItemRepairResult.OK, item.Id));
                plr.SendMoneyUpdate();
            }

            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ItemRefundItemReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var item = plr.Inventory[message.ItemId];
            var logger = plr.AddContextToLogger(_logger);

            if (item == null)
            {
                logger.Warning("Item={ItemId} not found", message.ItemId);
                session.Send(new SRefundItemAckMessage(ItemRefundResult.Failed, 0));
                return true;
            }

            var price = item.GetShopPrice();
            if (price == null)
            {
                logger.Warning("No shop entry found item={ItemId}", message.ItemId);
                session.Send(new SRefundItemAckMessage(ItemRefundResult.Failed, 0));
                return true;
            }

            if (!price.CanRefund)
            {
                logger.Warning("Cannot refund item={ItemId}", message.ItemId);
                session.Send(new SRefundItemAckMessage(ItemRefundResult.Failed, 0));
                return true;
            }

            plr.PEN += item.CalculateRefund();
            plr.Inventory.Remove(item);

            session.Send(new SRefundItemAckMessage(ItemRefundResult.OK, item.Id));
            plr.SendMoneyUpdate();

            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ItemDiscardItemReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var item = plr.Inventory[message.ItemId];
            var logger = plr.AddContextToLogger(_logger);

            if (item == null)
            {
                logger.Warning("Item={ItemId} not found", message.ItemId);
                session.Send(new SDiscardItemAckMessage(2, 0));
                return true;
            }

            var shopItem = item.GetShopItem();
            if (shopItem == null)
            {
                logger.Warning("No shop entry found item={ItemId}", message.ItemId);
                session.Send(new SDiscardItemAckMessage(2, 0));
                return true;
            }

            if (!shopItem.IsDestroyable)
            {
                logger.Warning("Cannot discard item={ItemId}", message.ItemId);
                session.Send(new SDiscardItemAckMessage(2, 0));
                return true;
            }

            plr.Inventory.Remove(item);
            session.Send(new SDiscardItemAckMessage(0, item.Id));

            return true;
        }
    }
}
