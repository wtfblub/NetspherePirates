using System.Threading.Tasks;
using ProudNet;

namespace Netsphere.Server.Game.Rules
{
    public class MustBeInChannel : IFirewallRule
    {
        public Task<bool> IsMessageAllowed(MessageContext context, object message)
        {
            var session = (Session)context.Session;
            return Task.FromResult(session.Player.Channel != null);
        }
    }
}
