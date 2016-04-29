using BlubLib.Network;
using ProudNet;

namespace Netsphere.Network
{
    internal class RelaySession : ProudSession
    {
        public GameSession GameSession { get; set; }
        public Player Player => GameSession?.Player;

        public RelaySession(IService service, ITransport transport)
            : base(service, transport)
        { }
    }

    internal class RelaySessionFactory : ISessionFactory
    {
        public ISession GetSession(IService service, ITransport transport)
        {
            return new RelaySession(service, transport);
        }
    }
}
