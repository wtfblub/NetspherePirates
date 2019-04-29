using System.Collections.Generic;
using System.Linq;
using Netsphere.Database.Game;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game.Data
{
    public class ShopItem
    {
        public ItemNumber ItemNumber { get; set; }
        public Gender Gender { get; set; }
        public ItemLicense License { get; set; }
        public int ColorGroup { get; set; }
        public int UniqueColorGroup { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public int MasterLevel { get; set; }
        public bool IsOneTimeUse { get; set; }
        public bool IsDestroyable { get; set; }
        public byte MainTab { get; set; }
        public byte SubTab { get; set; }
        public IList<ShopItemInfo> ItemInfos { get; set; }

        public ShopItem(ShopItemEntity entity, GameDataService gameDataService)
        {
            ItemNumber = entity.Id;
            Gender = (Gender)entity.RequiredGender;
            License = (ItemLicense)entity.RequiredLicense;
            ColorGroup = entity.Colors;
            UniqueColorGroup = entity.UniqueColors;
            MinLevel = entity.RequiredLevel;
            MaxLevel = entity.LevelLimit;
            MasterLevel = entity.RequiredMasterLevel;
            IsOneTimeUse = entity.IsOneTimeUse;
            IsDestroyable = entity.IsDestroyable;
            MainTab = entity.MainTab;
            SubTab = entity.SubTab;
            ItemInfos = entity.ItemInfos.Select(x => new ShopItemInfo(this, x, gameDataService)).ToList();
        }

        public ShopItemInfo GetItemInfo(int id)
        {
            return ItemInfos.FirstOrDefault(x => x.Id == id);
        }

        public ShopItemInfo GetItemInfo(ItemPriceType priceType)
        {
            return ItemInfos.FirstOrDefault(x => x.PriceGroup.PriceType == priceType);
        }
    }
}
