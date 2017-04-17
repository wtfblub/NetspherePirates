using BlubLib.Serialization;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Data.Club
{
    [BlubContract]
    public class JoinWaiterInfoDto
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        [BlubMember(4, typeof(StringSerializer))]
        public string Unk5 { get; set; }

        [BlubMember(5, typeof(StringSerializer))]
        public string Unk6 { get; set; }

        [BlubMember(6, typeof(StringSerializer))]
        public string Unk7 { get; set; }

        [BlubMember(7, typeof(StringSerializer))]
        public string Unk8 { get; set; }

        [BlubMember(8, typeof(StringSerializer))]
        public string Unk9 { get; set; }

        [BlubMember(9, typeof(StringSerializer))]
        public string Unk10 { get; set; }

        [BlubMember(10, typeof(StringSerializer))]
        public string Unk11 { get; set; }

        [BlubMember(11, typeof(StringSerializer))]
        public string Unk12 { get; set; }

        [BlubMember(12, typeof(StringSerializer))]
        public string Unk13 { get; set; }

        [BlubMember(13, typeof(StringSerializer))]
        public string Unk14 { get; set; }

        [BlubMember(14, typeof(StringSerializer))]
        public string Unk15 { get; set; }
    }
}
