using System.Collections.Generic;
using System.Linq;
using LinqToDB.Mapping;

namespace Netsphere.Database.Auth
{
    [Table("accounts")]
    public class AccountEntity
    {
        [PrimaryKey]
        [Identity]
        public long Id { get; set; }

        [Column(CanBeNull = false)]
        public string Username { get; set; }

        [Column(CanBeNull = true)]
        public string Nickname { get; set; }

        [Column]
        public string Password { get; set; }

        [Column]
        public string Salt { get; set; }

        [Column]
        public byte SecurityLevel { get; set; }

        [Association(CanBeNull = true, ThisKey = "Id", OtherKey = "AccountId")]
        public IEnumerable<BanEntity> Bans { get; set; } = Enumerable.Empty<BanEntity>();

        [Association(CanBeNull = true, ThisKey = "Id", OtherKey = "AccountId")]
        public IEnumerable<LoginHistoryEntity> LoginHistory { get; set; } = Enumerable.Empty<LoginHistoryEntity>();

        [Association(CanBeNull = true, ThisKey = "Id", OtherKey = "AccountId")]
        public IEnumerable<NicknameHistoryEntity> NicknameHistory { get; set; } = Enumerable.Empty<NicknameHistoryEntity>();
    }
}
