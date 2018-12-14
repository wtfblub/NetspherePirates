using Netsphere;

namespace WebApi.Models
{
    public class ItemDto
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int MasterLevel { get; set; }
        public ItemLicense License { get; set; }
        public Gender Gender { get; set; }
    }
}
