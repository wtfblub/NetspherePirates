using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BlubLib.Collections.Concurrent;
using ExpressMapper.Extensions;
using Foundatio.Messaging;
using Logging;
using Netsphere.Common;
using Netsphere.Common.Messaging;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;
using Netsphere.Server.Game.Data;
using Netsphere.Server.Game.Services;
using ProudNet.Hosting.Services;

namespace Netsphere.Server.Game
{
    public class Room
    {
        private ILogger _logger;
        private readonly GameRuleManager _gameRuleManager;
        private readonly GameDataService _gameDataService;
        private readonly ISchedulerService _schedulerService;
        private readonly IMessageBus _messageBus;
        private readonly ConcurrentDictionary<ulong, Player> _players;
        private readonly ConcurrentDictionary<ulong, object> _kickedPlayers;
        private readonly CounterRecycler _idRecycler;

        public RoomManager RoomManager { get; internal set; }
        public IReadOnlyDictionary<ulong, Player> Players => _players;
        public uint Id { get; internal set; }
        public RoomCreationOptions Options { get; internal set; }
        public DateTime TimeCreated { get; }
        public Player Master { get; private set; }
        public Player Host { get; private set; }
        public bool IsChangingRules { get; private set; }
        public TeamManager TeamManager { get; }
        public MapInfo Map { get; private set; }
        public GameRuleBase GameRule { get; private set; }

        public event EventHandler<RoomPlayerEventArgs> PlayerJoining;
        public event EventHandler<RoomPlayerEventArgs> PlayerJoined;
        public event EventHandler<RoomPlayerEventArgs> PlayerLeft;
        public event EventHandler<RoomEventArgs> OptionsChanged;

        protected virtual void OnPlayerJoining(Player plr)
        {
            PlayerJoining?.Invoke(this, new RoomPlayerEventArgs(this, plr));
            RoomManager.Channel.Broadcast(new SChangeGameRoomAckMessage(this.Map<Room, RoomDto>()));
            _messageBus.PublishAsync(new PlayerUpdateMessage(plr.Account.Id, plr.TotalExperience, Id, TeamId.Neutral));
        }

        internal virtual void OnPlayerJoined(Player plr)
        {
            PlayerJoined?.Invoke(this, new RoomPlayerEventArgs(this, plr));
            _messageBus.PublishAsync(new PlayerUpdateMessage(
                plr.Account.Id, plr.TotalExperience, Id, plr.Team?.Id ?? TeamId.Neutral));
        }

        protected virtual void OnPlayerLeft(Player plr)
        {
            PlayerLeft?.Invoke(this, new RoomPlayerEventArgs(this, plr));
            RoomManager.Channel.Broadcast(new SChangeGameRoomAckMessage(this.Map<Room, RoomDto>()));
            _messageBus.PublishAsync(new PlayerUpdateMessage(plr.Account.Id, plr.TotalExperience, 0, TeamId.Neutral));
        }

        protected virtual void OnOptionsChanged()
        {
            OptionsChanged?.Invoke(this, new RoomEventArgs(this));
            Broadcast(new SChangeRuleAckMessage(Options.Map<RoomCreationOptions, ChangeRuleDto>()));
        }

        public Room(ILogger<Room> logger, GameRuleManager gameRuleManager, GameDataService gameDataService,
            ISchedulerService schedulerService, IMessageBus messageBus)
        {
            _logger = logger;
            _gameRuleManager = gameRuleManager;
            _gameDataService = gameDataService;
            _schedulerService = schedulerService;
            _messageBus = messageBus;
            TimeCreated = DateTime.Now;
            _players = new ConcurrentDictionary<ulong, Player>();
            _kickedPlayers = new ConcurrentDictionary<ulong, object>();
            _idRecycler = new CounterRecycler(3);
            TeamManager = new TeamManager(this);
        }

        internal void Initialize(RoomManager roomManager, uint id, RoomCreationOptions options)
        {
            RoomManager = roomManager;
            Id = id;
            Options = options;
            Map = _gameDataService.Maps.First(x => x.Id == options.MatchKey.Map);
            GameRule = Options.GameRuleResolver.Resolve(this);
            GameRule.Initialize(this);
            GameRule.StateMachine.GameStateChanged += OnGameStateChanged;
            TeamManager.PlayerTeamChanged += OnPlayerTeamChanged;

            _logger = _logger.ForContext(
                ("ChannelId", RoomManager.Channel.Id),
                ("RoomId", Id));
        }

