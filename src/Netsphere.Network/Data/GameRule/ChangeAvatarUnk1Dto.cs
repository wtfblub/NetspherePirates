using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.GameRule
{
    public class ChangeAvatarUnk1Dto
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1, typeof(ArrayWithIntPrefixSerializer))]
        public ItemNumber[] Costumes { get; set; }

        [Serialize(2, typeof(ArrayWithIntPrefixSerializer))]
        public ItemNumber[] Skills { get; set; }

        [Serialize(3, typeof(ArrayWithIntPrefixSerializer))]
        public ItemNumber[] Weapons { get; set; }

        [Serialize(4, typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk5 { get; set; }

        [Serialize(5, typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk6 { get; set; }

        [Serialize(6, typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk7 { get; set; }

        [Serialize(7)]
        public int Unk8 { get; set; }

        [Serialize(8, typeof(EnumSerializer))]
        public CharacterGender Gender { get; set; }

        [Serialize(9)]
        public float HP { get; set; }

        [Serialize(10)]
        public byte Unk11 { get; set; }

        public ChangeAvatarUnk1Dto()
        {
            Costumes = Array.Empty<ItemNumber>();
            Skills = Array.Empty<ItemNumber>();
            Weapons = Array.Empty<ItemNumber>();
            Unk5 = Array.Empty<int>();
            Unk6 = Array.Empty<int>();
            Unk7 = Array.Empty<int>();
        }
    }
}
