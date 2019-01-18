using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("shop_effects")]
    public class ShopEffectEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column]
        public int EffectGroupId { get; set; }
        public ShopEffectGroupEntity EffectGroup { get; set; }

        [Column]
        public uint Effect { get; set; }
    }
}
