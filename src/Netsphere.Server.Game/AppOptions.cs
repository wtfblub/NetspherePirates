using System;
using Netsphere.Common.Configuration;

namespace Netsphere.Server.Game
{
    public class AppOptions
    {
        public NetworkOptions Network { get; set; }
        public ServerListOptions ServerList { get; set; }
        public Version[] ClientVersions { get; set; }
        public string RedisConnectionString { get; set; }
        public DatabasesOptions Database { get; set; }
        public LoggerOptions Logging { get; set; }
    }
}
