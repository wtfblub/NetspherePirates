using System.Linq;
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
            ItemNumber = shopResources.Items.Values.First(item => item.GetItemInfo(dto.ShopItemInfoId) != null).ItemNumber;
            ShopItemInfo = shopResources.Items[ItemNumber].GetItemInfo(dto.ShopItemInfoId);
            ShopPrice = ShopItemInfo.PriceGroup.GetPrice(dto.ShopPriceId);
            Color = dto.Color;
        }
    }
}
