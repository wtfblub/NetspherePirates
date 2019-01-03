using System.Threading.Tasks;
using ProudNet;

namespace Netsphere.Server.Game.Rules
{
    public class MustBeTimeState : IFirewallRule
    {
        private readonly GameTimeState _state;

        public MustBeTimeState(GameTimeState state)
        {
            _state = state;
        }

        public Task<bool> IsMessageAllowed(MessageContext context, object message)
        {
            var session = (Session)context.Session;
            var plr = session.Player;
            var room = plr.Room;

            return Task.FromResult(room.GameRule.StateMachine.TimeState == _state);
        }
    }
}
