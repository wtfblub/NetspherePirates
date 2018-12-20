using DotNetty.Transport.Channels;
using Logging;
using ProudNet;

namespace Netsphere.Server.Relay
{
    public class Session : ProudSession
    {
        public Player Player { get; set; }

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
