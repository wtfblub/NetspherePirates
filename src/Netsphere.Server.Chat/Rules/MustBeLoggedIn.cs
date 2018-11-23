using System.Threading.Tasks;
using ProudNet;

namespace Netsphere.Server.Chat.Rules
{
    public class MustBeLoggedIn : IFirewallRule
    {
        public Task<bool> IsMessageAllowed(MessageContext context, object message)
        {
            var session = (Session)context.Session;
            return Task.FromResult(session.Player != null);
        }
    }
}
