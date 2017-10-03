using System.Linq;
using System.Threading;
using Dapper.FastCrud;
using Netsphere.Database;
using Netsphere.Database.Game;

namespace Netsphere
{
    internal static class ItemIdGenerator
    {
        private static long s_counter;

        public static void Initialize()
        {
            using (var db = GameDatabase.Open())
            {
                var result = db.Find<PlayerItemDto>();
                if (result.Any())
                    s_counter = result.Max(item => item.Id);
            }
        }

        public static ulong GetNextId()
        {
            return (ulong)Interlocked.Add(ref s_counter, 1);
        }
    }

    internal static class CharacterIdGenerator
    {
        private static int s_counter;

        public static void Initialize()
        {
            using (var db = GameDatabase.Open())
            {
                var result = db.Find<PlayerCharacterDto>();
                if (result.Any())
                    s_counter = result.Max(item => item.Id);
            }
        }

        public static int GetNextId()
        {
            return Interlocked.Add(ref s_counter, 1);
        }
    }

    internal static class LicenseIdGenerator
    {
        private static int s_counter;

        public static void Initialize()
        {
            using (var db = GameDatabase.Open())
            {
                var result = db.Find<PlayerLicenseDto>();
                if (result.Any())
                    s_counter = result.Max(item => item.Id);
            }
        }

        public static int GetNextId()
        {
            return Interlocked.Add(ref s_counter, 1);
        }
    }

    internal static class DenyIdGenerator
    {
        private static int s_counter;

        public static void Initialize()
        {
            using (var db = GameDatabase.Open())
            {
                var result = db.Find<PlayerDenyDto>();
                if (result.Any())
                    s_counter = result.Max(item => item.Id);
            }
        }

        public static int GetNextId()
        {
            return Interlocked.Add(ref s_counter, 1);
        }
    }
}
