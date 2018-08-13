using LinqToDB.Mapping;

namespace Netsphere.Database.Auth
{
    [Table("bans")]
    public class BanEntity : Entity
    {
        [Column]
        public int AccountId { get; set; }

        [Column]
        public long Date { get; set; }

        [Column]
        public long? Duration { get; set; }

        [Column]
        public string Reason { get; set; }

        [Association(CanBeNull = false, ThisKey = "AccountId", OtherKey = "Id")]
        public AccountEntity Account { get; set; }
    }
}
