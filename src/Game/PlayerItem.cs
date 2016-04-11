using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Netsphere.Database.Game;
using Netsphere.Network;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using Netsphere.Resource;
using Netsphere.Shop;
using Shaolinq;

namespace Netsphere
{
    internal class PlayerItem
    {
        private int _durability;
        private uint _count;

        internal bool ExistsInDatabase { get; set; }
        internal bool NeedsToSave { get; set; }

        public Inventory Inventory { get; }

        public ulong Id { get; }
        public ItemNumber ItemNumber { get; }
        public ItemPriceType PriceType { get; }
        public ItemPeriodType PeriodType { get; }
        public ushort Period { get; }
        public byte Color { get; }
        public uint Effect { get; }
        public DateTimeOffset PurchaseDate { get; }
        public int Durability
        {
            get { return _durability; }
            set
            {
                if (_durability == value)
                    return;
                _durability = value;
                NeedsToSave = true;
            }
        }
        public uint Count
        {
            get { return _count; }
            set
            {
                if (_count == value)
                    return;
                _count = value;
                NeedsToSave = true;
            }
        }

        public DateTimeOffset ExpireDate => PeriodType == ItemPeriodType.Days ? PurchaseDate.AddDays(Period) : DateTimeOffset.MinValue;

        internal PlayerItem(Inventory inventory, PlayerItemDto dto)
        {
            ExistsInDatabase = true;
            Inventory = inventory;
            Id = (ulong)dto.Id;
            ItemNumber = dto.ShopItemInfo.ShopItem.Id;
            PriceType = (ItemPriceType)dto.ShopPrice.PriceGroup.PriceType;
            PeriodType = (ItemPeriodType)dto.ShopPrice.PeriodType;
            Period = (ushort)dto.ShopPrice.Period;
            Color = dto.Color;
            Effect = dto.Effect;
            PurchaseDate = DateTimeOffset.FromUnixTimeSeconds(dto.PurchaseDate);
            _durability = dto.Durability;
            _count = (uint)dto.Count;
        }

        internal PlayerItem(Inventory inventory, ShopItemInfo itemInfo, ShopPrice price, byte color, uint effect, DateTimeOffset purchaseDate, uint count)
        {
            Inventory = inventory;
            Id = ItemIdGenerator.GetNextId();
            ItemNumber = itemInfo.ShopItem.ItemNumber;
            PriceType = itemInfo.PriceGroup.PriceType;
            PeriodType = price.PeriodType;
            Period = price.Period;
            Color = color;
            Effect = effect;
            PurchaseDate = purchaseDate;
            _durability = price.Durability;
            _count = count;
        }

        public ItemEffect GetItemEffect()
        {
            if (Effect == 0)
                return null;

            var effects = GameServer.Instance.ResourceCache.GetEffects();
            return effects.GetValueOrDefault(Effect);
        }

        public ShopItem GetShopItem()
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();
            return shop.GetItem(ItemNumber);
        }

        public ShopItemInfo GetShopItemInfo()
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();
            return shop.GetItemInfo(ItemNumber, PriceType);
        }

        public async Task LoseDurabilityAsync(int loss)
        {
            if (loss < 0)
                throw new ArgumentOutOfRangeException(nameof(loss));

            if (Inventory.Player.Room == null)
                throw new InvalidOperationException("Player is not inside a room");

            if (Durability == -1)
                return;

            Durability -= loss;
            if (Durability < 0)
                Durability = 0;

            using (var scope = new DataAccessScope())
            {
                var dto = GameDatabase.Instance.PlayerItems.GetReference((int)Id);
                dto.Durability = Durability;

                await scope.CompleteAsync()
                    .ConfigureAwait(false);
            }

            await Inventory.Player.Session.SendAsync(new SItemDurabilityInfoAckMessage(new[] { this.Map<PlayerItem, ItemDurabilityInfoDto>() }))
                        .ConfigureAwait(false);
        }

        public uint CalculateRefund()
        {
            return 0; // ToDo
        }

        public uint CalculateRepair()
        {
            return 0; // Todo
        }
    }
}
