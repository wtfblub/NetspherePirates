using BlubLib.Network;
using ProudNet;

namespace Netsphere.Network
{
    internal class GameSession : ProudSession
    {
        public Player Player { get; set; }
        //public ChatSession ChatSession { get; set; }

        public GameSession(IService service, ITransport transport)
            : base(service, transport)
        { }
    }

    internal class GameSessionFactory : ISessionFactory
    {
        public ISession GetSession(IService service, ITransport transport)
        {
            return new GameSession(service, transport);
        }
    }
}
