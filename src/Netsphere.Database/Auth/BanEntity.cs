using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Auth
{
    [Table("bans")]
    public class BanEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column]
        public long AccountId { get; set; }
        public AccountEntity Account { get; set; }

        [Column]
        public long Date { get; set; }

        [Column]
        public long? Duration { get; set; }

        [Column]
        public string Reason { get; set; }
    }
}
