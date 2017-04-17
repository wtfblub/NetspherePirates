using BlubLib.Serialization;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Data.Club
{
    [BlubContract]
    public class UnjoinSettingMemberDto
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3, typeof(StringSerializer))]
        public string Unk4 { get; set; }

        [BlubMember(4, typeof(StringSerializer))]
        public string Unk5 { get; set; }
    }
}
