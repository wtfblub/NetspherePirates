using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class ClubMemberDto
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

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
        public string Unk7 { get; set; }

        [BlubMember(7)]
        public string Unk8 { get; set; }

        [BlubMember(8)]
        public int Unk9 { get; set; }

        public ClubMemberDto()
        {
            Unk2 = "";
            Unk7 = "";
            Unk8 = "";
        }
    }
}
