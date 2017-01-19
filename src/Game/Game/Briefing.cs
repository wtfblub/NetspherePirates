using System.Collections.Generic;
using System.IO;
using System.Linq;
using Netsphere.Game.GameRules;
using Netsphere.Game.Systems;
using Netsphere.Network;

// ReSharper disable once CheckNamespace
namespace Netsphere.Game
{
    internal class Briefing
    {
        public GameRuleBase GameRule { get; }

        public Briefing(GameRuleBase gameRule)
        {
            GameRule = gameRule;
        }

        protected virtual PlayerTeam GetWinnerTeam()
        {
            var teamMgr = GameRule.Room.TeamManager;
            var max = teamMgr.Values.Max(t => t.Score);
            var teams = teamMgr.Values.Where(t => t.Score == max).ToArray();

            if (teams.Length > 1)
            {
                var scores = new Dictionary<Team, uint>();
                foreach (var team in teams)
                {
                    var score = team.PlayersPlaying.Sum(plr => plr.RoomInfo.Stats.TotalScore);
                    scores.Add(team.Team, (uint)score);
                }

                max = scores.Values.Max();
                teams = teamMgr.Values.Where(t => scores[t.Team] == max).ToArray();
            }

            return teams[0];
        }

        protected virtual void WriteData(BinaryWriter w, bool isResult)
        {
            var teamMgr = GameRule.Room.TeamManager;
            var players = teamMgr.Players.ToArray();
            var spectators = teamMgr.Spectators.ToArray();

            w.Write((uint)GetWinnerTeam().Team);
            w.Write(teamMgr.Count);
            w.Write(players.Length);
            w.Write(spectators.Length);

            foreach (var team in teamMgr.Values)
            {
                w.WriteEnum(team.Team);
                w.Write(team.Score);
            }

            foreach (var plr in players)
                plr.RoomInfo.Stats.Serialize(w, isResult);

            foreach (var plr in spectators)
            {
                w.Write(plr.Account.Id);
                w.Write((ulong)0);
            }
        }

        public byte[] ToArray(bool isResult)
        {
            using (var w = new BinaryWriter(new MemoryStream()))
            {
                WriteData(w, isResult);
                return w.ToArray();
            }
        }
    }
}
