using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Data.GameRule
{
    [BlubContract]
    public class ChangeAvatarUnk1Dto
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1, typeof(ArrayWithIntPrefixSerializer))]
        public ItemNumber[] Costumes { get; set; }

        [BlubMember(2, typeof(ArrayWithIntPrefixSerializer))]
        public ItemNumber[] Skills { get; set; }

        [BlubMember(3, typeof(ArrayWithIntPrefixSerializer))]
        public ItemNumber[] Weapons { get; set; }

        [BlubMember(4, typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk5 { get; set; }

        [BlubMember(5, typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk6 { get; set; }

        [BlubMember(6, typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk7 { get; set; }

        [BlubMember(7)]
        public int Unk8 { get; set; }

        [BlubMember(8)]
        public CharacterGender Gender { get; set; }

        [BlubMember(9)]
        public float HP { get; set; }

        [BlubMember(10)]
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
