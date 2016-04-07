using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Game
{
    public class ItemDto
    {
        [Serialize(0)]
        public ulong Id { get; set; }

        [Serialize(1)]
        public ItemNumber ItemNumber { get; set; }

        [Serialize(2, typeof(EnumSerializer))]
        public ItemPriceType PriceType { get; set; }

        [Serialize(3, typeof(EnumSerializer))]
        public ItemPeriodType PeriodType { get; set; }

        [Serialize(4)]
        public ushort Period { get; set; }

        [Serialize(5)]
        public uint Color { get; set; }

        [Serialize(6)]
        public uint Effect { get; set; }

        [Serialize(7)]
        public uint Refund { get; set; }

        [Serialize(8)]
        public long PurchaseTime { get; set; }

        [Serialize(9)]
        public long ExpireTime { get; set; }

        [Serialize(10)]
        public int Durability { get; set; }

        [Serialize(11)]
        public int TimeLeft { get; set; } // ToDo time in seconds or units?

        [Serialize(12)]
        public uint Quantity { get; set; }

        // ToDo: esper chip shit
        [Serialize(13)]
        public uint Unk1 { get; set; } // chip

        [Serialize(14)]
        public long Unk2 { get; set; }

        [Serialize(15)]
        public long Unk3 { get; set; }

        [Serialize(16)]
        public int Unk4 { get; set; } // TimeLeft?

        [Serialize(17)]
        public uint Unk5 { get; set; } // PeriodType?

        [Serialize(18)]
        public uint Unk6 { get; set; } // Effect?
    }
}
