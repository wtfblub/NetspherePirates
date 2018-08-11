using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class NoteContentDto
    {
        [BlubMember(0)]
        public ulong Id { get; set; }

        [BlubMember(1)]
        public string Message { get; set; }

        [BlubMember(2)]
        public NoteGiftDto Gift { get; set; }

        public NoteContentDto()
        {
            Message = "";
            Gift = new NoteGiftDto();
        }
    }
}
