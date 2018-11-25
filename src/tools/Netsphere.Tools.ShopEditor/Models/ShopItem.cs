using System.Linq;
using Netsphere.Database.Game;
using Reactive.Bindings;
using ReactiveUI;
using ReactiveUI.Legacy;

namespace Netsphere.Tools.ShopEditor.Models
{
    public class ShopItem : ReactiveObject
    {
        public long ItemNumber { get; }
        public ReactiveProperty<Gender> RequiredGender { get; }
        public ReactiveProperty<ItemLicense> RequiredLicense { get; }
        public ReactiveProperty<byte> Colors { get; }
        public ReactiveProperty<byte> UniqueColors { get; }
        public ReactiveProperty<byte> RequiredLevel { get; }
        public ReactiveProperty<byte> LevelLimit { get; }
        public ReactiveProperty<byte> RequiredMasterLevel { get; }
        public ReactiveProperty<bool> IsOneTimeUse { get; }
        public ReactiveProperty<bool> IsDestroyable { get; }
        public IReactiveList<ShopItemInfo> ItemInfos { get; }

        public ShopItem(ShopItemEntity entity)
        {
            ItemNumber = entity.Id;
            RequiredGender = new ReactiveProperty<Gender>((Gender)entity.RequiredGender);
            RequiredLicense = new ReactiveProperty<ItemLicense>((ItemLicense)entity.RequiredLicense);
            Colors = new ReactiveProperty<byte>(entity.Colors);
            UniqueColors = new ReactiveProperty<byte>(entity.UniqueColors);
            RequiredLevel = new ReactiveProperty<byte>(entity.RequiredLevel);
            LevelLimit = new ReactiveProperty<byte>(entity.LevelLimit);
            RequiredMasterLevel = new ReactiveProperty<byte>(entity.RequiredMasterLevel);
            IsOneTimeUse = new ReactiveProperty<bool>(entity.IsOneTimeUse);
            IsDestroyable = new ReactiveProperty<bool>(entity.IsDestroyable);
            ItemInfos = new ReactiveList<ShopItemInfo>(entity.ItemInfos.Select(x => new ShopItemInfo(this, x)));
        }
    }
}
