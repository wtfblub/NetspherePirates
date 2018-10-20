using System.Threading.Tasks;

namespace ProudNet.Firewall
{
    public class MustBeUdpRelay : IFirewallRule
    {
        public Task<bool> IsMessageAllowed(MessageContext context, object message)
        {
            return Task.FromResult(context.Session.UdpEnabled);
        }
    }
}
