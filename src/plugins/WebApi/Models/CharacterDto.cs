using Netsphere;

namespace WebApi.Models
{
    public class CharacterDto
    {
        public int Id { get; set; }
        public int Slot { get; set; }
        public CharacterGender Gender { get; set; }
        public DefaultItemDto Hair { get; set; }
        public DefaultItemDto Face { get; set; }
        public DefaultItemDto Shirt { get; set; }
        public DefaultItemDto Pants { get; set; }
        public DefaultItemDto Gloves { get; set; }
        public DefaultItemDto Shoes { get; set; }
        public ulong[] Weapons { get; set; }
        public ulong[] Skills { get; set; }
        public ulong[] Costumes { get; set; }
    }
}
