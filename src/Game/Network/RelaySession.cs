using BlubLib.Network;
using ProudNet;

namespace Netsphere.Network
{
    internal class RelaySession : ProudSession
    {
        public GameSession GameSession { get; set; }
        public Player Player => GameSession?.Player;

        public RelaySession(IIOService service, IIOProcessor processor)
            : base(service, processor)
        { }
    }

    internal class RelaySessionFactory : ISessionFactory
    {
        public ISession GetSession(IIOService service, IIOProcessor processor)
        {
            return new RelaySession(service, processor);
        }
    }
}
