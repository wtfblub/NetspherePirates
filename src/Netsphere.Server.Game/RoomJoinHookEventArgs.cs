using System;

namespace Netsphere.Server.Game
{
    public class RoomJoinHookEventArgs : EventArgs
    {
        public Room Room { get; }
        public Player Player { get; }
        public RoomJoinError Error { get; set; }

        public RoomJoinHookEventArgs(Room room, Player player)
        {
            Room = room;
            Player = player;
            Error = RoomJoinError.OK;
        }
    }
}
