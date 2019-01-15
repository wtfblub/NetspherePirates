using System;

namespace Netsphere.Server.Game
{
    public class NicknameEventArgs : EventArgs
    {
        public Player Player { get; }
        public string Nickname { get; }

        public NicknameEventArgs(Player plr, string nickname)
        {
            Player = plr;
            Nickname = nickname;
        }
    }
}
