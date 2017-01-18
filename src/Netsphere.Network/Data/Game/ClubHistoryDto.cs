using BlubLib.Serialization;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class ClubHistoryDto
    {
        [BlubMember(0)]
        public uint Unk1 { get; set; }

        [BlubMember(1)]
        public uint Unk2 { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string Unk3 { get; set; }

        [BlubMember(3, typeof(StringSerializer))]
        public string Unk4 { get; set; }

        [BlubMember(4, typeof(StringSerializer))]
        public string Unk5 { get; set; }

        [BlubMember(5, typeof(StringSerializer))]
        public string Unk6 { get; set; }

        [BlubMember(6, typeof(StringSerializer))]
        public string Unk7 { get; set; }

        [BlubMember(7, typeof(StringSerializer))]
        public string Unk8 { get; set; }

        public ClubHistoryDto()
        {
            Unk3 = "";
            Unk4 = "";
            Unk5 = "";
            Unk6 = "";
            Unk7 = "";
            Unk8 = "";
        }
    }
}
