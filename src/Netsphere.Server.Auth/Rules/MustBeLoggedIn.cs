using System.Threading.Tasks;
using ProudNet;

namespace Netsphere.Server.Auth.Rules
{
    public class MustBeLoggedIn : IFirewallRule
    {
        public Task<bool> IsMessageAllowed(MessageContext context, object message)
        {
            return Task.FromResult(context.GetSession<Session>().Authenticated);
        }
    }
}
