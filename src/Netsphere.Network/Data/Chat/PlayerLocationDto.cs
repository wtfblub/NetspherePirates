using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class PlayerLocationDto
    {
        [BlubMember(0)]
        public int ServerGroupId { get; set; }
        
        [BlubMember(2)]
        public int ChannelId { get; set; }
        
        [BlubMember(3)]
        public int RoomId { get; set; }
        
        [BlubMember(4)]
        public int Unk { get; set; }
        
        [BlubMember(5)]
        public int GameServerId { get; set; }
        
        [BlubMember(6)]
        public int ChatServerId { get; set; }
    }
}
