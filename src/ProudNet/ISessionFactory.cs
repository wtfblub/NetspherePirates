using DotNetty.Transport.Channels;

namespace ProudNet
{
    public interface ISessionFactory
    {
        ProudSession Create(uint hostId, IChannel channel);
    }
}
