using System.Threading.Tasks;
using ProudNet;

namespace Netsphere.Server.Game.Rules
{
    public class MustNotHaveANickname : IFirewallRule
    {
        public Task<bool> IsMessageAllowed(MessageContext context, object message)
        {
            var session = (Session)context.Session;
            return Task.FromResult(session.Player != null && string.IsNullOrWhiteSpace(session.Player.Account.Nickname));
        }
    }
}
