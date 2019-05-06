using System;
using System.Collections.Immutable;
using System.Linq;
using ExpressMapper.Extensions;
using Logging;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Data;
using Netsphere.Server.Game.Services;
using Newtonsoft.Json;

namespace Netsphere.Server.Game
{
    public class PlayerItem : DatabaseObject
    {
        private readonly GameDataService _gameDataService;
        private int _durability;
        private uint _enchantMP;
        private uint _enchantLevel;

        public PlayerInventory Inventory { get; }

        public ulong Id { get; }
        public ItemNumber ItemNumber { get; }
        public ItemPriceType PriceType { get; }
        public ItemPeriodType PeriodType { get; }
        public ushort Period { get; }
        public byte Color { get; }
        public PlayerItemEffectCollection Effects { get; }
        public DateTimeOffset PurchaseDate { get; }
        public int Durability
        {
            get => _durability;
            set => SetIfChanged(ref _durability, value);
        }
        public DateTimeOffset ExpireDate =>
            PeriodType == ItemPeriodType.Days ? PurchaseDate.AddDays(Period) : DateTimeOffset.MinValue;
        public uint EnchantMP
        {
            get => _enchantMP;
            set => SetIfChanged(ref _enchantMP, value);
        }
        public uint EnchantLevel
        {
            get => _enchantLevel;
            set => SetIfChanged(ref _enchantLevel, value);
        }

        public CharacterInventory CharacterInventory { get; internal set; }

        internal PlayerItem(ILogger logger, GameDataService gameDataService, PlayerInventory inventory, PlayerItemEntity entity)
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

            var effects = Array.Empty<uint>();
            if (!string.IsNullOrWhiteSpace(entity.Effects))
            {
                try
                {
                    effects = JsonConvert.DeserializeObject<uint[]>(entity.Effects);
                }
                catch (Exception ex)
                {
                    logger.Warning(
                        ex,
                        "Unable to load effects from item={ItemId} effects={Effects}",
                        entity.Id,
                        entity.Effects
                    );
                }
            }

            Effects = new PlayerItemEffectCollection(this, effects);
            PurchaseDate = DateTimeOffset.FromUnixTimeSeconds(entity.PurchaseDate);
            _durability = entity.Durability;
            _enchantMP = (uint)entity.MP;
            _enchantLevel = (uint)entity.MPLevel;

            SetExistsState(true);
        }

        internal PlayerItem(GameDataService gameDataService, PlayerInventory inventory, long id,
            ShopItemInfo itemInfo, ShopPrice price,
            byte color, uint[] effects, DateTimeOffset purchaseDate)
        {
            _gameDataService = gameDataService;
            Inventory = inventory;
            Id = (ulong)id;
            ItemNumber = itemInfo.ShopItem.ItemNumber;
            PriceType = itemInfo.PriceGroup.PriceType;
            PeriodType = price.PeriodType;
            Period = price.Period;
            Color = color;
            Effects = new PlayerItemEffectCollection(this, effects);
            PurchaseDate = purchaseDate;
            _durability = price.Durability;
        }

        public ItemEffect[] GetItemEffects()
        {
            if (Effects.Count == 0)
                return null;

            return Effects.Select(x => _gameDataService.Effects.GetValueOrDefault(x)).ToArray();
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

            Inventory.Player.Session.Send(new ItemDurabilityItemAckMessage(
                new[]
                {
                    this.Map<PlayerItem, ItemDurabilityInfoDto>()
                })
            );
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
