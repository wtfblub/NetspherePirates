using Netsphere.Common.Configuration;

namespace Netsphere.Server.Relay
{
    public class AppOptions
    {
        public NetworkOptions Network { get; set; }
        public DatabaseOptions Database { get; set; }
        public LoggerOptions Logging { get; set; }
    }
}
