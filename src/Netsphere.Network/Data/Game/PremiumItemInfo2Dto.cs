using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class PremiumItemInfo2Dto
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public string Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        [BlubMember(4)]
        public short Unk5 { get; set; }

        [BlubMember(5)]
        public short Unk6 { get; set; }
    }
}
