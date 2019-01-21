using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Auth
{
    [Table("login_history")]
    public class LoginHistoryEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column]
        public int AccountId { get; set; }
        public AccountEntity Account { get; set; }

        [Column]
        public long Date { get; set; }

        [Column]
        [Required]
        [MaxLength(15)]
        public string IP { get; set; }
    }
}
