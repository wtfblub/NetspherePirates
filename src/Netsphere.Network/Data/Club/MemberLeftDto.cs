using BlubLib.Serialization;

namespace Netsphere.Network.Data.Club
{
    public class MemberLeftDto
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        [BlubMember(4)]
        public string Unk5 { get; set; }
    }
}
