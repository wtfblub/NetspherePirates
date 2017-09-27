using DotNetty.Transport.Channels;
using ProudNet;

namespace Netsphere.Network
{
    internal class GameSession : ProudSession
    {
        public Player Player { get; set; }
        //public ChatSession ChatSession { get; set; }

        public GameSession(uint hostId, IChannel channel, ProudServer server)
            : base(hostId, channel, server)
        { }
    }

    internal class GameSessionFactory : ISessionFactory
    {
        public ProudSession Create(uint hostId, IChannel channel, ProudServer server)
        {
            return new GameSession(hostId, channel, server);
        }
    }
}
