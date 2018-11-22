using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("player_characters")]
    public class PlayerCharacterEntity
    {
        [PrimaryKey]
        public long Id { get; set; }

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
        public long? Weapon1Id { get; set; }

        [Column]
        public long? Weapon2Id { get; set; }

        [Column]
        public long? Weapon3Id { get; set; }

        [Column]
        public long? SkillId { get; set; }

        [Column]
        public long? HairId { get; set; }

        [Column]
        public long? FaceId { get; set; }

        [Column]
        public long? ShirtId { get; set; }

        [Column]
        public long? PantsId { get; set; }

        [Column]
        public long? GlovesId { get; set; }

        [Column]
        public long? ShoesId { get; set; }

        [Column]
        public long? AccessoryId { get; set; }

        [Association(CanBeNull = true, ThisKey = "PlayerId", OtherKey = "Id")]
        public PlayerEntity Player { get; set; }
    }
}
