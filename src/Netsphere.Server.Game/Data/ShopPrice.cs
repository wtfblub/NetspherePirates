using Netsphere.Database.Game;

namespace Netsphere.Server.Game.Data
{
    public class ShopPrice
    {
        public int Id { get; set; }
        public ItemPeriodType PeriodType { get; set; }
        public ushort Period { get; set; }
        public int Price { get; set; }
        public bool CanRefund { get; set; }
        public int Durability { get; set; }
        public bool IsEnabled { get; set; }

        public ShopPrice(ShopPriceEntity entity)
        {
            Id = entity.Id;
            PeriodType = (ItemPeriodType)entity.PeriodType;
            Period = (ushort)entity.Period;
            Price = entity.Price;
            CanRefund = entity.IsRefundable;
            Durability = entity.Durability;
            IsEnabled = entity.IsEnabled;
        }
    }
}
