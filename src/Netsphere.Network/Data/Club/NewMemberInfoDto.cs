using BlubLib.Serialization;

namespace Netsphere.Network.Data.Club
{
    [BlubContract]
    public class NewMemberInfoDto
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public string Unk1 { get; set; }

        [BlubMember(2)]
        public int Unk2 { get; set; }

        [BlubMember(3)]
        public int Unk3 { get; set; }

        [BlubMember(4)]
        public int Unk4 { get; set; }

        [BlubMember(5)]
        public string Unk5 { get; set; }

        [BlubMember(6)]
        public string Unk6 { get; set; }

        [BlubMember(7)]
        public string Unk7 { get; set; }

        [BlubMember(8)]
        public string Unk8 { get; set; }

        [BlubMember(9)]
        public string Unk9 { get; set; }

        [BlubMember(10)]
        public string Unk10 { get; set; }

        [BlubMember(11)]
        public string Unk11 { get; set; }

        [BlubMember(12)]
        public string Unk12 { get; set; }

        [BlubMember(13)]
        public string Unk13 { get; set; }

        [BlubMember(14)]
        public string Unk14 { get; set; }

        [BlubMember(15)]
        public string Unk15 { get; set; }
    }
}
