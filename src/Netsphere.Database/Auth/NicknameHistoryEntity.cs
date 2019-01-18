using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Auth
{
    [Table("nickname_history")]
    public class NicknameHistoryEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column]
        public long AccountId { get; set; }
        public AccountEntity Account { get; set; }

        [Column]
        [Required]
        [MaxLength(40)]
        public string Nickname { get; set; }

        [Column]
        public long? ExpireDate { get; set; }
    }
}
