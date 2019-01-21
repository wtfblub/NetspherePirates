using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("player_licenses")]
    public class PlayerLicenseEntity
    {
        [Key]
        public long Id { get; set; }

        [Column]
        public int PlayerId { get; set; }
        public PlayerEntity Player { get; set; }

        [Column]
        public byte License { get; set; }

        [Column]
        public long FirstCompletedDate { get; set; }

        [Column]
        public int CompletedCount { get; set; }
    }
}
