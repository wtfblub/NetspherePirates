using BlubLib.Serialization;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Chat
{
    public class NoteDto
    {
        [Serialize(0)]
        public ulong Id { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Sender { get; set; }

        [Serialize(2, typeof(IntBooleanSerializer))]
        public bool IsGift { get; set; }

        [Serialize(3)]
        public ulong Unk1 { get; set; }

        [Serialize(4)]
        public ulong Unk2 { get; set; }

        [Serialize(5, typeof(StringSerializer))]
        public string Title { get; set; }

        [Serialize(6)]
        public uint ReadCount { get; set; }

        [Serialize(7)]
        public bool OpenedGift { get; set; }

        [Serialize(8)]
        public byte DaysLeft { get; set; }

        public NoteDto()
        {
            Sender = "";
            Title = "";
        }
    }
}
