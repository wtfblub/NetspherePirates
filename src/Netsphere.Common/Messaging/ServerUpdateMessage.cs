using System.Net;

namespace Netsphere.Common.Messaging
{
    public class ServerUpdateMessage
    {
        public ushort Id { get; set; }
        public ServerType ServerType { get; set; }
        public string Name { get; set; }
        public ushort Online { get; set; }
        public ushort Limit { get; set; }
        public IPEndPoint EndPoint { get; set; }
    }
}
