using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("shop_effect_groups")]
    public class ShopEffectGroupEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column]
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Column]
        public uint PreviewEffect { get; set; }

        public List<ShopEffectEntity> ShopEffects { get; set; } = new List<ShopEffectEntity>();
    }
}
