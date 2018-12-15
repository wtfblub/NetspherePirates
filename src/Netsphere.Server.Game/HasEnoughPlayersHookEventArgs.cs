using System;

namespace Netsphere.Server.Game
{
    public class HasEnoughPlayersHookEventArgs : EventArgs
    {
        public GameRuleBase GameRule { get; }
        public bool? Result { get; set; }

        public HasEnoughPlayersHookEventArgs(GameRuleBase gameRule)
        {
            GameRule = gameRule;
        }
    }
}
