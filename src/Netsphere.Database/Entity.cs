using LinqToDB.Mapping;

namespace Netsphere.Database
{
    public abstract class Entity
    {
        [PrimaryKey]
        [Identity]
        public int Id { get; set; }
    }
}
