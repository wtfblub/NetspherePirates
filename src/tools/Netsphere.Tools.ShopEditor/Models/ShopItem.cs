using System;
using System.Collections.Generic;
using System.Linq;
using Netsphere.Database.Game;
using Netsphere.Tools.ShopEditor.Services;
using Reactive.Bindings;
using ReactiveUI;
using ReactiveUI.Legacy;

namespace Netsphere.Tools.ShopEditor.Models
{
    public class ShopItem : ReactiveObject
    {
        private static readonly IEnumerable<Gender> s_genders =
            Enum.GetValues(typeof(Gender)).Cast<Gender>();

        private static readonly IEnumerable<ItemLicense> s_licenses =
            Enum.GetValues(typeof(ItemLicense)).Cast<ItemLicense>();

        public IEnumerable<Gender> Genders => s_genders;
        public IEnumerable<ItemLicense> Licenses => s_licenses;

        public Item Item { get; }
        public long ItemNumber { get; }
        public string DisplayName => $"{Item.Name} ({ItemNumber})";
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
            Item = ResourceService.Instance.Items.First(x => x.ItemNumber == entity.Id);
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
