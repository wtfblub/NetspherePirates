using LinqToDB.Mapping;

namespace Netsphere.Database.Game
{
    [Table("shop_version")]
    public class ShopVersionEntity
    {
        [PrimaryKey]
        [Identity]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public string Version { get; set; }
    }
}
