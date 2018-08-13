using System;

namespace Netsphere
{
    public class MatchKey : IEquatable<MatchKey>
    {
        private readonly byte[] _key;

        public uint Key { get; }
        public byte GameType => (byte)(_key[0] & 1);
        public byte PublicType => (byte)((_key[0] >> 1) & 1);
        public byte JoinAuth => (byte)((_key[0] >> 2) & 1);

        // Contains spectator limit
        public bool IsObserveEnabled => _key[3] > 0;

        public GameRule GameRule => (GameRule)(byte)(_key[0] >> 4);
        public byte Map => _key[1];

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

                return 0;
            }
        }

        public int SpectatorLimit => IsObserveEnabled ? 12 - PlayerLimit : 0;

        public MatchKey()
            : this(0)
        { }

        public MatchKey(uint key)
        {
            Key = key;
            _key = BitConverter.GetBytes(Key);
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
