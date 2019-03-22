using System;

namespace Netsphere.Server.Relay
{
    public class RoomEventArgs : EventArgs
    {
        public Room Room { get; }

        public RoomEventArgs(Room room)
        {
            Room = room;
        }
    }
}
