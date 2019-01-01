using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Netsphere.Database.Game;
using Reactive.Bindings;
using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.Models
{
    public class ShopPrice : ReactiveObject
    {
        private static readonly IEnumerable<ItemPeriodType> s_periodTypes =
            Enum.GetValues(typeof(ItemPeriodType)).Cast<ItemPeriodType>();

        public int Id { get; }
        public IEnumerable<ItemPeriodType> PeriodTypes => s_periodTypes;
        public ShopPriceGroup PriceGroup { get; }
        public ReactiveProperty<ItemPeriodType> PeriodType { get; }
        public ReactiveProperty<uint> Period { get; }
        public ReactiveProperty<int> Price { get; }
        public ReactiveProperty<bool> IsRefundable { get; }
        public ReactiveProperty<int> Durability { get; }
        public ReactiveProperty<bool> IsEnabled { get; }

        public ShopPrice(ShopPriceGroup priceGroup, ShopPriceEntity entity)
        {
            Id = entity.Id;
            PriceGroup = priceGroup;
            PeriodType = new ReactiveProperty<ItemPeriodType>((ItemPeriodType)entity.PeriodType);
            Period = new ReactiveProperty<uint>((uint)entity.Period);
            Price = new ReactiveProperty<int>(entity.Price);
            IsRefundable = new ReactiveProperty<bool>(entity.IsRefundable);
            Durability = new ReactiveProperty<int>(entity.Durability);
            IsEnabled = new ReactiveProperty<bool>(entity.IsEnabled);

            this.WhenAnyValue(x => x.PeriodType.Value)
                .Select(x =>
                {
                    bool hasDurability;
                    switch (x)
                    {
                        case ItemPeriodType.Hours:
                        case ItemPeriodType.Days:
                        case ItemPeriodType.Units:
                            hasDurability = false;
                            break;

                        default:
                            hasDurability = true;
                            break;
                    }

                    if (hasDurability && Durability.Value < 0)
                        return 0;

                    if (!hasDurability)
                        return -1;

                    return Durability.Value;
                })
                .BindTo(this, x => x.Durability.Value);

            this.WhenAnyValue(x => x.PeriodType.Value)
                .Select(x =>
                {
                    bool hasPeriod;
                    switch (x)
                    {
                        case ItemPeriodType.Days:
                        case ItemPeriodType.Hours:
                            hasPeriod = true;
                            break;

                        default:
                            hasPeriod = false;
                            break;
                    }

                    return hasPeriod ? Period.Value : 0;
                })
                .BindTo(this, x => x.Period.Value);
        }
    }
}
