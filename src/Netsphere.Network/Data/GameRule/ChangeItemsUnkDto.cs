using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Data.GameRule
{
    public class ChangeItemsUnkDto
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1, typeof(ArrayWithIntPrefixSerializer))]
        public ItemNumber[] Skills { get; set; }

        [Serialize(2, typeof(ArrayWithIntPrefixSerializer))]
        public ItemNumber[] Weapons { get; set; }

        [Serialize(3, typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk4 { get; set; }

        [Serialize(4, typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk5 { get; set; }

        [Serialize(5)]
        public int Unk6 { get; set; }

        [Serialize(6)]
        public float HP { get; set; }

        [Serialize(7)]
        public byte Unk8 { get; set; }

        public ChangeItemsUnkDto()
        {
            Skills = Array.Empty<ItemNumber>();
            Weapons = Array.Empty<ItemNumber>();
            Unk4 = Array.Empty<int>();
            Unk5 = Array.Empty<int>();
        }
    }
}
