using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("player_settings")]
    public class PlayerSettingEntity
    {
        [PrimaryKey]
        public long Id { get; set; }

        [Column]
        public int PlayerId { get; set; }

        [Column(CanBeNull = false)]
        public string Setting { get; set; }

        [Column(CanBeNull = false)]
        public string Value { get; set; }

        [Association(CanBeNull = true, ThisKey = "PlayerId", OtherKey = "Id")]
        public PlayerEntity Player { get; set; }
    }
}
