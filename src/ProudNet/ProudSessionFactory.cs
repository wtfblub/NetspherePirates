using DotNetty.Transport.Channels;

namespace ProudNet
{
    public class ProudSessionFactory : ISessionFactory
    {
        public ProudSession Create(uint hostId, IChannel channel)
        {
            return new ProudSession(hostId, channel);
        }
    }
}
