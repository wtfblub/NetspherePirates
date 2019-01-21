using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("player_settings")]
    public class PlayerSettingEntity
    {
        [Key]
        public long Id { get; set; }

        [Column]
        public int PlayerId { get; set; }
        public PlayerEntity Player { get; set; }

        [Column]
        [Required]
        [MaxLength(100)]
        public string Setting { get; set; }

        [Column]
        [Required]
        [MaxLength(512)]
        public string Value { get; set; }
    }
}
