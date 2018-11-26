using System.Linq;
using Netsphere.Database.Game;
using Netsphere.Tools.ShopEditor.Services;
using Reactive.Bindings;
using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.Models
{
    public class ShopItemInfo : ReactiveObject
    {
        public int Id { get; }
        public ShopItem Item { get; }
        public ReactiveProperty<ShopPriceGroup> PriceGroup { get; }
        public ReactiveProperty<ShopEffectGroup> EffectGroup { get; }
        public ReactiveProperty<byte> DiscountPercentage { get; }
        public ReactiveProperty<bool> IsEnabled { get; }

        public ShopItemInfo(ShopItem item, ShopItemInfoEntity entity)
        {
            Id = entity.Id;
            Item = item;

            var priceGroup = ShopService.Instance.PriceGroups.First(x => x.Id == entity.PriceGroupId);
            PriceGroup = new ReactiveProperty<ShopPriceGroup>(priceGroup);

            var effectGroup = ShopService.Instance.EffectGroups.First(x => x.Id == entity.EffectGroupId);
            EffectGroup = new ReactiveProperty<ShopEffectGroup>(effectGroup);

            DiscountPercentage = new ReactiveProperty<byte>(entity.DiscountPercentage);
            IsEnabled = new ReactiveProperty<bool>(entity.IsEnabled);
        }
    }
}
