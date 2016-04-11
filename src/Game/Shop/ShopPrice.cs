using System.Collections.Generic;
using System.Linq;
using Netsphere.Database.Game;

namespace Netsphere.Shop
{
    internal class ShopPriceGroup
    {
        public int Id { get; set; }
        public ItemPriceType PriceType { get; set; }
        public string Name { get; set; }
        public IList<ShopPrice> Prices { get; set; }

        public ShopPriceGroup(ShopPriceGroupDto dto)
        {
            Id = dto.Id;
            PriceType = (ItemPriceType) dto.PriceType;
            Name = dto.Name;
            Prices = dto.ShopPrices.Select(p => new ShopPrice(p)).ToList();
        }

        public ShopPrice GetPrice(int id)
        {
            return Prices.FirstOrDefault(p => p.Id == id);
        }

        public ShopPrice GetPrice(ItemPeriodType periodType, ushort period)
        {
            return Prices.FirstOrDefault(p => p.PeriodType == periodType && p.Period == period);
        }
    }

    internal class ShopPrice
    {
        public int Id { get; set; }
        public ItemPeriodType PeriodType { get; set; }
        public ushort Period { get; set; }
        public int Price { get; set; }
        public bool CanRefund { get; set; }
        public int Durability { get; set; }
        public bool IsEnabled { get; set; }

        public ShopPrice(ShopPriceDto dto)
        {
            Id = dto.Id;
            PeriodType = (ItemPeriodType) dto.PeriodType;
            Period = (ushort) dto.Period;
            Price = dto.Price;
            CanRefund = dto.IsRefundable;
            Durability = dto.Durability;
            IsEnabled = dto.IsEnabled;
        }
    }
}
