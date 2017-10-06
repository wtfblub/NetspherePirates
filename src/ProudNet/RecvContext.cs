using System.Net;

namespace ProudNet
{
    public class RecvContext
    {
        public object Message { get; set; }
        public IPEndPoint UdpEndPoint { get; set; }
    }
}
