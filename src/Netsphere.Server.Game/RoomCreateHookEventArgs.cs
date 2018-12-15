using System;

namespace Netsphere.Server.Game
{
    public class RoomCreateHookEventArgs : EventArgs
    {
        public RoomManager RoomManager { get; }
        public RoomCreationOptions Options { get; }
        public RoomCreateError Error { get; set; }

        public RoomCreateHookEventArgs(RoomManager roomManager, RoomCreationOptions options)
        {
            RoomManager = roomManager;
            Options = options;
            Error = RoomCreateError.OK;
        }
    }
}
