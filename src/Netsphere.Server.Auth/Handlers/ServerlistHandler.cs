using System.Threading.Tasks;
using Netsphere.Network.Message.Auth;
using Netsphere.Server.Auth.Rules;
using ProudNet;

namespace Netsphere.Server.Auth.Handlers
{
    internal class ServerlistHandler : IHandle<CServerListReqMessage>
    {
        [Firewall(typeof(MustBeLoggedIn))]
        public async Task<bool> OnHandle(MessageContext context, CServerListReqMessage message)
        {
            var session = context.Session;

            await session.SendAsync(new SServerListAckMessage());
            return true;
        }
    }
}
