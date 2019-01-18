using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("shop_price_groups")]
    public class ShopPriceGroupEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column]
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Column]
        public byte PriceType { get; set; }

        public List<ShopPriceEntity> ShopPrices { get; set; } = new List<ShopPriceEntity>();
    }
}
