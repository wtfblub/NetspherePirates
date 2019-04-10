using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;

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
        [BlubSerializer(typeof(UnixTimeSerializer))]
        public DateTimeOffset ExpireTime { get; set; }

        [BlubMember(7)]
        public int Durability { get; set; }

        [BlubMember(8)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ItemEffectDto[] Effects { get; set; }

        [BlubMember(9)]
        public uint EnchantMP { get; set; }

        [BlubMember(10)]
        public uint EnchantLevel { get; set; }
    }
}
