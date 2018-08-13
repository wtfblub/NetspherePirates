using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("shop_prices")]
    public class ShopPriceEntity : Entity
    {
        [Column]
        public int PriceGroupId { get; set; }

        [Column]
        public byte PeriodType { get; set; }

        [Column]
        public int Period { get; set; }

        [Column]
        public int Price { get; set; }

        [Column]
        public bool IsRefundable { get; set; }

        [Column]
        public int Durability { get; set; }

        [Column]
        public bool IsEnabled { get; set; }

        [Association(CanBeNull = true, ThisKey = "PriceGroupId", OtherKey = "Id")]
        public ShopPriceGroupEntity PriceGroup { get; set; }
    }
}
