using DotNetty.Transport.Channels;
using Logging;
using ProudNet;

namespace Netsphere.Server.Auth
{
    internal class Session : ProudSession
    {
        public bool Authenticated { get; set; }
        public bool XbnSent { get; set; }

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
