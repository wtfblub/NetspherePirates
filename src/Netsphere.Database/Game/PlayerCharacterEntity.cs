using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("player_characters")]
    public class PlayerCharacterEntity : Entity
    {
        [Column]
        public int PlayerId { get; set; }

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
        public int? Weapon1Id { get; set; }

        [Column]
        public int? Weapon2Id { get; set; }

        [Column]
        public int? Weapon3Id { get; set; }

        [Column]
        public int? SkillId { get; set; }

        [Column]
        public int? HairId { get; set; }

        [Column]
        public int? FaceId { get; set; }

        [Column]
        public int? ShirtId { get; set; }

        [Column]
        public int? PantsId { get; set; }

        [Column]
        public int? GlovesId { get; set; }

        [Column]
        public int? ShoesId { get; set; }

        [Column]
        public int? AccessoryId { get; set; }

        [Association(CanBeNull = true, ThisKey = "PlayerId", OtherKey = "Id")]
        public PlayerEntity Player { get; set; }
    }
}
