using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Chat
{
    public class NoteGiftDto
    {
        [Serialize(0)]
        public ItemNumber ItemNumber { get; set; }

        [Serialize(1, typeof(EnumSerializer))]
        public ItemPriceType PriceType { get; set; }

        [Serialize(2, typeof(EnumSerializer))]
        public ItemPeriodType PeriodType { get; set; }

        [Serialize(3)]
        public ushort Period { get; set; }

        [Serialize(4)]
        public byte Unk5 { get; set; }

        [Serialize(5)]
        public int Unk6 { get; set; }

        [Serialize(6)]
        public byte Unk7 { get; set; }
    }
}