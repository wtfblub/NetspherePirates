using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class NoteGiftDto
    {
        [BlubMember(0)]
        public ItemNumber ItemNumber { get; set; }

        [BlubMember(1)]
        public ItemPriceType PriceType { get; set; }

        [BlubMember(2)]
        public ItemPeriodType PeriodType { get; set; }

        [BlubMember(3)]
        public ushort Period { get; set; }

        [BlubMember(4)]
        public byte Unk5 { get; set; }

        [BlubMember(5)]
        public int Unk6 { get; set; }
    }
}
