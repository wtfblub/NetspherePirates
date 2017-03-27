using BlubLib.Serialization;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class PlayerClubInfoDto
    {
        [BlubMember(0)]
        public uint Unk1 { get; set; }

        [BlubMember(1)]
        public uint Unk2 { get; set; }

        [BlubMember(2)]
        public uint Unk3 { get; set; }

        [BlubMember(3)]
        public uint Unk4 { get; set; }

        [BlubMember(4)]
        public ulong Unk5 { get; set; }

        [BlubMember(5)]
        public uint Unk6 { get; set; }

        [BlubMember(6, typeof(StringSerializer))]
        public string Unk7 { get; set; }

        [BlubMember(7, typeof(StringSerializer))]
        public string Unk8 { get; set; }

        [BlubMember(8, typeof(StringSerializer))]
        public string Unk9 { get; set; } // Clan name?

        [BlubMember(9, typeof(StringSerializer))]
        public string ModeratorName { get; set; }

        [BlubMember(10, typeof(StringSerializer))]
        public string Unk11 { get; set; }

        [BlubMember(11, typeof(StringSerializer))]
        public string Unk12 { get; set; }

        [BlubMember(12, typeof(StringSerializer))]
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
