using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Netsphere.Common.Configuration;
using Netsphere.Network.Message.GameRule;

namespace Netsphere.Server.Game
{
    public abstract partial class GameRuleBase
    {
        private static readonly EventPipeline<CanStartGameHookEventArgs> s_preCanStartGameHook =
            new EventPipeline<CanStartGameHookEventArgs>();
        private static readonly EventPipeline<HasEnoughPlayersHookEventArgs> s_preHasEnoughPlayersHook =
            new EventPipeline<HasEnoughPlayersHookEventArgs>();

        private readonly GameOptions _gameOptions;

        public abstract GameRule GameRule { get; }
        public abstract bool HasHalfTime { get; }

        public Room Room { get; private set; }
        public TeamManager TeamManager => Room.TeamManager;
        public GameRuleStateMachine StateMachine { get; }

        public static event EventPipeline<CanStartGameHookEventArgs>.SubscriberDelegate CanStartGameHook
        {
            add => s_preCanStartGameHook.Subscribe(value);
            remove => s_preCanStartGameHook.Unsubscribe(value);
        }
        public static event EventPipeline<HasEnoughPlayersHookEventArgs>.SubscriberDelegate HasEnoughPlayersHook
        {
            add => s_preHasEnoughPlayersHook.Subscribe(value);
            remove => s_preHasEnoughPlayersHook.Unsubscribe(value);
        }

        protected GameRuleBase(GameRuleStateMachine stateMachine, IOptions<GameOptions> gameOptions)
        {
            StateMachine = stateMachine;
            _gameOptions = gameOptions.Value;
        }

        public virtual void Initialize(Room room)
        {
            Room = room;
            StateMachine.Initialize(this, _CanStartGame, HasHalfTime);
            Room.PlayerJoining += OnPlayerJoining;
            Room.PlayerLeft += OnPlayerLeft;

            foreach (var plr in Room.Players.Values)
                plr.Score = CreateScore(plr);
        }

        public virtual void Cleanup()
        {
            Room.PlayerJoining -= OnPlayerJoining;
            Room.PlayerLeft -= OnPlayerLeft;
        }

        protected internal virtual BriefingPlayer[] CreateBriefingPlayers()
        {
            return Room.Players.Values.Select(CreateBriefingPlayer).ToArray();
        }

        protected internal virtual void OnResult()
        {
            var briefing = Room.GetBriefing();
            briefing.WinnerTeam = GetWinnerTeam().Id;

            // Result calculations exp, pen, item durability
            foreach (var plr in TeamManager.PlayersPlaying)
            {
                var (expGain, bonusExpGain) = CalculateExperienceGained(plr);
                var (penGain, bonusPENGain) = CalculatePENGained(plr);

                var briefingPlayer = briefing.Players.First(x => x.AccountId == plr.Account.Id);
                briefingPlayer.ExperienceGained = expGain;
                briefingPlayer.BonusExperienceGained = bonusExpGain;
                briefingPlayer.PENGained = penGain;
                briefingPlayer.BonusPENGained = bonusPENGain;

                if (expGain > 0)
                {
                    var levelUp = plr.GainExperience(briefingPlayer.ExperienceGained + briefingPlayer.BonusExperienceGained);
                    briefingPlayer.LevelUp = levelUp;
                }

                if (penGain > 0)
                {
                    plr.PEN += penGain + bonusPENGain;
                    plr.SendMoneyUpdate();
                }

                // Durability loss based on play time
                foreach (var character in plr.CharacterManager)
                {
                    if (plr.CharacterStartPlayTime[character.Slot] == default)
                        continue;

                    var playTime = plr.GetCharacterPlayTime(character.Slot);
                    var loss = (int)playTime.TotalMinutes * _gameOptions.DurabilityLossPerMinute;
                    loss += (int)plr.Score.Deaths * _gameOptions.DurabilityLossPerDeath;

                    foreach (var item in character.Weapons.GetItems()
                        .Where(item => item != null && item.Durability != -1))
                    {
                        item.LoseDurability(loss);
                    }

                    foreach (var item in character.Costumes.GetItems()
                        .Where(item => item != null && item.Durability != -1))
                    {
                        item.LoseDurability(loss);
                    }

                    foreach (var item in character.Skills.GetItems()
                        .Where(item => item != null && item.Durability != -1))
                    {
                        item.LoseDurability(loss);
                    }
                }
            }

            Room.Broadcast(new GameBriefingInfoAckMessage(true, false, briefing.GetData()));
        }

        protected internal virtual Team GetWinnerTeam()
        {
            var max = TeamManager.Values.Max(x => x.Score);
            var teams = TeamManager.Values.Where(x => x.Score == max).ToArray();

            // If team score is tied get team with highest player score
            if (teams.Length > 1)
            {
                var scores = new Dictionary<TeamId, uint>();
                foreach (var team in teams)
                {
                    var score = team.PlayersPlaying.Sum(x => x.Score.GetTotalScore());
                    scores.Add(team.Id, (uint)score);
                }

                max = scores.Values.Max();
                teams = TeamManager.Values.Where(x => scores[x.Id] == max).ToArray();
            }

            return teams[0];
        }

        protected abstract bool CanStartGame();

        protected abstract bool HasEnoughPlayers();

        protected internal virtual Briefing CreateBriefing()
        {
            return new Briefing();
        }

        protected abstract PlayerScore CreateScore(Player plr);

        protected internal virtual BriefingTeam[] CreateBriefingTeams()
        {
            return GetTeams().ToArray();

            IEnumerable<BriefingTeam> GetTeams()
            {
                foreach (var team in TeamManager.Values)
                    yield return CreateBriefingTeam(team);
            }
        }

        protected internal virtual BriefingTeam CreateBriefingTeam(Team team)
        {
            return new BriefingTeam(team.Id, team.Score);
        }

        protected abstract BriefingPlayer CreateBriefingPlayer(Player plr);

        protected abstract (uint baseGain, uint bonusGain) CalculateExperienceGained(Player plr);

        protected abstract (uint baseGain, uint bonusGain) CalculatePENGained(Player plr);

        private bool _CanStartGame()
        {
            var eventArgs = new CanStartGameHookEventArgs(this);
            s_preCanStartGameHook.Invoke(eventArgs);
            return eventArgs.Result ?? CanStartGame();
        }

        private bool _HasEnoughPlayers()
        {
            var eventArgs = new HasEnoughPlayersHookEventArgs(this);
            s_preHasEnoughPlayersHook.Invoke(eventArgs);
            return eventArgs.Result ?? HasEnoughPlayers();
        }

        private void OnPlayerJoining(object _, RoomPlayerEventArgs e)
        {
            e.Player.Score = CreateScore(e.Player);
        }

        private void OnPlayerLeft(object _, RoomPlayerEventArgs e)
        {
            if (StateMachine.GameState == GameState.Playing && !_HasEnoughPlayers())
                StateMachine.StartResult();
        }
    }
}
