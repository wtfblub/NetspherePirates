using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("start_items")]
    public class StartItemEntity : Entity
    {
        [Column]
        public int ShopItemInfoId { get; set; }

        [Column]
        public int ShopPriceId { get; set; }

        [Column]
        public int ShopEffectId { get; set; }

        [Column]
        public byte Color { get; set; }

        [Column]
        public int Count { get; set; }

        [Column]
        public byte RequiredSecurityLevel { get; set; }

        [Association(CanBeNull = true, ThisKey = "ShopItemInfoId", OtherKey = "Id")]
        public ShopItemInfoEntity ShopItemInfo { get; set; }

        [Association(CanBeNull = true, ThisKey = "ShopPriceId", OtherKey = "Id")]
        public ShopPriceEntity ShopPrice { get; set; }

        [Association(CanBeNull = true, ThisKey = "ShopEffectId", OtherKey = "Id")]
        public ShopEffectEntity ShopEffect { get; set; }
    }
}
