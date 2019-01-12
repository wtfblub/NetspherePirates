using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("player_deny")]
    public class PlayerDenyEntity
    {
        [PrimaryKey]
        public long Id { get; set; }

        [Column]
        public long PlayerId { get; set; }

        [Column]
        public long DenyPlayerId { get; set; }

        [Association(CanBeNull = true, ThisKey = "PlayerId", OtherKey = "Id")]
        public PlayerEntity Player { get; set; }

        [Association(CanBeNull = true, ThisKey = "DenyPlayerId", OtherKey = "Id")]
        public PlayerEntity DenyPlayer { get; set; }
    }
}
