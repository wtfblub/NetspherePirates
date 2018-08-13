using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("shop_effects")]
    public class ShopEffectEntity : Entity
    {
        [Column]
        public int EffectGroupId { get; set; }

        [Column]
        public uint Effect { get; set; }

        [Association(CanBeNull = true, ThisKey = "EffectGroupId", OtherKey = "Id")]
        public ShopEffectGroupEntity EffectGroup { get; set; }
    }
}
