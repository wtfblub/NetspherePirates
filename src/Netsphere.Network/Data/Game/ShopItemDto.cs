using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class ShopItemDto
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
        public byte Color { get; set; }

        [BlubMember(5)]
        public uint Effect { get; set; }
    }
}
