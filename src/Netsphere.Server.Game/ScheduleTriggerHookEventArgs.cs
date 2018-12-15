using System;

namespace Netsphere.Server.Game
{
    public class ScheduleTriggerHookEventArgs : EventArgs
    {
        public GameRuleStateTrigger Trigger { get; set; }
        public TimeSpan Delay { get; set; }
        public bool Cancel { get; set; }

        public ScheduleTriggerHookEventArgs(GameRuleStateTrigger trigger, TimeSpan delay)
        {
            Trigger = trigger;
            Delay = delay;
        }
    }
}
