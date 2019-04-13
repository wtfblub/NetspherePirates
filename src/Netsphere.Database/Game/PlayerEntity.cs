using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("players")]
    public class PlayerEntity
    {
        [Key]
        public int Id { get; set; }

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

        public List<PlayerCharacterEntity> Characters { get; set; } = new List<PlayerCharacterEntity>();
        public List<PlayerDenyEntity> Ignores { get; set; } = new List<PlayerDenyEntity>();
        public List<PlayerItemEntity> Items { get; set; } = new List<PlayerItemEntity>();
        public List<PlayerMailEntity> Inbox { get; set; } = new List<PlayerMailEntity>();
        public List<PlayerSettingEntity> Settings { get; set; } = new List<PlayerSettingEntity>();
    }
}
