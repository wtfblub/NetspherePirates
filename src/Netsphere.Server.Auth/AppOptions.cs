using System;
using System.Net;
using Netsphere.Common.Configuration;

namespace Netsphere.Server.Auth
{
    public class AppOptions
    {
        public IPEndPoint Listener { get; set; }
        public int WorkerThreads { get; set; }
        public TimeSpan ServerlistTimeout { get; set; }
        public DatabaseOptions Database { get; set; }
        public LoggerOptions Logging { get; set; }
        public string DataDir { get; set; }
    }
}
