using System.Collections.Generic;
using System.Linq;
using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("shop_price_groups")]
    public class ShopPriceGroupEntity : Entity
    {
        [Column(CanBeNull = false)]
        public string Name { get; set; }

        [Column]
        public byte PriceType { get; set; }

        [Association(CanBeNull = true, ThisKey = "Id", OtherKey = "PriceGroupId")]
        public IEnumerable<ShopPriceEntity> ShopPrices { get; set; } = Enumerable.Empty<ShopPriceEntity>();
    }
}
