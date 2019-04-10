using BlubLib.Serialization;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class ClubNoticeRecordDto
    {
        [BlubMember(0)]
        public string Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        [BlubMember(4)]
        public int Unk5 { get; set; }

        [BlubMember(5)]
        public int Unk6 { get; set; }

        [BlubMember(6)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public string[] Unk7 { get; set; }

        [BlubMember(7)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public string[] Unk8 { get; set; }
    }
}
