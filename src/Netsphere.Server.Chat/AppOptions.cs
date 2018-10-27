using System;
using System.Net;
using Netsphere.Common.Configuration;

namespace Netsphere.Server.Chat
{
    public class AppOptions
    {
        public ServerOptions Server { get; set; }
        public string RedisConnectionString { get; set; }
        public DatabasesOptions Database { get; set; }
        public LoggerOptions Logging { get; set; }
    }

    public class ServerOptions
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
        public IPEndPoint Listener { get; set; }
        public int WorkerThreads { get; set; }
        public int PlayerLimit { get; set; }
        public TimeSpan ServerUpdateInterval { get; set; }
    }
}
