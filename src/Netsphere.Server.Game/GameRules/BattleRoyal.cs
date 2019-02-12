using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using Netsphere.Common.Configuration;
using Netsphere.Network.Message.GameRule;

namespace Netsphere.Server.Game.GameRules
{
    public class BattleRoyal : GameRuleBase
    {
        private readonly BattleRoyalOptions _options;
        private Player _firstPlace;

        public override GameRule GameRule => GameRule.BattleRoyal;
        public override bool HasHalfTime => false;
        public virtual Player FirstPlace
        {
            get => _firstPlace;
            protected set
            {
                if (_firstPlace == value)
                    return;

                _firstPlace = value;
                if (StateMachine.GameState == GameState.Playing)
                    Room.Broadcast(new SGameRuleChangeTheFirstAckMessage(_firstPlace?.Account.Id ?? 0));
            }
        }

        public BattleRoyal(GameRuleStateMachine stateMachine, IOptions<GameOptions> gameOptions,
            IOptions<BattleRoyalOptions> options)
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

            // Is atleast one player ready?
            var teams = TeamManager.Values;
            return teams.Sum(team => team.Players.Count(plr => plr.IsReady)) > 0;
        }

        protected override bool HasEnoughPlayers()
        {
            // We need at least 2 players
            return TeamManager.Values.Sum(team => team.PlayersPlaying.Count()) > 1;
        }

        protected internal override Team GetWinnerTeam()
        {
            return TeamManager.Values.First();
        }

        protected override PlayerScore CreateScore(Player plr)
        {
            return new BattleRoyalPlayerScore(_options);
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
            return new BriefingPlayerBattleRoyal(plr);
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

            if (target.Player == FirstPlace)
            {
                GetScore(killer).BonusKills++;
                if (assist != null)
                    GetScore(assist).BonusKillAssists++;
            }
            else
            {
                killer.Score.Kills++;
                if (assist != null)
                    assist.Score.KillAssists++;
            }

            target.Score.Deaths++;
            SendScoreKill(killer, assist, target, attackAttribute);
            FirstPlace = GetFirstPlace();

            if (FirstPlace.Score.GetTotalScore() >= Room.Options.ScoreLimit)
                StateMachine.StartResult();
        }

        protected internal override void OnScoreSuicide(Player plr)
        {
            plr.Score.Deaths++;
            plr.Score.Suicides++;
            SendScoreSuicide(plr);
        }

        private Player GetFirstPlace()
        {
            return TeamManager.PlayersPlaying
                .Aggregate((highestPlayer, player) =>
                    highestPlayer == null || player.Score.GetTotalScore() > highestPlayer.Score.GetTotalScore()
                        ? player
                        : highestPlayer);
        }

        private static BattleRoyalPlayerScore GetScore(ScoreContext plr)
        {
            return (BattleRoyalPlayerScore)plr.Score;
        }
    }

    public class BriefingPlayerBattleRoyal : BriefingPlayer
    {
        public uint Kills { get; set; }
        public uint KillAssists { get; set; }
        public uint BonusKills { get; set; }
        public uint BonusKillAssists { get; set; }

        public BriefingPlayerBattleRoyal(Player plr)
        {
            AccountId = plr.Account.Id;
            Experience = plr.TotalExperience;
            TeamId = plr.Team.Id;
            State = plr.State;
            Mode = plr.Mode;
            IsReady = plr.IsReady;
            TotalScore = plr.Score.GetTotalScore();

            var score = (BattleRoyalPlayerScore)plr.Score;
            Kills = score.Kills;
            KillAssists = score.KillAssists;
            BonusKills = score.BonusKills;
            BonusKillAssists = score.BonusKillAssists;
        }

        public override void Serialize(BinaryWriter w)
        {
            base.Serialize(w);

            w.Write(Kills);
            w.Write(KillAssists);
            w.Write(BonusKills);
            w.Write(BonusKillAssists);
            w.Write(0);
            w.Write(0);
            w.Write(0);
            w.Write(0);
            w.Write(0);
            w.Write(0);
            w.Write(0);
        }
    }

    public class BattleRoyalPlayerScore : PlayerScore
    {
        private readonly BattleRoyalOptions _options;

        public uint BonusKills { get; set; }
        public uint BonusKillAssists { get; set; }

        public BattleRoyalPlayerScore(BattleRoyalOptions options)
        {
            _options = options;
        }

        public override uint GetTotalScore()
        {
            return (uint)(Kills * _options.PointsPerKill +
                          KillAssists * _options.PointsPerKillAssist +
                          BonusKills * _options.PointsPerBonusKill +
                          BonusKillAssists * _options.PointsPerBonusAssist +
                          Deaths * _options.PointsPerDeath);
        }

        public override void Reset()
        {
            base.Reset();
            BonusKills = 0;
            BonusKillAssists = 0;
        }
    }
}
