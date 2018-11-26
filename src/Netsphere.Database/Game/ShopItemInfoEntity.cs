using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("shop_iteminfos")]
    public class ShopItemInfoEntity
    {
        [PrimaryKey]
        [Identity]
        public int Id { get; set; }

        [Column]
        public long ShopItemId { get; set; }

        [Column]
        public int PriceGroupId { get; set; }

        [Column]
        public int EffectGroupId { get; set; }

        [Column]
        public byte DiscountPercentage { get; set; }

        [Column]
        public bool IsEnabled { get; set; }

        [Association(CanBeNull = true, ThisKey = "ShopItemId", OtherKey = "Id")]
        public ShopItemEntity ShopItem { get; set; }
    }
}
