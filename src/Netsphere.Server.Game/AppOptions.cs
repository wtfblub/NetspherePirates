using System;
using Netsphere.Common.Configuration;

namespace Netsphere.Server.Game
{
    public class AppOptions
    {
        public NetworkOptions Network { get; set; }
        public ServerListOptions ServerList { get; set; }
        public Version[] ClientVersions { get; set; }
        public DatabaseOptions Database { get; set; }
        public LoggerOptions Logging { get; set; }
        public TimeSpan SaveInterval { get; set; }
        public GameOptions Game { get; set; }
    }
}
