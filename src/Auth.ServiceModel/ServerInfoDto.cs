using System.Net;

namespace Auth.ServiceModel
{
    public class ServerInfoDto
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
        public ushort PlayerLimit { get; set; }
        public ushort PlayerOnline { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public IPEndPoint ChatEndPoint { get; set; }
    }
}
