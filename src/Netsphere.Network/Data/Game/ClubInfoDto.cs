using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class ClubInfoDto
    {
        [BlubMember(0)]
        public string Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public string Unk3 { get; set; }

        [BlubMember(3)]
        public string Unk4 { get; set; }

        [BlubMember(4)]
        public string Unk5 { get; set; }

        [BlubMember(5)]
        public ushort Unk6 { get; set; }

        [BlubMember(6)]
        public uint Unk7 { get; set; }

        [BlubMember(7)]
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