        public RoomJoinError Join(Player plr)
        {
            if (plr.Room != null)
                return RoomJoinError.AlreadyInRoom;

            if (_players.Count >= Options.MatchKey.PlayerLimit + Options.MatchKey.SpectatorLimit)
                return RoomJoinError.RoomFull;

            if (_kickedPlayers.ContainsKey(plr.Account.Id))
                return RoomJoinError.KickedPreviously;

            if (IsChangingRules)
                return RoomJoinError.ChangingRules;

            plr.Slot = (byte)_idRecycler.GetId();
            plr.State = PlayerState.Lobby;
            plr.IsReady = false;

            if (TeamManager.Any(x => x.Value.Players.Count() < x.Value.PlayerLimit))
            {
                plr.Mode = PlayerGameMode.Normal;
            }
            else
            {
                if (TeamManager.Any(x => x.Value.Spectators.Count() < x.Value.SpectatorLimit))
                    plr.Mode = PlayerGameMode.Spectate;
                else
                    return RoomJoinError.RoomFull;
            }

            if (TeamManager.Join(plr) != TeamJoinError.OK)
                return RoomJoinError.RoomFull;

            _players.TryAdd(plr.Account.Id, plr);
            plr.Room = this;
            plr.IsConnectingToRoom = true;
            plr.PeerId = null;

            if (Master == null)
            {
                ChangeMaster(plr);
                ChangeHost(plr);
            }

            Broadcast(new SEnteredPlayerAckMessage(plr.Map<Player, RoomPlayerDto>()));
            plr.Session.Send(new SSuccessEnterRoomAckMessage(this.Map<Room, EnterRoomInfoDto>()));
            plr.Session.Send(new SIdsInfoAckMessage(0, plr.Slot));
            plr.Session.Send(new SEnteredPlayerListAckMessage(
                _players.Values.Select(x => x.Map<Player, RoomPlayerDto>()).ToArray()));
            OnPlayerJoining(plr);

            return RoomJoinError.OK;
        }

        public void Leave(Player plr, RoomLeaveReason roomLeaveReason = RoomLeaveReason.Left)
        {
            if (plr.Room != this)
                return;

            Broadcast(new Network.Message.GameRule.SLeavePlayerAckMessage(plr.Account.Id, plr.Account.Nickname, roomLeaveReason));

            if (roomLeaveReason == RoomLeaveReason.Kicked ||
                roomLeaveReason == RoomLeaveReason.ModeratorKick ||
                roomLeaveReason == RoomLeaveReason.VoteKick)
                _kickedPlayers.TryAdd(plr.Account.Id, null);

            plr.Team.Leave(plr);
            _players.Remove(plr.Account.Id);
            _idRecycler.Return(plr.Slot);
            plr.Room = null;
            plr.PeerId = null;
            plr.Session.Send(new Network.Message.Game.SLeavePlayerAckMessage(plr.Account.Id));

            OnPlayerLeft(plr);

            if (_players.Count > 0)
            {
                if (Master == plr)
                {
                    // Prioritize players that are ready
                    // This makes it possible for players to give master to a specific player
                    var newMaster = GetPlayerWithLowestPing(Players.Values.Where(x => x.IsReady))
                                    ?? GetPlayerWithLowestPing();
                    ChangeMaster(newMaster);
                    ChangeHost(newMaster);
                }
            }
            else
            {
                RoomManager.Remove(this);
            }
        }

        public void ChangeMaster(Player plr)
        {
            if (plr.Room != this || Master == plr)
                return;

            Master = plr;
            Master.IsReady = false;
            Broadcast(new SChangeMasterAckMessage(Master.Account.Id));
        }

        public void ChangeHost(Player plr)
        {
            if (plr.Room != this || Host == plr)
                return;

            _logger.Debug("Changing host to {Nickname} - Ping:{Ping} ms", plr.Account.Nickname, plr.Session.UnreliablePing);
            Host = plr;
            Broadcast(new SChangeRefeReeAckMessage(Host.Account.Id));
        }

