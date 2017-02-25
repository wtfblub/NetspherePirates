using DotNetty.Transport.Channels;
using ProudNet;

namespace Netsphere.Network
{
    internal class RelaySession : ProudSession
    {
        public GameSession GameSession { get; set; }
        public Player Player => GameSession?.Player;

        public RelaySession(uint hostId, IChannel channel)
            : base(hostId, channel)
        { }
    }

    internal class RelaySessionFactory : ISessionFactory
    {
        public ProudSession Create(uint hostId, IChannel channel)
        {
            return new RelaySession(hostId, channel);
        }
    }
}
