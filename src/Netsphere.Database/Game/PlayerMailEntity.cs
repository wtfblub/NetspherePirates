using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("player_mails")]
    public class PlayerMailEntity
    {
        [PrimaryKey]
        public long Id { get; set; }

        [Column]
        public long PlayerId { get; set; }

        [Column]
        public long SenderPlayerId { get; set; }

        [Column]
        public long SentDate { get; set; }

        [Column(CanBeNull = false)]
        public string Title { get; set; }

        [Column(CanBeNull = false)]
        public string Message { get; set; }

        [Column]
        public bool IsMailNew { get; set; }

        [Column]
        public bool IsMailDeleted { get; set; }

        [Association(CanBeNull = true, ThisKey = "PlayerId", OtherKey = "Id")]
        public PlayerEntity Player { get; set; }

        [Association(CanBeNull = true, ThisKey = "SenderPlayerId", OtherKey = "Id")]
        public PlayerEntity SenderPlayer { get; set; }
    }
}
