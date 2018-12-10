using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Data;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game
{
    public class PlayerItem : DatabaseObject
    {
        private readonly GameDataService _gameDataService;
        private int _durability;
        private uint _count;

        public PlayerInventory Inventory { get; }

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
            set => SetIfChanged(ref _durability, value);
        }
        public uint Count
        {
            get => _count;
            set => SetIfChanged(ref _count, value);
        }
        public DateTimeOffset ExpireDate =>
            PeriodType == ItemPeriodType.Days ? PurchaseDate.AddDays(Period) : DateTimeOffset.MinValue;

        public CharacterInventory CharacterInventory { get; internal set; }

        internal PlayerItem(GameDataService gameDataService, PlayerInventory inventory, PlayerItemEntity entity)
        {
            _gameDataService = gameDataService;
            Inventory = inventory;
            Id = (ulong)entity.Id;

            var itemInfo = _gameDataService.ShopItems.Values.First(group => group.GetItemInfo(entity.ShopItemInfoId) != null);
            ItemNumber = itemInfo.ItemNumber;

            var priceGroup = _gameDataService.ShopPrices.Values.First(group => group.GetPrice(entity.ShopPriceId) != null);
            var price = priceGroup.GetPrice(entity.ShopPriceId);

            PriceType = priceGroup.PriceType;
            PeriodType = price.PeriodType;
            Period = price.Period;
            Color = entity.Color;
            Effect = entity.Effect;
            PurchaseDate = DateTimeOffset.FromUnixTimeSeconds(entity.PurchaseDate);
            _durability = entity.Durability;
            _count = (uint)entity.Count;

            SetExistsState(true);
        }

        internal PlayerItem(GameDataService gameDataService, PlayerInventory inventory, long id,
            ShopItemInfo itemInfo, ShopPrice price,
            byte color, uint effect, DateTimeOffset purchaseDate, uint count)
        {
            _gameDataService = gameDataService;
            Inventory = inventory;
            Id = (ulong)id;
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

            return _gameDataService.Effects.GetValueOrDefault(Effect);
        }

        public ShopItem GetShopItem()
        {
            return _gameDataService.GetShopItem(ItemNumber);
        }

        public ShopItemInfo GetShopItemInfo()
        {
            return _gameDataService.GetShopItemInfo(ItemNumber, PriceType);
        }

        public ShopPrice GetShopPrice()
        {
            return GetShopItemInfo().PriceGroup.GetPrice(PeriodType, Period);
        }

        public void LoseDurability(int loss)
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

            Inventory.Player.Session.Send(new SItemDurabilityInfoAckMessage(
                new[] { this.Map<PlayerItem, ItemDurabilityInfoDto>() }));
        }

        // TODO Calculate refund/repair cost

        public uint CalculateRefund()
        {
            return 0;
        }

        public uint CalculateRepair()
        {
            return 0;
        }
    }
}
