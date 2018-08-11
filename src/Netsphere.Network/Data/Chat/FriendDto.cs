using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class FriendDto
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public string Nickname { get; set; }

        [BlubMember(2)]
        public uint State { get; set; } // request pending, accepted etc.

        [BlubMember(3)]
        public uint Unk { get; set; }

        public FriendDto()
        {
            Nickname = "";
        }
    }
}
