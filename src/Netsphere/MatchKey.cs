using System;

namespace Netsphere
{
    public class MatchKey : IEquatable<MatchKey>
    {
        private readonly byte[] _key;

        public uint Key => BitConverter.ToUInt32(_key, 0);
        public byte GameType => (byte)(_key[0] & 1);
        public byte PublicType => (byte)((_key[0] >> 1) & 1);
        public byte JoinAuth => (byte)((_key[0] >> 2) & 1);

        // Contains spectator limit
        public bool IsObserveEnabled
        {
            get => _key[3] > 0;
            set => _key[3] = (byte)(value ? 1 : 0);
        }

        public GameRule GameRule
        {
            get => (GameRule)(byte)(_key[0] >> 4);
            set => _key[0] = (byte)(_key[0] & 0x0F | (byte)value << 4);
        }

        public byte Map
        {
            get => _key[1];
            set => _key[1] = value;
        }

        public int PlayerLimit
        {
            get
            {
                switch (_key[2])
                {
                    case 8:
                        return 12;

                    case 7:
                        return 10;

                    case 6:
                        return 8;

                    case 5:
                        return 6;

                    case 3:
                        return 4;
                }

                return _key[2];
            }
            set
            {
                switch (value)
                {
                    case 12:
                        _key[2] = 8;
                        break;

                    case 10:
                        _key[2] = 7;
                        break;

                    case 8:
                        _key[2] = 6;
                        break;

                    case 6:
                        _key[2] = 5;
                        break;

                    case 4:
                        _key[2] = 3;
                        break;

                    default:
                        _key[2] = (byte)value;
                        break;
                }
            }
        }

        public int SpectatorLimit => IsObserveEnabled ? 12 - PlayerLimit : 0;

        public MatchKey()
            : this(0)
        {
        }

        public MatchKey(uint key)
        {
            _key = BitConverter.GetBytes(key);
        }

        public override bool Equals(object obj)
        {
            return Key.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public bool Equals(MatchKey other)
        {
            return Key == other.Key;
        }

        public static implicit operator uint(MatchKey i)
        {
            return i.Key;
        }

        public static implicit operator MatchKey(uint key)
        {
            return new MatchKey(key);
        }
    }
}
