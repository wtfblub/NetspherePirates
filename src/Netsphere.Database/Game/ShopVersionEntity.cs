using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("shop_version")]
    public class ShopVersionEntity : Entity
    {
        [Column(CanBeNull = false)]
        public string Version { get; set; }
    }
}
