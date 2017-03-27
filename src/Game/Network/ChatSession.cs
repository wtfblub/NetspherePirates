using DotNetty.Transport.Channels;
using ProudNet;

namespace Netsphere.Network
{
    internal class ChatSession : ProudSession
    {
        public GameSession GameSession { get; set; }
        public Player Player => GameSession.Player;

        public ChatSession(uint hostId, IChannel channel)
            : base(hostId, channel)
        { }
    }

    internal class ChatSessionFactory : ISessionFactory
    {
        public ProudSession Create(uint hostId, IChannel channel)
        {
            return new ChatSession(hostId, channel);
        }
    }
}
