using BlubLib.Serialization;

namespace Netsphere.Network.Data.GameRule
{
    [BlubContract]
    public class SeizeIntrudeInfoDto
    {
        [BlubMember(0)]
        public short Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }

        [BlubMember(2)]
        public short Unk3 { get; set; }

        [BlubMember(3)]
        public short Unk4 { get; set; }

        [BlubMember(4)]
        public short Unk5 { get; set; }
    }
}
