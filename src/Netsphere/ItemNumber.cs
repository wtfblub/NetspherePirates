using System;

namespace Netsphere
{
    public struct ItemNumber : IEquatable<ItemNumber>
    {
        public uint Id { get; }
        public ItemCategory Category { get; }
        public byte SubCategory { get; }
        public ushort Number { get; }

        public ItemNumber(long id)
            : this((uint)id)
        { }

        public ItemNumber(uint id)
        {
            Id = id;
            Category = (ItemCategory)(id / 1000000);

            var tmp = (byte)Category * 1000000;
            SubCategory = (byte)((id - tmp) / 10000);

            tmp = SubCategory * 10000 + tmp;
            Number = (ushort)(id - tmp);
        }

        public ItemNumber(ItemCategory category, byte subCategory, ushort number)
        {
            Category = category;
            SubCategory = subCategory;
            Number = number;

            Id = (uint)((byte)Category * 1000000 + SubCategory * 10000 + Number);
        }

        public ItemNumber(byte category, byte subCategory, ushort number)
            : this((ItemCategory)category, subCategory, number)
        { }

        public override bool Equals(object obj)
        {
            return Id.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public bool Equals(ItemNumber other)
        {
            return Id == other.Id;
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        public static implicit operator uint(ItemNumber i)
        {
            return i.Id;
        }

        public static implicit operator ItemNumber(long id)
        {
            return new ItemNumber(id);
        }

        public static implicit operator ItemNumber(uint id)
        {
            return new ItemNumber(id);
        }

        public static implicit operator ItemCategory(ItemNumber i)
        {
            return i.Category;
        }

        public static bool operator ==(ItemNumber a, ItemNumber b)
        {
            return a.Id == b.Id;
        }

        public static bool operator !=(ItemNumber a, ItemNumber b)
        {
            return a.Id != b.Id;
        }
    }
}
