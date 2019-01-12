using System.Collections.Generic;
using System.Linq;
using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("players")]
    public class PlayerEntity
    {
        [PrimaryKey]
        public long Id { get; set; }

        [Column]
        public byte TutorialState { get; set; }

        [Column]
        public int TotalExperience { get; set; }

        [Column]
        public int PEN { get; set; }

        [Column]
        public int AP { get; set; }

        [Column]
        public int Coins1 { get; set; }

        [Column]
        public int Coins2 { get; set; }

        [Column]
        public byte CurrentCharacterSlot { get; set; }

        [Association(CanBeNull = true, ThisKey = "Id", OtherKey = "PlayerId")]
        public IEnumerable<PlayerCharacterEntity> Characters { get; set; } = Enumerable.Empty<PlayerCharacterEntity>();

        [Association(CanBeNull = true, ThisKey = "Id", OtherKey = "PlayerId")]
        public IEnumerable<PlayerDenyEntity> Ignores { get; set; } = Enumerable.Empty<PlayerDenyEntity>();

        [Association(CanBeNull = true, ThisKey = "Id", OtherKey = "PlayerId")]
        public IEnumerable<PlayerItemEntity> Items { get; set; } = Enumerable.Empty<PlayerItemEntity>();

        [Association(CanBeNull = true, ThisKey = "Id", OtherKey = "PlayerId")]
        public IEnumerable<PlayerLicenseEntity> Licenses { get; set; } = Enumerable.Empty<PlayerLicenseEntity>();

        [Association(CanBeNull = true, ThisKey = "Id", OtherKey = "PlayerId")]
        public IEnumerable<PlayerMailEntity> Inbox { get; set; } = Enumerable.Empty<PlayerMailEntity>();

        [Association(CanBeNull = true, ThisKey = "Id", OtherKey = "PlayerId")]
        public IEnumerable<PlayerSettingEntity> Settings { get; set; } = Enumerable.Empty<PlayerSettingEntity>();
    }
}
