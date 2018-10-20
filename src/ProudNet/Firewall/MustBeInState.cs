using System.Threading.Tasks;

namespace ProudNet.Firewall
{
    internal class MustBeInState : IFirewallRule
    {
        private readonly SessionState _requiredState;

        public MustBeInState(SessionState requiredState)
        {
            _requiredState = requiredState;
        }

        public Task<bool> IsMessageAllowed(MessageContext context, object message)
        {
            return Task.FromResult(context.Session.State == _requiredState);
        }
    }
}
