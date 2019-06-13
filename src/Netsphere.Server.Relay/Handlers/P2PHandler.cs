using System.Threading.Tasks;
using Foundatio.Messaging;
using Netsphere.Network.Message.P2P;
using ProudNet;

namespace Netsphere.Server.Relay.Handlers
{
    internal class P2PHandler : IHandle<PlayerSpawnReqMessage>
    {
        private readonly IMessageBus _messageBus;

        public P2PHandler(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, PlayerSpawnReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            return true;
        }
    }
}
