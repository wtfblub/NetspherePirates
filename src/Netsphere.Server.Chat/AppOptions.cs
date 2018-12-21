using Netsphere.Common.Configuration;

namespace Netsphere.Server.Chat
{
    public class AppOptions
    {
        public NetworkOptions Network { get; set; }
        public ServerListOptions ServerList { get; set; }
        public DatabaseOptions Database { get; set; }
        public LoggerOptions Logging { get; set; }
        public string PluginDirectory { get; set; }
    }
}
