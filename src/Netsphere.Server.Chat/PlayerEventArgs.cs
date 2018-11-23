using System;

namespace Netsphere.Server.Chat
{
    public class PlayerEventArgs : EventArgs
    {
        public Player Player { get; }

        public PlayerEventArgs(Player player)
        {
            Player = player;
        }
    }
}
