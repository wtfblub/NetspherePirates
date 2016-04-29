using BlubLib.Serialization;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class NoteDto
    {
        [BlubMember(0)]
        public ulong Id { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Sender { get; set; }

        [BlubMember(2, typeof(IntBooleanSerializer))]
        public bool IsGift { get; set; }

        [BlubMember(3)]
        public ulong Unk1 { get; set; }

        [BlubMember(4)]
        public ulong Unk2 { get; set; }

        [BlubMember(5, typeof(StringSerializer))]
        public string Title { get; set; }

        [BlubMember(6)]
        public uint ReadCount { get; set; }

        [BlubMember(7)]
        public bool OpenedGift { get; set; }

        [BlubMember(8)]
        public byte DaysLeft { get; set; }

        public NoteDto()
        {
            Sender = "";
            Title = "";
        }
    }
}
