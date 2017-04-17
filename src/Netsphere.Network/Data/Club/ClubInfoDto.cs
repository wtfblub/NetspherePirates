using BlubLib.Serialization;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Data.Club
{
    [BlubContract]
    public class ClubInfoDto
    {
        [BlubMember(0)]
        public int Id { get; set; }
        
        [BlubMember(1, typeof(StringSerializer))]
        public string Unk1 { get; set; }
        
        [BlubMember(2, typeof(StringSerializer))]
        public string Unk2 { get; set; }

        [BlubMember(3)]
        public int Unk3 { get; set; }
        
        [BlubMember(4, typeof(StringSerializer))]
        public string Unk4 { get; set; }
        
        [BlubMember(5, typeof(StringSerializer))]
        public string Unk5 { get; set; }

        [BlubMember(6)]
        public int Unk6 { get; set; }

        [BlubMember(7)]
        public int Unk7 { get; set; }

        [BlubMember(8)]
        public int Unk8 { get; set; }

        [BlubMember(9)]
        public int Unk9 { get; set; }

        [BlubMember(10)]
        public int Unk10 { get; set; }

        [BlubMember(11)]
        public int Unk11 { get; set; }
        
        [BlubMember(12, typeof(StringSerializer))]
        public string Unk12 { get; set; }
        
        [BlubMember(13, typeof(StringSerializer))]
        public string Unk13 { get; set; }

        [BlubMember(14)]
        public int Unk14 { get; set; }

        [BlubMember(15)]
        public int Unk15 { get; set; }

        [BlubMember(16)]
        public int Unk16 { get; set; }

        [BlubMember(17)]
        public int Unk17 { get; set; }
    }
}
