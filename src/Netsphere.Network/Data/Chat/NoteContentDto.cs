using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Chat
{
    public class NoteContentDto
    {
        [Serialize(0)]
        public ulong Id { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Message { get; set; }

        [Serialize(2)]
        public NoteGiftDto Gift { get; set; }

        public NoteContentDto()
        {
            Message = "";
            Gift = new NoteGiftDto();
        }
    }
}
