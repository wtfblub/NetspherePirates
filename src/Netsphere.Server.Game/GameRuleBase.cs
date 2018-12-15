using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Netsphere.Common.Configuration;
using Netsphere.Network.Message.GameRule;

namespace Netsphere.Server.Game
{
    public abstract partial class GameRuleBase
    {
        private readonly GameOptions _gameOptions;
        private readonly EventPipeline<CanStartGameHookEventArgs> _preCanStartGameHook;
        private readonly EventPipeline<HasEnoughPlayersHookEventArgs> _preHasEnoughPlayersHook;

        public abstract GameRule GameRule { get; }
        protected abstract bool HasHalfTime { get; }

        public Room Room { get; private set; }
        public TeamManager TeamManager => Room.TeamManager;
        public GameRuleStateMachine StateMachine { get; }

        public event EventPipeline<CanStartGameHookEventArgs>.SubscriberDelegate CanStartGameHook
        {
            add => _preCanStartGameHook.Subscribe(value);
            remove => _preCanStartGameHook.Unsubscribe(value);
        }
        public event EventPipeline<HasEnoughPlayersHookEventArgs>.SubscriberDelegate HasEnoughPlayersHook
        {
            add => _preHasEnoughPlayersHook.Subscribe(value);
            remove => _preHasEnoughPlayersHook.Unsubscribe(value);
        }

        protected GameRuleBase(GameRuleStateMachine stateMachine, IOptions<GameOptions> gameOptions)
        {
            StateMachine = stateMachine;
            _gameOptions = gameOptions.Value;
            _preCanStartGameHook = new EventPipeline<CanStartGameHookEventArgs>();
            _preHasEnoughPlayersHook = new EventPipeline<HasEnoughPlayersHookEventArgs>();
        }

        public virtual void Initialize(Room room)
        {
            Room = room;
            StateMachine.Initialize(this, _CanStartGame, HasHalfTime);
            Room.PlayerJoining += OnPlayerJoining;
            Room.PlayerLeft += OnPlayerLeft;

            foreach (var plr in Room.Players.Values)
                plr.Score = CreateScore();
        }

        public virtual void Cleanup()
        {
            Room.PlayerJoining -= OnPlayerJoining;
            Room.PlayerLeft -= OnPlayerLeft;
        }

        internal BriefingPlayer[] CreateBriefingPlayers()
        {
            return Room.Players.Values.Select(CreateBriefingPlayer).ToArray();
        }

        protected internal virtual void OnResult()
        {
            var teams = CreateBriefingTeams();
            var players = CreateBriefingPlayers();

            foreach (var plr in TeamManager.PlayersPlaying)
            {
                var (expGain, bonusExpGain) = CalculateExperienceGained(plr);
                var (penGain, bonusPENGain) = CalculatePENGained(plr);

                var briefingPlayer = players.First(x => x.AccountId == plr.Account.Id);
                briefingPlayer.ExperienceGained = expGain;
                briefingPlayer.BonusExperienceGained = bonusExpGain;
                briefingPlayer.PENGained = penGain;
                briefingPlayer.BonusPENGained = bonusPENGain;

                var levelUp = plr.GainExperience(briefingPlayer.ExperienceGained + briefingPlayer.BonusExperienceGained);
                briefingPlayer.LevelUp = levelUp;

                plr.PEN += penGain + bonusPENGain;
                plr.SendMoneyUpdate();

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

            var briefing = new Briefing
            {
                WinnerTeam = GetWinnerTeam().Id,
                Teams = teams,
                Players = players,
                Spectators = TeamManager.Spectators.Select(x => x.Account.Id).ToArray()
            };

            Room.Broadcast(new SBriefingAckMessage(true, false, briefing.Serialize()));
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

        protected abstract PlayerScore CreateScore();

        protected internal abstract BriefingTeam[] CreateBriefingTeams();

        protected abstract BriefingPlayer CreateBriefingPlayer(Player plr);

        protected abstract (uint baseGain, uint bonusGain) CalculateExperienceGained(Player plr);

        protected abstract (uint baseGain, uint bonusGain) CalculatePENGained(Player plr);

        private bool _CanStartGame()
        {
            var eventArgs = new CanStartGameHookEventArgs(this);
            _preCanStartGameHook.Invoke(eventArgs);
            return eventArgs.Result ?? CanStartGame();
        }

        private bool _HasEnoughPlayers()
        {
            var eventArgs = new HasEnoughPlayersHookEventArgs(this);
            _preHasEnoughPlayersHook.Invoke(eventArgs);
            return eventArgs.Result ?? HasEnoughPlayers();
        }

        private void OnPlayerJoining(object _, RoomPlayerEventArgs e)
        {
            e.Player.Score = CreateScore();
        }

        private void OnPlayerLeft(object _, RoomPlayerEventArgs e)
        {
            if (StateMachine.GameState == GameState.Playing && !_HasEnoughPlayers())
                StateMachine.StartResult();
        }
    }
}
