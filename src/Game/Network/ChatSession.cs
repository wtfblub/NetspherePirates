using DotNetty.Transport.Channels;
using ProudNet;

namespace Netsphere.Network
{
    internal class ChatSession : ProudSession
    {
        public GameSession GameSession { get; set; }
        public Player Player => GameSession.Player;

        public ChatSession(uint hostId, IChannel channel, ProudServer server)
            : base(hostId, channel, server)
        { }
    }

    internal class ChatSessionFactory : ISessionFactory
    {
        public ProudSession Create(uint hostId, IChannel channel, ProudServer server)
        {
            return new ChatSession(hostId, channel, server);
        }
    }
}
