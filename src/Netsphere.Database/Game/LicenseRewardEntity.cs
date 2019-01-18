using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("license_rewards")]
    public class LicenseRewardEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column]
        public int ShopItemInfoId { get; set; }
        public ShopItemInfoEntity ShopItemInfo { get; set; }

        [Column]
        public int ShopPriceId { get; set; }
        public ShopPriceEntity ShopPrice { get; set; }

        [Column]
        public byte Color { get; set; }
    }
}
