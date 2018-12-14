using Netsphere;

namespace WebApi.Models
{
    public class ChannelDto
    {
        public uint Id { get; set; }
        public ChannelCategory Category { get; set; }
        public string Name { get; set; }
        public int PlayerLimit { get; set; }
        public byte Type { get; set; }
        public int PlayersOnline { get; set; }
    }
}
