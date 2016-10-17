using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Auth
{
    [Table("accounts")]
    public class AccountDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public byte SecurityLevel { get; set; }

        public IEnumerable<BanDto> Bans { get; set; }
        public IEnumerable<LoginHistoryDto> LoginHistory { get; set; }
        public IEnumerable<NicknameHistoryDto> NicknameHistory { get; set; }
    }

    [Table("bans")]
    public class BanDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Account))]
        public int AccountId { get; set; }
        public AccountDto Account { get; set; }

        public long Date { get; set; }
        public long? Duration { get; set; }
        public string Reason { get; set; }
    }

    [Table("login_history")]
    public class LoginHistoryDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Account))]
        public int AccountId { get; set; }
        public AccountDto Account { get; set; }

        public long Date { get; set; }
        public string IP { get; set; }
    }

    [Table("nickname_history")]
    public class NicknameHistoryDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Account))]
        public int AccountId { get; set; }
        public AccountDto Account { get; set; }

        public string Nickname { get; set; }
        public long? ExpireDate { get; set; }
    }
}
