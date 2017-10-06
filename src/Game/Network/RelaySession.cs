using DotNetty.Transport.Channels;
using ProudNet;

namespace Netsphere.Network
{
    internal class RelaySession : ProudSession
    {
        public GameSession GameSession { get; set; }
        public Player Player => GameSession?.Player;

        public RelaySession(uint hostId, IChannel channel, ProudServer server)
            : base(hostId, channel, server)
        { }
    }

    internal class RelaySessionFactory : ISessionFactory
    {
        public ProudSession Create(uint hostId, IChannel channel, ProudServer server)
        {
            return new RelaySession(hostId, channel, server);
        }
    }
}
