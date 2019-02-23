using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class ClubNoteDto
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }

        [BlubMember(2)]
        public byte Unk3 { get; set; }

        [BlubMember(3)]
        public byte Unk4 { get; set; }

        [BlubMember(4)]
        public byte Unk5 { get; set; }

        [BlubMember(5)]
        public string Unk6 { get; set; }

        [BlubMember(6)]
        public string Unk7 { get; set; }

        public ClubNoteDto()
        {
            Unk6 = "";
            Unk7 = "";
        }
    }
}
