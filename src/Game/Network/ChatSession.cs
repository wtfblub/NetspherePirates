using BlubLib.Network;
using ProudNet;

namespace Netsphere.Network
{
    internal class ChatSession : ProudSession
    {
        public GameSession GameSession { get; set; }
        public Player Player => GameSession.Player;

        public ChatSession(IIOService service, IIOProcessor processor)
            : base(service, processor)
        { }
    }

    internal class ChatSessionFactory : ISessionFactory
    {
        public ISession GetSession(IIOService service, IIOProcessor processor)
        {
            return new ChatSession(service, processor);
        }
    }
}
