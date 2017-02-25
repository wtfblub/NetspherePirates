using System;
using BlubLib.Serialization;
using BlubLib.Serialization.Serializers;
using Netsphere.Network.Serializers;
using ProudNet.Serialization.Serializers;
using SlimMath;

namespace Netsphere.Network.Data.P2P
{
    [BlubContract]
    public class CharacterDto
    {
        [BlubMember(0)]
        public LongPeerId Id { get; set; }

        [BlubMember(1)]
        public Team Team { get; set; }

        [BlubMember(2, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [BlubMember(3)]
        public byte Rotation1 { get; set; }

        [BlubMember(4)]
        public byte Rotation2 { get; set; }

        [BlubMember(5, typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Costumes { get; set; }

        [BlubMember(6, typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Skills { get; set; }

        [BlubMember(7, typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Weapons { get; set; }

        [BlubMember(8, typeof(EnumSerializer), typeof(uint))]
        public WeaponSlot CurrentWeapon { get; set; }

        [BlubMember(9)]
        public CharacterGender Gender { get; set; }

        [BlubMember(10, typeof(StringSerializer))]
        public string Name { get; set; }

        [BlubMember(11)]
        public byte Unk4 { get; set; }

        [BlubMember(12, typeof(CompressedFloatSerializer))]
        public float CurrentHP { get; set; }

        [BlubMember(13, typeof(CompressedFloatSerializer))]
        public float MaxHP { get; set; }

        [BlubMember(14, typeof(ArrayWithIntPrefixSerializer))]
        public ValueDto[] Values { get; set; }

        public CharacterDto()
        {
            Id = 0;
            CurrentWeapon = WeaponSlot.None;
            Position = Vector3.Zero;
            Costumes = Array.Empty<ItemDto>();
            Skills = Array.Empty<ItemDto>();
            Weapons = Array.Empty<ItemDto>();
            Name = "";
            Values = Array.Empty<ValueDto>();
        }
    }
}
