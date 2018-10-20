using System.Threading.Tasks;

namespace ProudNet.Firewall
{
    public class MustBeInP2PGroup : IFirewallRule
    {
        public Task<bool> IsMessageAllowed(MessageContext context, object message)
        {
            return Task.FromResult(context.Session.P2PGroup != null);
        }
    }
}
