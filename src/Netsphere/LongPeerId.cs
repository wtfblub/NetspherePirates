using System;

namespace Netsphere
{
    public class LongPeerId : IEquatable<LongPeerId>
    {
        public ulong AccountId { get; set; }
        public PeerId PeerId { get; set; }

        public ulong Value => GetValue();

        public LongPeerId(ulong value)
        {
            AccountId = value & 0x0000FFFFFFFFFFFF;
            PeerId = (ushort)(value >> 48);
        }

        public LongPeerId(ulong accountId, PeerId peerId)
        {
            AccountId = accountId;
            PeerId = peerId;
        }

        public LongPeerId(ulong accountId, byte id, byte slot, byte unk)
        {
            AccountId = accountId;
            PeerId = new PeerId(id, slot, unk);
        }

        public override bool Equals(object obj)
        {
            return GetValue().Equals(obj);
        }

        public override int GetHashCode()
        {
            return GetValue().GetHashCode();
        }

        public bool Equals(LongPeerId other)
        {
            return GetValue() == other.GetValue();
        }

        public override string ToString()
        {
            return $"<AccountId:{AccountId}, {PeerId}>";
        }

        private ulong GetValue()
        {
            return AccountId | ((ulong)PeerId << 48);
        }

        public static implicit operator PeerId(LongPeerId id)
        {
            return id?.PeerId;
        }

        public static implicit operator ulong(LongPeerId id)
        {
            return id?.GetValue() ?? 0;
        }

        public static implicit operator LongPeerId(ulong value)
        {
            return new LongPeerId(value);
        }

        public static bool operator ==(LongPeerId a, LongPeerId b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(LongPeerId a, LongPeerId b)
        {
            return !(a == b);
        }
    }

    public class PeerId : IEquatable<PeerId>
    {
        public byte Id { get; set; }
        public byte Slot { get; set; }
        public byte ObjectType { get; set; } // maybe source - player,sentry,senti

        public ushort Value => GetValue();

        public PeerId(ushort value)
        {
            Id = (byte)(value >> 8);
            Slot = (byte)((value >> 3) & 31);
            ObjectType = (byte)(value & 7);
        }

        public PeerId(byte id, byte slot, byte objectType)
        {
            Id = id;
            Slot = slot;
            ObjectType = objectType;
        }

        public override bool Equals(object obj)
        {
            return GetValue().Equals(obj);
        }

        public override int GetHashCode()
        {
            return GetValue().GetHashCode();
        }

        public bool Equals(PeerId other)
        {
            return GetValue() == other.GetValue();
        }

        public override string ToString()
        {
            return $"<Id:{Id}, Slot:{Slot}, Unk:{ObjectType}>";
        }

        private ushort GetValue()
        {
            var value = (ushort)(ObjectType & 7);
            value = (ushort)(8 * (Slot & 31) | value);
            value = (ushort)(Id << 8 | value);
            return value;
        }

        public static implicit operator ushort(PeerId id)
        {
            return id.GetValue();
        }

        public static implicit operator PeerId(ushort value)
        {
            return new PeerId(value);
        }

        public static bool operator ==(PeerId a, PeerId b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(PeerId a, PeerId b)
        {
            return !(a == b);
        }
    }
}
