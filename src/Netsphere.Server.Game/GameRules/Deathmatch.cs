using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using Netsphere.Common.Configuration;

namespace Netsphere.Server.Game.GameRules
{
    public class Deathmatch : GameRuleBase
    {
        private readonly DeathmatchOptions _options;

        public override GameRule GameRule => GameRule.Deathmatch;
        public override bool HasHalfTime => true;

        public Deathmatch(GameRuleStateMachine stateMachine, IOptions<GameOptions> gameOptions,
            IOptions<DeathmatchOptions> options)
            : base(stateMachine, gameOptions)
        {
            _options = options.Value;
        }

        public override void Initialize(Room room)
        {
            base.Initialize(room);

            var playersPerTeam = Room.Options.MatchKey.PlayerLimit / 2;
            var spectatorsPerTeam = Room.Options.MatchKey.SpectatorLimit / 2;
            Room.TeamManager.Add(TeamId.Alpha, playersPerTeam, spectatorsPerTeam);
            Room.TeamManager.Add(TeamId.Beta, playersPerTeam, spectatorsPerTeam);
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

            // Is atleast one player per team ready?
            var teams = TeamManager.Values;
            return teams.All(team => team.Players.Any(plr => plr.IsReady || Room.Master == plr));
        }

        protected override bool HasEnoughPlayers()
        {
            return TeamManager.Values.All(team => team.PlayersPlaying.Any());
        }

        protected override PlayerScore CreateScore(Player plr)
        {
            return new DeathmatchPlayerScore(_options);
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
            return new BriefingPlayerDeathmatch(plr);
        }

        protected override (uint baseGain, uint bonusGain) CalculateExperienceGained(Player plr)
        {
            var experienceRates = _options.ExperienceRates;
            var place = 1;

            var plrs = TeamManager.Players
                .Where(x => x.State == PlayerState.Waiting && x.Mode == PlayerGameMode.Normal)
                .ToArray();

            foreach (var x in plrs.OrderByDescending(x => x.Score.GetTotalScore()))
            {
                if (x == plr)
                    break;

                place++;
                if (place > 3)
                    break;
            }

            var rankingBonus = 0f;
            switch (place)
            {
                case 1:
                    rankingBonus = experienceRates.FirstPlaceBonus;
                    break;

                case 2:
                    rankingBonus = experienceRates.SecondPlaceBonus;
                    break;

                case 3:
                    rankingBonus = experienceRates.ThirdPlaceBonus;
                    break;
            }

            var experienceGained = (uint)(plr.Score.GetTotalScore() * experienceRates.ScoreFactor +
                                          rankingBonus +
                                          plrs.Length * experienceRates.PlayerCountFactor +
                                          plr.GetCurrentPlayTime().TotalMinutes * experienceRates.ExperiencePerMinute);

            return (experienceGained, 0);
        }

        protected override (uint baseGain, uint bonusGain) CalculatePENGained(Player plr)
        {
            return (0, 0);
        }

        protected internal override void OnScoreKill(ScoreContext killer, ScoreContext assist, ScoreContext target,
            AttackAttribute attackAttribute)
        {
            if (target.IsSentry)
            {
                SendScoreKill(killer, assist, target, attackAttribute);
                return;
            }

            killer.Score.Kills++;
            target.Score.Deaths++;

            if (assist != null)
                assist.Score.KillAssists++;

            SendScoreKill(killer, assist, target, attackAttribute);

            killer.Player.Team.Score++;
            if (killer.Player.Team.Score == Room.Options.ScoreLimit / 2)
                StateMachine.StartHalfTime();
            else if (killer.Player.Team.Score == Room.Options.ScoreLimit)
                StateMachine.StartResult();
        }

        protected internal override void OnScoreSuicide(Player plr)
        {
            plr.Score.Deaths++;
            plr.Score.Suicides++;
            SendScoreSuicide(plr);
        }

        protected internal override void OnScoreTeamKill(ScoreContext killer, ScoreContext target,
            AttackAttribute attackAttribute)
        {
            if (!target.IsSentry)
                target.Score.Deaths++;

            SendScoreTeamKill(killer, target, attackAttribute);
        }

        protected internal override void OnScoreHeal(Player plr)
        {
            plr.Score.HealAssists++;
            SendScoreHeal(plr);
        }
    }

    public class BriefingPlayerDeathmatch : BriefingPlayer
    {
        public uint Kills { get; set; }
        public uint KillAssists { get; set; }
        public uint HealAssists { get; set; }
        public uint Deaths { get; set; }

        public BriefingPlayerDeathmatch(Player plr)
        {
            AccountId = plr.Account.Id;
            Experience = plr.TotalExperience;
            TeamId = plr.Team.Id;
            State = plr.State;
            Mode = plr.Mode;
            IsReady = plr.IsReady;
            TotalScore = plr.Score.GetTotalScore();

            Kills = plr.Score.Kills;
            KillAssists = plr.Score.KillAssists;
            HealAssists = plr.Score.HealAssists;
            Deaths = plr.Score.Deaths;
        }

        public override void Serialize(BinaryWriter w)
        {
            base.Serialize(w);

            w.Write(Kills);
            w.Write(KillAssists);
            w.Write(HealAssists);
            w.Write(Deaths);
            w.Write(0);
            w.Write(0);
            w.Write(0);
        }
    }

    public class DeathmatchPlayerScore : PlayerScore
    {
        private readonly DeathmatchOptions _options;

        public DeathmatchPlayerScore(DeathmatchOptions options)
        {
            _options = options;
        }

        public override uint GetTotalScore()
        {
            return (uint)(Kills * _options.PointsPerKill +
                          KillAssists * _options.PointsPerKillAssist +
                          HealAssists * _options.PointsPerHealAssist +
                          Deaths * _options.PointsPerDeath);
        }
    }
}
