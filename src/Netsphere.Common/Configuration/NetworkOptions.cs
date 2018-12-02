using System.Net;

namespace Netsphere.Common.Configuration
{
    public class NetworkOptions
    {
        public IPEndPoint Listener { get; set; }
        public IPAddress Address { get; set; }
        public ushort[] UdpPorts { get; set; }
        public int WorkerThreads { get; set; }
        public uint MaxSessions { get; set; }
    }
}
