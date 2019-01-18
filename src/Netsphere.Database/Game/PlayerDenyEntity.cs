using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("player_deny")]
    public class PlayerDenyEntity
    {
        [Key]
        public long Id { get; set; }

        [Column]
        public long PlayerId { get; set; }
        public PlayerEntity Player { get; set; }

        [Column]
        public long DenyPlayerId { get; set; }
        public PlayerEntity DenyPlayer { get; set; }
    }
}
