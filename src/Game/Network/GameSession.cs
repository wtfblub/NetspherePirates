using BlubLib.Network;
using ProudNet;

namespace Netsphere.Network
{
    internal class GameSession : ProudSession
    {
        public Player Player { get; set; }
        //public ChatSession ChatSession { get; set; }

        public GameSession(IIOService service, IIOProcessor processor)
            : base(service, processor)
        { }
    }

    internal class GameSessionFactory : ISessionFactory
    {
        public ISession GetSession(IIOService service, IIOProcessor processor)
        {
            return new GameSession(service, processor);
        }
    }
}
