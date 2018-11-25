using System;
using System.Collections.Generic;
using System.Linq;
using Netsphere.Database.Game;
using Reactive.Bindings;
using ReactiveUI;
using ReactiveUI.Legacy;

namespace Netsphere.Tools.ShopEditor.Models
{
    public class ShopPriceGroup : ReactiveObject
    {
        private static readonly IEnumerable<ItemPriceType> s_priceTypes =
            Enum.GetValues(typeof(ItemPriceType)).Cast<ItemPriceType>();

        public int Id { get; }
        public IEnumerable<ItemPriceType> PriceTypes => s_priceTypes;
        public ReactiveProperty<string> Name { get; }
        public ReactiveProperty<ItemPriceType> PriceType { get; }
        public IReactiveList<ShopPrice> Prices { get; }

        public ShopPriceGroup(ShopPriceGroupEntity entity)
        {
            Id = entity.Id;
            Name = new ReactiveProperty<string>(entity.Name);
            PriceType = new ReactiveProperty<ItemPriceType>((ItemPriceType)entity.PriceType);

            var prices = entity.ShopPrices.Select(priceEntity => new ShopPrice(this, priceEntity));
            Prices = new ReactiveList<ShopPrice>(prices);
        }
    }
}
