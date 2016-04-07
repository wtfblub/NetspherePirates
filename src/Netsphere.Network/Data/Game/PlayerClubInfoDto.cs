using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Game
{
    public class PlayerClubInfoDto
    {
        [Serialize(0)]
        public uint Unk1 { get; set; }

        [Serialize(1)]
        public uint Unk2 { get; set; }

        [Serialize(2)]
        public uint Unk3 { get; set; }

        [Serialize(3)]
        public uint Unk4 { get; set; }

        [Serialize(4)]
        public ulong Unk5 { get; set; }

        [Serialize(5)]
        public uint Unk6 { get; set; }

        [Serialize(6, typeof(StringSerializer))]
        public string Unk7 { get; set; }

        [Serialize(7, typeof(StringSerializer))]
        public string Unk8 { get; set; }

        [Serialize(8, typeof(StringSerializer))]
        public string Unk9 { get; set; } // Clan name?

        [Serialize(9, typeof(StringSerializer))]
        public string ModeratorName { get; set; }

        [Serialize(10, typeof(StringSerializer))]
        public string Unk11 { get; set; }

        [Serialize(11, typeof(StringSerializer))]
        public string Unk12 { get; set; }

        [Serialize(12, typeof(StringSerializer))]
        public string Unk13 { get; set; }

        public PlayerClubInfoDto()
        {
            Unk7 = "";
            Unk8 = "";
            Unk9 = "";
            ModeratorName = "";
            Unk11 = "";
            Unk12 = "";
            Unk13 = "";
        }
    }
}
