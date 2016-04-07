using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    public class ChannelInfoDto
    {
        [Serialize(0)]
        public ushort ChannelId { get; set; }

        [Serialize(1)]
        public ushort PlayerCount { get; set; }
    }
}
