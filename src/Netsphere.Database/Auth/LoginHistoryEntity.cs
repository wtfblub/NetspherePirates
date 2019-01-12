using LinqToDB.Mapping;

namespace Netsphere.Database.Auth
{
    [Table("login_history")]
    public class LoginHistoryEntity
    {
        [PrimaryKey]
        [Identity]
        public int Id { get; set; }

        [Column]
        public long AccountId { get; set; }

        [Column]
        public long Date { get; set; }

        [Column]
        public string IP { get; set; }

        [Association(CanBeNull = false, ThisKey = "AccountId", OtherKey = "Id")]
        public AccountEntity Account { get; set; }
    }
}
