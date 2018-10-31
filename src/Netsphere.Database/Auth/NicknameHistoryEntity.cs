using LinqToDB.Mapping;

namespace Netsphere.Database.Auth
{
    [Table("nickname_history")]
    public class NicknameHistoryEntity
    {
        [PrimaryKey]
        [Identity]
        public int Id { get; set; }

        [Column]
        public int AccountId { get; set; }

        [Column]
        public string Nickname { get; set; }

        [Column]
        public long? ExpireDate { get; set; }

        [Association(CanBeNull = false, ThisKey = "AccountId", OtherKey = "Id")]
        public AccountEntity Account { get; set; }
    }
}
