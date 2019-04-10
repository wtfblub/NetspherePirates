using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class PromotionActiveDto
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public string Unk4 { get; set; }
    }
}