        public RoomChangeRulesError ChangeRules(ChangeRuleDto options)
        {
            if (IsChangingRules)
                return RoomChangeRulesError.AlreadyChangingRules;

            if (!_gameRuleManager.HasGameRule(options.MatchKey.GameRule))
                return RoomChangeRulesError.InvalidGameRule;

            var map = _gameDataService.Maps.FirstOrDefault(x => x.Id == options.MatchKey.Map);
            if (map == null)
                return RoomChangeRulesError.InvalidMap;

            if (!map.GameRules.Contains(options.MatchKey.GameRule))
                return RoomChangeRulesError.InvalidGameRule;

            if (options.MatchKey.PlayerLimit < Players.Count)
                return RoomChangeRulesError.PlayerLimitTooLow;

            IsChangingRules = true;
            _schedulerService.ScheduleAsync(OnChangeRules, this, null, TimeSpan.FromSeconds(5));

            Options.Name = options.Name;
            Options.MatchKey = options.MatchKey;
            Options.TimeLimit = options.TimeLimit;
            Options.ScoreLimit = options.ScoreLimit;
            Options.Password = options.Password;
            Options.IsFriendly = options.IsFriendly;
            Options.IsBalanced = options.IsBalanced;
            Options.ItemLimit = options.ItemLimit;
            Options.IsNoIntrusion = options.IsNoIntrusion;

            Broadcast(new SChangeRuleNotifyAckMessage(Options.Map<RoomCreationOptions, ChangeRuleDto>()));
            return RoomChangeRulesError.OK;
        }

        public uint GetAveragePing()
        {
            var players = TeamManager.SelectMany(t => t.Value.Values).ToArray();
            return (uint)(players.Sum(plr => plr.Session.UnreliablePing) / players.Length);
        }

        public void Broadcast(IGameMessage message)
        {
            foreach (var plr in _players.Values)
                plr.Session.Send(message);
        }

        public void Broadcast(IGameRuleMessage message)
        {
            foreach (var plr in _players.Values)
                plr.Session.Send(message);
        }

        public void BroadcastBriefing()
        {
            var briefing = new Briefing
            {
                Teams = GameRule.CreateBriefingTeams(),
                Players = GameRule.CreateBriefingPlayers(),
                Spectators = TeamManager.Spectators.Select(x => x.Account.Id).ToArray()
            };

            Broadcast(new SBriefingAckMessage(false, false, briefing.Serialize()));
        }

        private Player GetPlayerWithLowestPing(IEnumerable<Player> players = null)
        {
            players = players ?? Players.Values;
            return players.Aggregate(default(Player),
                (lowestPlayer, player) =>
                    lowestPlayer == null || player.Session.UnreliablePing < lowestPlayer.Session.UnreliablePing
                        ? player
                        : lowestPlayer);
        }

        private void OnGameStateChanged(object sender, EventArgs e)
        {
            RoomManager.Channel.Broadcast(new SChangeGameRoomAckMessage(this.Map<Room, RoomDto>()));
            if (GameRule.StateMachine.GameState == GameState.Result)
            {
                foreach (var plr in Players.Values)
                {
                    _messageBus.PublishAsync(new PlayerUpdateMessage(
                        plr.Account.Id, plr.TotalExperience, Id, plr.Team?.Id ?? TeamId.Neutral));
                }
            }
        }

        private void OnPlayerTeamChanged(object sender, PlayerTeamChangedEventArgs e)
        {
            var plr = e.Player;
            _messageBus.PublishAsync(new PlayerUpdateMessage(
                plr.Account.Id, plr.TotalExperience, Id, plr.Team?.Id ?? TeamId.Neutral));
        }

        private static void OnChangeRules(object This, object _)
        {
            var room = (Room)This;

            room.GameRule.Cleanup();

            room.Map = room._gameDataService.Maps[room.Options.MatchKey.Map];
            room.GameRule = room.Options.GameRuleResolver.Resolve(room);
            room.GameRule.Initialize(room);
            room.GameRule.StateMachine.GameStateChanged += room.OnGameStateChanged;

            room.IsChangingRules = false;
            room.OnOptionsChanged();
        }
    }
}
