using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;

namespace ProudNet
{
    public interface ISessionFactory
    {
        ProudSession Create(ILogger logger, uint hostId, IChannel channel);
    }
}
