using DotNetty.Transport.Channels;
using Logging;

namespace ProudNet
{
    public class ProudSessionFactory : ISessionFactory
    {
        public ProudSession Create(ILogger logger, uint hostId, IChannel channel)
        {
            return new ProudSession(logger, hostId, channel);
        }
    }
}
