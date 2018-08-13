using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("license_rewards")]
    public class LicenseRewardEntity : Entity
    {
        [Column]
        public int ShopItemInfoId { get; set; }

        [Column]
        public int ShopPriceId { get; set; }

        [Column]
        public byte Color { get; set; }

        [Association(CanBeNull = false, ThisKey = "ShopItemInfoId", OtherKey = "Id")]
        public ShopItemInfoEntity ShopItemInfo { get; set; }

        [Association(CanBeNull = false, ThisKey = "ShopPiceId", OtherKey = "Id")]
        public ShopPriceEntity PriceInfo { get; set; }
    }
}
