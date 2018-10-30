using System.Net;

namespace Netsphere.Common.Configuration
{
    public class NetworkOptions
    {
        public IPEndPoint Listener { get; set; }
        public int WorkerThreads { get; set; }
        public uint MaxSessions { get; set; }
    }
}
