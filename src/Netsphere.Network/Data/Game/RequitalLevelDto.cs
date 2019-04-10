using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class RequitalLevelDto
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        [BlubMember(4)]
        public int Unk5 { get; set; }

        [BlubMember(5)]
        public short Unk6 { get; set; }

        [BlubMember(6)]
        public byte Unk7 { get; set; }

        [BlubMember(7)]
        public int Unk8 { get; set; }
    }
}
