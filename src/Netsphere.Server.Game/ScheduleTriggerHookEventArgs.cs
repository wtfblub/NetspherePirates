using System;

namespace Netsphere.Server.Game
{
    public class ScheduleTriggerHookEventArgs : EventArgs
    {
        public GameRuleStateMachine StateMachine { get; }
        public GameRuleStateTrigger Trigger { get; set; }
        public TimeSpan Delay { get; set; }
        public bool Cancel { get; set; }

        public ScheduleTriggerHookEventArgs(GameRuleStateMachine stateMachine, GameRuleStateTrigger trigger, TimeSpan delay)
        {
            StateMachine = stateMachine;
            Trigger = trigger;
            Delay = delay;
        }
    }
}
