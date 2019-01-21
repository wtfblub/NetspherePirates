using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("player_mails")]
    public class PlayerMailEntity
    {
        [Key]
        public long Id { get; set; }

        [Column]
        public int PlayerId { get; set; }
        public PlayerEntity Player { get; set; }

        [Column]
        public int SenderPlayerId { get; set; }
        public PlayerEntity SenderPlayer { get; set; }

        [Column]
        public long SentDate { get; set; }

        [Column]
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Column]
        [Required]
        [MaxLength(500)]
        public string Message { get; set; }

        [Column]
        public bool IsMailNew { get; set; }

        [Column]
        public bool IsMailDeleted { get; set; }
    }
}
