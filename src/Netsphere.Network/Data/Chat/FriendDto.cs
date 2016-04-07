using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Chat
{
    public class FriendDto
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [Serialize(2)]
        public uint State { get; set; } // request pending, accepted etc.

        [Serialize(3)]
        public uint Unk { get; set; }

        public FriendDto()
        {
            Nickname = "";
        }
    }
}
