using System;

namespace Netsphere.Server.Game
{
    public class PlayerTeamChangedEventArgs : EventArgs
    {
        public Player Player { get; }
        public Team SourceTeam { get; }
        public Team TargetTeam { get; }

        public PlayerTeamChangedEventArgs(Player player, Team sourceTeam, Team targetTeam)
        {
            Player = player;
            SourceTeam = sourceTeam;
            TargetTeam = targetTeam;
        }
    }
}
