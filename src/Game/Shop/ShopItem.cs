using System.Collections.Generic;
using System.Linq;
using Netsphere.Database.Game;
using Netsphere.Resource;

namespace Netsphere.Shop
{
    internal class ShopItem
    {
        public ItemNumber ItemNumber { get; set; }
        public Gender Gender { get; set; }
        public ItemLicense License { get; set; }
        public int ColorGroup { get; set; }
        public int UniqueColorGroup { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public int MasterLevel { get; set; }
        //public int RepairCost { get; set; }
        public bool IsOneTimeUse { get; set; }
        public bool IsDestroyable { get; set; }
        public IList<ShopItemInfo> ItemInfos { get; set; }

        public ShopItem(ShopItemDto dto, ShopResources shopResources)
        {
            ItemNumber = dto.Id;
            Gender = (Gender) dto.RequiredGender;
            License = (ItemLicense) dto.RequiredLicense;
            ColorGroup = dto.Colors;
            UniqueColorGroup = dto.UniqueColors;
            MinLevel = dto.RequiredLevel;
            MaxLevel = dto.LevelLimit;
            MasterLevel = dto.RequiredMasterLevel;
            //RepairCost = dto.repair_cost;
            IsOneTimeUse = dto.IsOneTimeUse;
            IsDestroyable = dto.IsDestroyable;
            ItemInfos = dto.ItemInfos.Select(i => new ShopItemInfo(this, i, shopResources)).ToList();
        }

        public ShopItemInfo GetItemInfo(int id)
        {
            return ItemInfos.FirstOrDefault(i => i.Id == id);
        }

        public ShopItemInfo GetItemInfo(ItemPriceType priceType)
        {
            return ItemInfos.FirstOrDefault(i => i.PriceGroup.PriceType == priceType);
        }
    }

    internal class ShopItemInfo
    {
        public int Id { get; set; }
        public ShopPriceGroup PriceGroup { get; set; }
        public ShopEffectGroup EffectGroup { get; set; }
        public bool IsEnabled { get; set; }
        public int Discount { get; set; }

        public ShopItem ShopItem { get; private set; }

        public ShopItemInfo(ShopItem shopItem, ShopItemInfoDto dto, ShopResources shopResources)
        {
            Id = dto.Id;
            PriceGroup = shopResources.Prices[dto.PriceGroup.Id];
            EffectGroup = shopResources.Effects[dto.EffectGroup.Id];
            IsEnabled = dto.IsEnabled;
            Discount = dto.DiscountPercentage;

            ShopItem = shopItem;
        }
    }
}
