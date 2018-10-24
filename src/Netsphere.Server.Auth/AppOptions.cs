using System.Net;
using Netsphere.Common.Configuration;

namespace Netsphere.Server.Auth
{
    public class AppOptions
    {
        public IPEndPoint Listener { get; set; }
        public int WorkerThreads { get; set; }
        public string RedisConnectionString { get; set; }
        public DatabasesOptions Database { get; set; }
        public LoggerOptions Logging { get; set; }
    }
}
