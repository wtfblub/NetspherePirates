using System;
using Netsphere.Network.Data.GameRule;

namespace Netsphere.Server.Game
{
    public class RoomChangeHookEventArgs : EventArgs
    {
        public Room Room { get; }
        public ChangeRuleDto Options { get; }
        public RoomChangeRulesError Error { get; set; }

        public RoomChangeHookEventArgs(Room room, ChangeRuleDto options)
        {
            Room = room;
            Options = options;
            Error = RoomChangeRulesError.OK;
        }
    }
}
