using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class BookEffectInfoDto
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public short Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        [BlubMember(4)]
        public int Unk5 { get; set; }

        [BlubMember(5)]
        public int Unk6 { get; set; }

        [BlubMember(6)]
        public string Unk7 { get; set; }

        [BlubMember(7)]
        public string Unk8 { get; set; }

        [BlubMember(8)]
        public string Unk9 { get; set; }

        [BlubMember(9)]
        public string Unk10 { get; set; }

        [BlubMember(10)]
        public string Unk11 { get; set; }
    }
}
