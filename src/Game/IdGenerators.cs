using System.Linq;
using System.Threading;

namespace Netsphere
{
    internal static class ItemIdGenerator
    {
        private static long _counter;

        public static void Initialize()
        {
            if (GameDatabase.Instance.PlayerItems.Any())
                _counter = GameDatabase.Instance.PlayerItems.Max(item => item.Id);
        }

        public static ulong GetNextId()
        {
            return (ulong)Interlocked.Add(ref _counter, 1);
        }
    }

    internal static class CharacterIdGenerator
    {
        private static int _counter;

        public static void Initialize()
        {
            if (GameDatabase.Instance.PlayerCharacters.Any())
                _counter = GameDatabase.Instance.PlayerCharacters.Max(@char => @char.Id);
        }

        public static int GetNextId()
        {
            return Interlocked.Add(ref _counter, 1);
        }
    }

    internal static class LicenseIdGenerator
    {
        private static int _counter;

        public static void Initialize()
        {
            if (GameDatabase.Instance.PlayerLicenses.Any())
                _counter = GameDatabase.Instance.PlayerLicenses.Max(license => license.Id);
        }

        public static int GetNextId()
        {
            return Interlocked.Add(ref _counter, 1);
        }
    }

    internal static class DenyIdGenerator
    {
        private static int _counter;

        public static void Initialize()
        {
            if (GameDatabase.Instance.PlayerDeny.Any())
                _counter = GameDatabase.Instance.PlayerDeny.Max(deny => deny.Id);
        }

        public static int GetNextId()
        {
            return Interlocked.Add(ref _counter, 1);
        }
    }
}
