using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("shop_prices")]
    public class ShopPriceEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column]
        public int PriceGroupId { get; set; }
        public ShopPriceGroupEntity PriceGroup { get; set; }

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
    }
}
