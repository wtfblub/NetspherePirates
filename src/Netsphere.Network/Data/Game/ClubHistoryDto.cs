using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Game
{
    public class ClubHistoryDto
    {
        [Serialize(0)]
        public uint Unk1 { get; set; }

        [Serialize(1)]
        public uint Unk2 { get; set; }

        [Serialize(2, typeof(StringSerializer))]
        public string Unk3 { get; set; }

        [Serialize(3, typeof(StringSerializer))]
        public string Unk4 { get; set; }

        [Serialize(4, typeof(StringSerializer))]
        public string Unk5 { get; set; }

        [Serialize(5, typeof(StringSerializer))]
        public string Unk6 { get; set; }

        [Serialize(6, typeof(StringSerializer))]
        public string Unk7 { get; set; }

        [Serialize(7, typeof(StringSerializer))]
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
