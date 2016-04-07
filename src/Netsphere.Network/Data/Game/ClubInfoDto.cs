using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Game
{
    public class ClubInfoDto
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Unk1 { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Unk2 { get; set; }

        [Serialize(2, typeof(StringSerializer))]
        public string Unk3 { get; set; }

        [Serialize(3, typeof(StringSerializer))]
        public string Unk4 { get; set; }

        [Serialize(4, typeof(StringSerializer))]
        public string Unk5 { get; set; }

        [Serialize(5)]
        public ushort Unk6 { get; set; }

        [Serialize(6)]
        public uint Unk7 { get; set; }

        [Serialize(7)]
        public uint Unk8 { get; set; }

        public ClubInfoDto()
        {
            Unk1 = "";
            Unk2 = "";
            Unk3 = "";
            Unk4 = "";
            Unk5 = "";
        }
    }
}
