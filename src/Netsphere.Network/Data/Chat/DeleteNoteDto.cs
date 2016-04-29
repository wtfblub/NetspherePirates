using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class DeleteNoteDto
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        public DeleteNoteDto()
        { }

        public DeleteNoteDto(ulong unk1, int unk2)
        {
            Unk1 = unk1;
            Unk2 = unk2;
        }
    }
}
