using DotNetty.Transport.Channels;
using Logging;

namespace ProudNet
{
    public interface ISessionFactory
    {
        ProudSession Create(ILogger logger, uint hostId, IChannel channel);
    }
}
