using System;
using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class ShopItemDto : IEquatable<ShopItemDto>
    {
        [BlubMember(0)]
        public ItemNumber ItemNumber { get; set; }

        [BlubMember(1)]
        public ItemPriceType PriceType { get; set; }

        [BlubMember(2)]
        public ItemPeriodType PeriodType { get; set; }

        [BlubMember(3)]
        public ushort Period { get; set; }

        [BlubMember(4)]
        public byte Color { get; set; }

        [BlubMember(5)]
        public uint Effect { get; set; }

        public bool Equals(ShopItemDto other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return ItemNumber.Equals(other.ItemNumber) &&
                   PriceType == other.PriceType &&
                   PeriodType == other.PeriodType &&
                   Period == other.Period &&
                   Color == other.Color &&
                   Effect == other.Effect;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            return Equals((ShopItemDto)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ItemNumber.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)PriceType;
                hashCode = (hashCode * 397) ^ (int)PeriodType;
                hashCode = (hashCode * 397) ^ Period.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Effect;
                return hashCode;
            }
        }

        public static bool operator ==(ShopItemDto left, ShopItemDto right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ShopItemDto left, ShopItemDto right)
        {
            return !Equals(left, right);
        }
    }
}
