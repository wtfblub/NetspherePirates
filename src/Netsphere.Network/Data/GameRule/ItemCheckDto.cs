using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Data.GameRule
{
    [BlubContract]
    public class ItemCheckDto
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }

        [BlubMember(1)]
        public ItemNumber ItemNumber { get; set; }

        [BlubMember(2)]
        public uint Color { get; set; }

        [BlubMember(3)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public uint[] Effects { get; set; }

        [BlubMember(4)]
        public float Power { get; set; }

        [BlubMember(5)]
        public float MoveSpeedRate { get; set; }

        public ItemCheckDto()
        {
            ItemNumber = 0;
            Effects = Array.Empty<uint>();
        }
    }
}
