using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("shop_effects")]
    public class ShopEffectEntity
    {
        [PrimaryKey]
        [Identity]
        public int Id { get; set; }

        [Column]
        public int EffectGroupId { get; set; }

        [Column]
        public uint Effect { get; set; }

        [Association(CanBeNull = true, ThisKey = "EffectGroupId", OtherKey = "Id")]
        public ShopEffectGroupEntity EffectGroup { get; set; }
    }
}
