using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("player_characters")]
    public class PlayerCharacterEntity
    {
        [Key]
        public long Id { get; set; }

        [Column]
        public int PlayerId { get; set; }
        public PlayerEntity Player { get; set; }

        [Column]
        public byte Slot { get; set; }

        [Column]
        public byte Gender { get; set; }

        [Column]
        public byte BasicHair { get; set; }

        [Column]
        public byte BasicFace { get; set; }

        [Column]
        public byte BasicShirt { get; set; }

        [Column]
        public byte BasicPants { get; set; }

        [Column]
        public long? Weapon1Id { get; set; }
        public PlayerItemEntity Weapon1 { get; set; }

        [Column]
        public long? Weapon2Id { get; set; }
        public PlayerItemEntity Weapon2 { get; set; }

        [Column]
        public long? Weapon3Id { get; set; }
        public PlayerItemEntity Weapon3 { get; set; }

        [Column]
        public long? SkillId { get; set; }
        public PlayerItemEntity Skill { get; set; }

        [Column]
        public long? HairId { get; set; }
        public PlayerItemEntity Hair { get; set; }

        [Column]
        public long? FaceId { get; set; }
        public PlayerItemEntity Face { get; set; }

        [Column]
        public long? ShirtId { get; set; }
        public PlayerItemEntity Shirt { get; set; }

        [Column]
        public long? PantsId { get; set; }
        public PlayerItemEntity Pants { get; set; }

        [Column]
        public long? GlovesId { get; set; }
        public PlayerItemEntity Gloves { get; set; }

        [Column]
        public long? ShoesId { get; set; }
        public PlayerItemEntity Shoes { get; set; }

        [Column]
        public long? AccessoryId { get; set; }
        public PlayerItemEntity Accessory { get; set; }
    }
}
