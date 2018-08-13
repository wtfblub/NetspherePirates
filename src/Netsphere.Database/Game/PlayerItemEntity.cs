using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("player_items")]
    public class PlayerItemEntity : Entity
    {
        [Column]
        public int PlayerId { get; set; }

        [Column]
        public int ShopItemInfoId { get; set; }

        [Column]
        public int ShopPriceId { get; set; }

        [Column]
        public uint Effect { get; set; }

        [Column]
        public byte Color { get; set; }

        [Column]
        public long PurchaseDate { get; set; }

        [Column]
        public int Durability { get; set; }

        [Column]
        public int Count { get; set; }

        [Association(CanBeNull = true, ThisKey = "PlayerId", OtherKey = "Id")]
        public PlayerEntity Player { get; set; }

        [Association(CanBeNull = true, ThisKey = "ShopItemInfoId", OtherKey = "Id")]
        public ShopItemInfoEntity ShopItemInfo { get; set; }

        [Association(CanBeNull = true, ThisKey = "ShopPriceId", OtherKey = "Id")]
        public ShopPriceEntity ShopPrice { get; set; }
    }
}
