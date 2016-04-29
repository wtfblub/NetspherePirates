using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class ItemDto
    {
        [BlubMember(0)]
        public ulong Id { get; set; }

        [BlubMember(1)]
        public ItemNumber ItemNumber { get; set; }

        [BlubMember(2)]
        public ItemPriceType PriceType { get; set; }

        [BlubMember(3)]
        public ItemPeriodType PeriodType { get; set; }

        [BlubMember(4)]
        public ushort Period { get; set; }

        [BlubMember(5)]
        public uint Color { get; set; }

        [BlubMember(6)]
        public uint Effect { get; set; }

        [BlubMember(7)]
        public uint Refund { get; set; }

        [BlubMember(8)]
        public long PurchaseTime { get; set; }

        [BlubMember(9)]
        public long ExpireTime { get; set; }

        [BlubMember(10)]
        public int Durability { get; set; }

        [BlubMember(11)]
        public int TimeLeft { get; set; } // ToDo time in seconds or units?

        [BlubMember(12)]
        public uint Quantity { get; set; }

        // ToDo: esper chip shit
        [BlubMember(13)]
        public uint Unk1 { get; set; } // chip

        [BlubMember(14)]
        public long Unk2 { get; set; }

        [BlubMember(15)]
        public long Unk3 { get; set; }

        [BlubMember(16)]
        public int Unk4 { get; set; } // TimeLeft?

        [BlubMember(17)]
        public uint Unk5 { get; set; } // PeriodType?

        [BlubMember(18)]
        public uint Unk6 { get; set; } // Effect?
    }
}
