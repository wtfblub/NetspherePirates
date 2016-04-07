using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Game
{
    public class RandomShopDto
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public ItemNumber[] ItemNumbers { get; set; }

        [Serialize(1, typeof(ArrayWithIntPrefixSerializer))]
        public uint[] Effects { get; set; }

        [Serialize(2, typeof(ArrayWithIntPrefixSerializer))]
        public uint[] Colors { get; set; }

        [Serialize(3, typeof(ArrayWithIntPrefixSerializer), typeof(EnumSerializer))]
        public ItemPeriodType[] PeriodTypes { get; set; }

        [Serialize(4, typeof(ArrayWithIntPrefixSerializer))]
        public ushort[] Periods { get; set; }

        [Serialize(5)]
        public uint Unk6 { get; set; }

        public RandomShopDto()
        {
            ItemNumbers = Array.Empty<ItemNumber>();
            Effects = Array.Empty<uint>();
            Colors = Array.Empty<uint>();
            PeriodTypes = Array.Empty<ItemPeriodType>();
            Periods = Array.Empty<ushort>();
        }
    }
}
