using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Auth
{
    [Table("accounts")]
    public class AccountEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column]
        [Required]
        [MaxLength(40)]
        public string Username { get; set; }

        [Column]
        [MaxLength(40)]
        public string Nickname { get; set; }

        [Column]
        [MaxLength(40)]
        public string Password { get; set; }

        [Column]
        [MaxLength(40)]
        public string Salt { get; set; }

        [Column]
        public byte SecurityLevel { get; set; }

        public List<BanEntity> Bans { get; set; } = new List<BanEntity>();
        public List<LoginHistoryEntity> LoginHistory { get; set; } = new List<LoginHistoryEntity>();
        public List<NicknameHistoryEntity> NicknameHistory { get; set; } = new List<NicknameHistoryEntity>();
    }
}
