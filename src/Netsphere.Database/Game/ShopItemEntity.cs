using System.Collections.Generic;
using System.Linq;
using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("shop_items")]
    public class ShopItemEntity
    {
        [PrimaryKey]
        public long Id { get; set; }

        [Column]
        public byte RequiredGender { get; set; }

        [Column]
        public byte RequiredLicense { get; set; }

        [Column]
        public byte Colors { get; set; }

        [Column]
        public byte UniqueColors { get; set; }

        [Column]
        public byte RequiredLevel { get; set; }

        [Column]
        public byte LevelLimit { get; set; }

        [Column]
        public byte RequiredMasterLevel { get; set; }

        [Column]
        public bool IsOneTimeUse { get; set; }

        [Column]
        public bool IsDestroyable { get; set; }

        [Association(CanBeNull = true, ThisKey = "Id", OtherKey = "ShopItemId")]
        public IEnumerable<ShopItemInfoEntity> ItemInfos { get; set; } = Enumerable.Empty<ShopItemInfoEntity>();
    }
}
