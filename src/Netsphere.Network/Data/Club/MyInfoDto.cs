using BlubLib.Serialization;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Data.Club
{
    [BlubContract]
    public class MyInfoDto
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }
        
        [BlubMember(1, typeof(StringSerializer))]
        public string Unk2 { get; set; }
        
        [BlubMember(2, typeof(StringSerializer))]
        public string Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        [BlubMember(4)]
        public int Unk5 { get; set; }

        [BlubMember(5)]
        public int Unk6 { get; set; }

        [BlubMember(6)]
        public int Unk7 { get; set; }

        [BlubMember(7)]
        public int Unk8 { get; set; }

        [BlubMember(8)]
        public int Unk9 { get; set; }

        [BlubMember(9)]
        public int Unk10 { get; set; }

        [BlubMember(10)]
        public int Unk11 { get; set; }

        [BlubMember(11)]
        public int Unk12 { get; set; }

        [BlubMember(12)]
        public byte Unk13 { get; set; }
    }
}
