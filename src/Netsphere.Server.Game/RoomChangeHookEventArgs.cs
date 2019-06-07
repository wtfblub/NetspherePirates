using System;
using Netsphere.Network.Data.GameRule;

namespace Netsphere.Server.Game
{
    public class RoomChangeHookEventArgs : EventArgs
    {
        public Room Room { get; }
        public ChangeRule2Dto Options { get; }
        public RoomChangeRulesError Error { get; set; }

        public RoomChangeHookEventArgs(Room room, ChangeRule2Dto options)
        {
            Room = room;
            Options = options;
            Error = RoomChangeRulesError.OK;
        }
    }
}
