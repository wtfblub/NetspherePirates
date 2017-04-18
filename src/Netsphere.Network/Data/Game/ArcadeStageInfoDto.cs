using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class ArcadeStageInfoDto
    {
        [BlubMember(0)]
        public uint Unk1 { get; set; }

        [BlubMember(1)]
        public uint Unk2 { get; set; }

        [BlubMember(2)]
        public uint Unk3 { get; set; }

        [BlubMember(3)]
        public uint Unk4 { get; set; }

        [BlubMember(4)]
        public uint Unk5 { get; set; }

        [BlubMember(5)]
        public uint Unk6 { get; set; }

        [BlubMember(6)]
        public uint Unk7 { get; set; }

        [BlubMember(7)]
        public uint Unk8 { get; set; }

        [BlubMember(8)]
        public uint Unk9 { get; set; }

        [BlubMember(9)]
        public uint Unk10 { get; set; }

        [BlubMember(10)]
        public uint Unk11 { get; set; }

        [BlubMember(11)]
        public uint Unk12 { get; set; }

        [BlubMember(12)]
        public byte Unk13 { get; set; }
    }
}
