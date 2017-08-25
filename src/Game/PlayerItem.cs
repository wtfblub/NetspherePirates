using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Netsphere.Database.Game;
using Netsphere.Network;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using Netsphere.Resource;
using Netsphere.Shop;

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
            get => _durability;
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
            get => _count;
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
            var shop = GameServer.Instance.ResourceCache.GetShop();
            ExistsInDatabase = true;
            Inventory = inventory;
            Id = (ulong)dto.Id;

            var itemInfo = shop.Items.Values.First(group => group.GetItemInfo(dto.ShopItemInfoId) != null);
            ItemNumber = itemInfo.ItemNumber;

            var priceGroup = shop.Prices.Values.First(group => group.GetPrice(dto.ShopPriceId) != null);
            var price = priceGroup.GetPrice(dto.ShopPriceId);

            PriceType = priceGroup.PriceType;
            PeriodType = price.PeriodType;
            Period = price.Period;
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

        public ShopPrice GetShopPrice()
        {
            return GetShopItemInfo().PriceGroup.GetPrice(PeriodType, Period);
        }

        public Task LoseDurabilityAsync(int loss)
        {
            if (loss < 0)
                throw new ArgumentOutOfRangeException(nameof(loss));

            if (Inventory.Player.Room == null)
                throw new InvalidOperationException("Player is not inside a room");

            if (Durability == -1)
                return Task.CompletedTask;

            Durability -= loss;
            if (Durability < 0)
                Durability = 0;

            return Inventory.Player.Session.SendAsync(new ItemDurabilityItemAckMessage(new[] { this.Map<PlayerItem, ItemDurabilityInfoDto>() }));
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
