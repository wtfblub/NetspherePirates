namespace Netsphere.Server.Game
{
    public class RoomPlayerEventArgs : RoomEventArgs
    {
        public Player Player { get; }

        public RoomPlayerEventArgs(Room room, Player plr)
            : base(room)
        {
            Player = plr;
        }
    }
}
