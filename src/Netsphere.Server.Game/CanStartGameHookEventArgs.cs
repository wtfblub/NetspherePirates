using System;

namespace Netsphere.Server.Game
{
    public class CanStartGameHookEventArgs : EventArgs
    {
        public GameRuleBase GameRule { get; }
        public bool? Result { get; set; }

        public CanStartGameHookEventArgs(GameRuleBase gameRule)
        {
            GameRule = gameRule;
        }
    }
}
