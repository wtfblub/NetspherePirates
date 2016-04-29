using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class ChannelInfoDto
    {
        [BlubMember(0)]
        public ushort ChannelId { get; set; }

        [BlubMember(1)]
        public ushort PlayerCount { get; set; }
    }
}
