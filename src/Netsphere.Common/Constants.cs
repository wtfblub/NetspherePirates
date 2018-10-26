namespace Netsphere.Common
{
    public static class Constants
    {
        public static class Cache
        {
            public static string SessionKey(long accountId)
            {
                return $"session_{accountId}";
            }

            public static string SessionKey(ulong accountId)
            {
                return $"session_{accountId}";
            }

            public const string ServerlistKey = "serverlist";
        }
    }
}
