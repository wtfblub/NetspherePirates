using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Game
{
    public class RoomPlayerDto
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1)]
        public byte Unk1 { get; set; }

        [Serialize(2, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [Serialize(3)]
        public byte Unk2 { get; set; }

        public RoomPlayerDto()
        {
            Nickname = "";
        }
    }
}
