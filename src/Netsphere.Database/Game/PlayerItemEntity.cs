using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("player_items")]
    public class PlayerItemEntity
    {
        [Key]
        public long Id { get; set; }

        [Column]
        public int PlayerId { get; set; }
        public PlayerEntity Player { get; set; }

        [Column]
        public int ShopItemInfoId { get; set; }
        public ShopItemInfoEntity ShopItemInfo { get; set; }

        [Column]
        public int ShopPriceId { get; set; }
        public ShopPriceEntity ShopPrice { get; set; }

        [Column]
        public uint Effect { get; set; }

        [Column]
        public byte Color { get; set; }

        [Column]
        public long PurchaseDate { get; set; }

        [Column]
        public int Durability { get; set; }

        [Column]
        public int MP { get; set; }

        [Column]
        public int MPLevel { get; set; }
    }
}
