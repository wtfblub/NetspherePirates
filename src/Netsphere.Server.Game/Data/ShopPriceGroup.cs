using System.Collections.Generic;
using System.Linq;
using Netsphere.Database.Game;

namespace Netsphere.Server.Game.Data
{
    public class ShopPriceGroup
    {
        public int Id { get; set; }
        public ItemPriceType PriceType { get; set; }
        public string Name { get; set; }
        public IList<ShopPrice> Prices { get; set; }

        public ShopPriceGroup(ShopPriceGroupEntity entity)
        {
            Id = entity.Id;
            PriceType = (ItemPriceType)entity.PriceType;
            Name = entity.Name;
            Prices = entity.ShopPrices.Select(x => new ShopPrice(x)).ToList();
        }

        public ShopPrice GetPrice(int id)
        {
            return Prices.FirstOrDefault(x => x.Id == id);
        }

        public ShopPrice GetPrice(ItemPeriodType periodType, ushort period)
        {
            return Prices.FirstOrDefault(x => x.PeriodType == periodType && x.Period == period);
        }
    }
}
