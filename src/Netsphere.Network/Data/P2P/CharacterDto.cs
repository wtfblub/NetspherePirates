using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;
using SlimMath;

namespace Netsphere.Network.Data.P2P
{
    public class CharacterDto
    {
        [Serialize(0)]
        public LongPeerId Id { get; set; }

        [Serialize(1, typeof(EnumSerializer))]
        public Team Team { get; set; }

        [Serialize(2, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [Serialize(3)]
        public byte Rotation1 { get; set; }

        [Serialize(4)]
        public byte Rotation2 { get; set; }

        [Serialize(5, typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Costumes { get; set; }

        [Serialize(6, typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Skills { get; set; }

        [Serialize(7, typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Weapons { get; set; }

        [Serialize(8, typeof(EnumSerializer), typeof(uint))]
        public WeaponSlot CurrentWeapon { get; set; }

        [Serialize(9, typeof(EnumSerializer))]
        public CharacterGender Gender { get; set; }

        [Serialize(10, typeof(StringSerializer))]
        public string Name { get; set; }

        [Serialize(11)]
        public byte Unk4 { get; set; }

        [Serialize(12, typeof(CompressedFloatSerializer))]
        public float CurrentHP { get; set; }

        [Serialize(13, typeof(CompressedFloatSerializer))]
        public float MaxHP { get; set; }

        [Serialize(14, typeof(ArrayWithIntPrefixSerializer))]
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
