using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using Netsphere.Common.Configuration;
using Netsphere.Network.Message.GameRule;

namespace Netsphere.Server.Game.GameRules
{
    public class Practice : GameRuleBase
    {
        public override GameRule GameRule => GameRule.Practice;
        public override bool HasHalfTime => false;

        public Practice(GameRuleStateMachine stateMachine, IOptions<GameOptions> gameOptions)
            : base(stateMachine, gameOptions)
        {
        }

        public override void Initialize(Room room)
        {
            base.Initialize(room);
            Room.TeamManager.Add(TeamId.Alpha, 1, 0);
            Room.TeamManager.Add(TeamId.Beta, 0, 0);
        }

        public override void Cleanup()
        {
            base.Cleanup();
            Room.TeamManager.Remove(TeamId.Alpha);
            Room.TeamManager.Remove(TeamId.Beta);
        }

        protected override bool CanStartGame()
        {
            if (StateMachine.GameState != GameState.Waiting)
                return false;

            return true;
        }

        protected override bool HasEnoughPlayers()
        {
            return TeamManager.Values.All(team => team.PlayersPlaying.Any());
        }

        protected override PlayerScore CreateScore(Player plr)
        {
            return new PracticePlayerScore();
        }

        protected internal override BriefingTeam[] CreateBriefingTeams()
        {
            return GetTeams().ToArray();

            IEnumerable<BriefingTeam> GetTeams()
            {
                foreach (var team in TeamManager.Values)
                    yield return new BriefingTeam(team.Id, team.Score);
            }
        }

        protected override BriefingPlayer CreateBriefingPlayer(Player plr)
        {
            return new BriefingPlayerPractice(plr);
        }

        protected override (uint baseGain, uint bonusGain) CalculateExperienceGained(Player plr)
        {
            return (0, 0);
        }

        protected override (uint baseGain, uint bonusGain) CalculatePENGained(Player plr)
        {
            return (0, 0);
        }

        protected internal override void OnScoreKill(ScoreContext killer, ScoreContext assist, ScoreContext target,
            AttackAttribute attackAttribute)
        {
            killer.Score.Kills++;
            killer.Player.Team.Score++;
            SendScoreKill(killer, null, target, attackAttribute);

            if (killer.Score.Kills >= Room.Options.ScoreLimit)
                StateMachine.StartResult();
        }

        protected internal override void OnScoreMission(Player plr)
        {
            SendScoreMission(new ScoreContext(plr, 0), 1);
        }
    }

    public class BriefingPlayerPractice : BriefingPlayer
    {
        public uint Kills { get; set; }

        public BriefingPlayerPractice(Player plr)
        {
            AccountId = plr.Account.Id;
            Experience = plr.TotalExperience;
            TeamId = plr.Team.Id;
            State = plr.State;
            Mode = plr.Mode;
            IsReady = plr.IsReady;
            TotalScore = plr.Score.GetTotalScore();

            Kills = plr.Score.Kills;
        }

        public override void Serialize(BinaryWriter w)
        {
            base.Serialize(w);
            w.Write(Kills);
        }
    }

    public class PracticePlayerScore : PlayerScore
    {
        public override uint GetTotalScore()
        {
            return Kills;
        }
    }
}
