using System.Collections.Generic;
using System.Linq;
using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("shop_effect_groups")]
    public class ShopEffectGroupEntity
    {
        [PrimaryKey]
        [Identity]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public string Name { get; set; }

        [Association(CanBeNull = true, ThisKey = "Id", OtherKey = "EffectGroupId")]
        public IEnumerable<ShopEffectEntity> ShopEffects { get; set; } = Enumerable.Empty<ShopEffectEntity>();
    }
}
