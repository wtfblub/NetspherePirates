using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using ProudNet;

namespace Netsphere.Server.Game
{
    public class Session : ProudSession
    {
        public Player Player { get; set; }
        public string SessionId { get; set; }

        public Session(ILogger logger, uint hostId, IChannel channel)
            : base(logger, hostId, channel)
        {
        }
    }

    internal class SessionFactory : ISessionFactory
    {
        public ProudSession Create(ILogger logger, uint hostId, IChannel channel)
        {
            return new Session(logger, hostId, channel);
        }
    }
}
