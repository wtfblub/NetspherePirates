using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("shop_iteminfos")]
    public class ShopItemInfoEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column]
        public long ShopItemId { get; set; }
        public ShopItemEntity ShopItem { get; set; }

        [Column]
        public int PriceGroupId { get; set; }
        public ShopPriceGroupEntity PriceGroup { get; set; }

        [Column]
        public int EffectGroupId { get; set; }
        public ShopEffectGroupEntity EffectGroup { get; set; }

        [Column]
        public byte DiscountPercentage { get; set; }

        [Column]
        public bool IsEnabled { get; set; }
    }
}
