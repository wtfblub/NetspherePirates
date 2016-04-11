using Netsphere.Database.Game;
using Netsphere.Resource;

namespace Netsphere.Shop
{
    internal class LicenseReward
    {
        public ItemLicense ItemLicense { get; set; }
        public ItemNumber ItemNumber { get; set; }
        public ShopItemInfo ShopItemInfo { get; set; }
        public ShopPrice ShopPrice { get; set; }
        public byte Color { get; set; }

        public LicenseReward(LicenseRewardDto dto, ShopResources shopResources)
        {
            ItemLicense = (ItemLicense)dto.Id;
            ItemNumber = dto.ShopItemInfo.ShopItem.Id;
            ShopItemInfo = shopResources.Items[ItemNumber].GetItemInfo(dto.ShopItemInfo.Id);
            ShopPrice = ShopItemInfo.PriceGroup.GetPrice(dto.ShopPrice.Id);
            Color = dto.Color;
        }
    }
}
