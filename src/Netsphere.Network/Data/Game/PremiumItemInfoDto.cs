using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class PremiumItemInfoDto
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public string Unk4 { get; set; }

        [BlubMember(4)]
        public string Unk5 { get; set; }

        [BlubMember(5)]
        public int Unk6 { get; set; }

        [BlubMember(6)]
        public int Unk7 { get; set; }

        [BlubMember(7)]
        public short Unk8 { get; set; }

        [BlubMember(8)]
        public short Unk9 { get; set; }
    }
}
