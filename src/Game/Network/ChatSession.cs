using BlubLib.Network;
using ProudNet;

namespace Netsphere.Network
{
    internal class ChatSession : ProudSession
    {
        public GameSession GameSession { get; set; }
        public Player Player => GameSession.Player;

        public ChatSession(IService service, ITransport transport)
            : base(service, transport)
        { }
    }

    internal class ChatSessionFactory : ISessionFactory
    {
        public ISession GetSession(IService service, ITransport transport)
        {
            return new ChatSession(service, transport);
        }
    }
}
