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

        public IEnumerable<Gender> Genders => s_genders;

        public Item Item { get; }
        public long ItemNumber { get; }
        public string DisplayName => $"{Item.Name} ({ItemNumber})";
        public ReactiveProperty<Gender> RequiredGender { get; }
        public ReactiveProperty<byte> Colors { get; }
        public ReactiveProperty<byte> UniqueColors { get; }
        public ReactiveProperty<byte> RequiredLevel { get; }
        public ReactiveProperty<byte> LevelLimit { get; }
        public ReactiveProperty<byte> RequiredMasterLevel { get; }
        public ReactiveProperty<bool> IsOneTimeUse { get; }
        public ReactiveProperty<bool> IsDestroyable { get; }
        public ReactiveProperty<byte> MainTab { get; set; }
        public ReactiveProperty<byte> SubTab { get; set; }
        public IReactiveList<ShopItemInfo> ItemInfos { get; }

        public ShopItem(ShopItemEntity entity)
        {
            Item = ResourceService.Instance.Items.First(x => x.ItemNumber == entity.Id);
            ItemNumber = entity.Id;
            RequiredGender = new ReactiveProperty<Gender>((Gender)entity.RequiredGender);
            Colors = new ReactiveProperty<byte>(entity.Colors);
            UniqueColors = new ReactiveProperty<byte>(entity.UniqueColors);
            RequiredLevel = new ReactiveProperty<byte>(entity.RequiredLevel);
            LevelLimit = new ReactiveProperty<byte>(entity.LevelLimit);
            RequiredMasterLevel = new ReactiveProperty<byte>(entity.RequiredMasterLevel);
            IsOneTimeUse = new ReactiveProperty<bool>(entity.IsOneTimeUse);
            IsDestroyable = new ReactiveProperty<bool>(entity.IsDestroyable);
            MainTab = new ReactiveProperty<byte>(entity.MainTab);
            SubTab = new ReactiveProperty<byte>(entity.SubTab);
            ItemInfos = new ReactiveList<ShopItemInfo>(entity.ItemInfos.Select(x => new ShopItemInfo(this, x)));
        }
    }
}
