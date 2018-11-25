using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using ProudNet;

namespace Netsphere.Server.Relay
{
    public class Session : ProudSession
    {
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
