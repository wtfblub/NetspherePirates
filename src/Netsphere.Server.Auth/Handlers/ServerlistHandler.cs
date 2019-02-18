using System.Threading.Tasks;
using Netsphere.Network.Message.Auth;
using Netsphere.Server.Auth.Rules;
using Netsphere.Server.Auth.Services;
using ProudNet;

namespace Netsphere.Server.Auth.Handlers
{
    internal class ServerlistHandler : IHandle<CServerListReqMessage>
    {
        private readonly ServerlistService _serverlistService;

        public ServerlistHandler(ServerlistService serverlistService)
        {
            _serverlistService = serverlistService;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, CServerListReqMessage message)
        {
            var session = context.Session;

            var servers = _serverlistService.GetServerList();
            session.Send(new SServerListAckMessage(servers));
            return true;
        }
    }
}
