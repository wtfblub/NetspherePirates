using System.Net;

namespace Netsphere.Common.Caching
{
    public class ServerInfo
    {
        public int GameId { get; set; }
        public int ChatId { get; set; }
        public string Name { get; set; }
        public int Online { get; set; }
        public int Limit { get; set; }
        public IPEndPoint GameEndPoint { get; set; }
        public IPEndPoint ChatEndPoint { get; set; }
    }
}
