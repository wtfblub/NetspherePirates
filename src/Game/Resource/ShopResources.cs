using System.Collections.Generic;
using System.Linq;
using Dapper.FastCrud;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Shop;

namespace Netsphere.Resource
{
    internal class     ShopResources
    {
        private Dictionary<ItemNumber, ShopItem> _items;
        private Dictionary<int, ShopEffectGroup> _effects;
        private Dictionary<int, ShopPriceGroup> _prices;
        private Dictionary<ItemLicense, LicenseReward> _licenses;

        public IReadOnlyDictionary<ItemNumber, ShopItem> Items => _items;

        public IReadOnlyDictionary<int, ShopEffectGroup> Effects => _effects;

        public IReadOnlyDictionary<int, ShopPriceGroup> Prices => _prices;

        public IReadOnlyDictionary<ItemLicense, LicenseReward> Licenses => _licenses;

        public string Version { get; private set; }

        public void Load()
        {
            using (var db = GameDatabase.Open())
            {
                _effects = db.Find<ShopEffectGroupDto>(statement => statement
                        .Include<ShopEffectDto>(join => join.LeftOuterJoin()))
                    .ToArray()
                    .Select(dto => new ShopEffectGroup(dto))
                    .ToDictionary(x => x.Id);

                _prices = db.Find<ShopPriceGroupDto>(statement => statement
                        .Include<ShopPriceDto>(join => join.LeftOuterJoin()))
                    .ToArray()
                    .Select(dto => new ShopPriceGroup(dto))
                    .ToDictionary(x => x.Id);

                _items = db.Find<ShopItemDto>(statement => statement
                        .Include<ShopItemInfoDto>(join => join.LeftOuterJoin()))
                    .ToArray()
                    .Select(dto => new ShopItem(dto, this))
                    .ToDictionary(x => x.ItemNumber);

                _licenses = db.Find<LicenseRewardDto>()
                    .ToArray()
                    .Select(dto => new LicenseReward(dto, this))
                    .ToDictionary(x => x.ItemLicense);

                Version = db.Find<ShopVersionDto>().First().Version;
            }
        }

        public void Clear()
        {
            _items.Clear();
            _effects.Clear();
            _prices.Clear();
            _licenses.Clear();
            Version = "";
        }

        public ShopItem GetItem(ItemNumber itemNumber)
        {
            ShopItem shopItem;
            Items.TryGetValue(itemNumber, out shopItem);
            return shopItem;
        }

        public ShopItemInfo GetItemInfo(ItemNumber itemNumber, ItemPriceType priceType)
        {
            var item = GetItem(itemNumber);
            return item?.GetItemInfo(priceType);
        }
        public ShopItemInfo GetItemInfo(PlayerItem item)
        {
            return GetItemInfo(item.ItemNumber, item.PriceType);
        }

        public ShopPrice GetPrice(ItemNumber itemNumber, ItemPriceType priceType, ItemPeriodType periodType, ushort period)
        {
            var itemInfo = GetItemInfo(itemNumber, priceType);
            return itemInfo?.PriceGroup.GetPrice(periodType, period);
        }

        public ShopPrice GetPrice(PlayerItem item)
        {
            return GetPrice(item.ItemNumber, item.PriceType, item.PeriodType, item.Period);
        }
    }
}
