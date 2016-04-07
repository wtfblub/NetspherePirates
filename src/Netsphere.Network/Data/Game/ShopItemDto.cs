using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Game
{
    public class ShopItemDto
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
        public byte Color { get; set; }

        [Serialize(5)]
        public uint Effect { get; set; }
    }
}
