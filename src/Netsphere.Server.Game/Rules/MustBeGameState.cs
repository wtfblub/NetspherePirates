using System.Threading.Tasks;
using ProudNet;

namespace Netsphere.Server.Game.Rules
{
    public class MustBeGameState : IFirewallRule
    {
        private readonly GameState _gameState;

        public MustBeGameState(GameState gameState)
        {
            _gameState = gameState;
        }

        public Task<bool> IsMessageAllowed(MessageContext context, object message)
        {
            var session = (Session)context.Session;
            var plr = session.Player;
            var room = plr.Room;

            return Task.FromResult(room.GameRule.StateMachine.GameState == _gameState);
        }
    }
}
