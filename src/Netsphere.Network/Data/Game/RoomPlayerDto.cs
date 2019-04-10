using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class RoomPlayerDto
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public ulong AccountId { get; set; }

        [BlubMember(2)]
        public byte Unk2 { get; set; }

        [BlubMember(3)]
        public string Nickname { get; set; }

        [BlubMember(4)]
        public byte Unk3 { get; set; }

        [BlubMember(5)]
        public byte Unk4 { get; set; }
    }
}
