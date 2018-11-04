using System.Linq;
using Netsphere.Database.Game;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game.Data
{
    public class LicenseReward
    {
        public ItemLicense ItemLicense { get; set; }
        public ItemNumber ItemNumber { get; set; }
        public ShopItemInfo ShopItemInfo { get; set; }
        public ShopPrice ShopPrice { get; set; }
        public byte Color { get; set; }

        public LicenseReward(LicenseRewardEntity entity, GameDataService gameDataService)
        {
            ItemLicense = (ItemLicense)entity.Id;
            ItemNumber = gameDataService.ShopItems.Values.First(x => x.GetItemInfo(entity.ShopItemInfoId) != null).ItemNumber;
            ShopItemInfo = gameDataService.ShopItems[ItemNumber].GetItemInfo(entity.ShopItemInfoId);
            ShopPrice = ShopItemInfo.PriceGroup.GetPrice(entity.ShopPriceId);
            Color = entity.Color;
        }
    }
}
