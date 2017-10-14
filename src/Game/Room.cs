using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.Collections.Concurrent;
using BlubLib.Threading.Tasks;
using ExpressMapper.Extensions;
using Netsphere.Game.Systems;
using Netsphere.Network;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Message.Chat;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;
using ProudNet;
using Serilog;
using Serilog.Core;
using SLeavePlayerAckMessage = Netsphere.Network.Message.GameRule.SLeavePlayerAckMessage;

namespace Netsphere
{
    internal class Room
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ILogger Logger = Log.ForContext(Constants.SourceContextPropertyName, nameof(Room));
        private readonly AsyncLock _slotIdSync = new AsyncLock();

        private readonly ConcurrentDictionary<ulong, Player> _players = new ConcurrentDictionary<ulong, Player>();
        private readonly ConcurrentDictionary<ulong, object> _kickedPlayers = new ConcurrentDictionary<ulong, object>();
        private readonly TimeSpan _hostUpdateTime = TimeSpan.FromSeconds(30);
        private readonly TimeSpan _changingRulesTime = TimeSpan.FromSeconds(5);
        private const uint PingDifferenceForChange = 20;

        private TimeSpan _hostUpdateTimer;
        private TimeSpan _changingRulesTimer;

        public RoomManager RoomManager { get; }
        public uint Id { get; }
        public RoomCreationOptions Options { get; }
        public DateTime TimeCreated { get; }

        public TeamManager TeamManager { get; }
        public GameRuleManager GameRuleManager { get; }

        public IReadOnlyDictionary<ulong, Player> Players => _players;

        public Player Master { get; private set; }
        public Player Host { get; private set; }
        public Player Creator { get; private set; }

        public P2PGroup Group { get; }

        public bool IsChangingRules { get; private set; }

        #region Events

        public event EventHandler<RoomPlayerEventArgs> PlayerJoining;
        public event EventHandler<RoomPlayerEventArgs> PlayerJoined;
        public event EventHandler<RoomPlayerEventArgs> PlayerLeft;
        public event EventHandler StateChanged;

        protected virtual void OnPlayerJoining(RoomPlayerEventArgs e)
        {
            PlayerJoining?.Invoke(this, e);
            RoomManager.Channel.Broadcast(new SChangeGameRoomAckMessage(this.Map<Room, RoomDto>()));
            RoomManager.Channel.Broadcast(new SUserDataAckMessage(e.Player.Map<Player, UserDataDto>()));
        }

        internal virtual void OnPlayerJoined(RoomPlayerEventArgs e)
        {
            PlayerJoined?.Invoke(this, e);
            RoomManager.Channel.Broadcast(new SUserDataAckMessage(e.Player.Map<Player, UserDataDto>()));
        }

        protected virtual void OnPlayerLeft(RoomPlayerEventArgs e)
        {
            PlayerLeft?.Invoke(this, e);
            RoomManager.Channel.Broadcast(new SChangeGameRoomAckMessage(this.Map<Room, RoomDto>()));
            RoomManager.Channel.Broadcast(new SUserDataAckMessage(e.Player.Map<Player, UserDataDto>()));
        }

        protected virtual void OnStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
            RoomManager.Channel.Broadcast(new SChangeGameRoomAckMessage(this.Map<Room, RoomDto>()));
        }

        #endregion

        public Room(RoomManager roomManager, uint id, RoomCreationOptions options, P2PGroup group)
        {
            RoomManager = roomManager;
            Id = id;
            Options = options;
            TimeCreated = DateTime.Now;
            TeamManager = new TeamManager(this);
            GameRuleManager = new GameRuleManager(this);
            Group = group;

            TeamManager.TeamChanged += TeamManager_TeamChanged;
            GameRuleManager.GameRuleChanged += GameRuleManager_OnGameRuleChanged;
            GameRuleManager.MapInfo = GameServer.Instance.ResourceCache.GetMaps()[options.MatchKey.Map];
            GameRuleManager.GameRule = RoomManager.GameRuleFactory.Get(Options.MatchKey.GameRule, this);
        }

        public void Update(TimeSpan delta)
        {
            if (Players.Count == 0)
                return;

            if (Host != null)
            {
                _hostUpdateTimer += delta;
                if (_hostUpdateTimer >= _hostUpdateTime)
                {
                    var lowest = GetPlayerWithLowestPing();
                    if (Host != lowest)
                    {
                        var diff = Math.Abs(Host.Session.UnreliablePing - lowest.Session.UnreliablePing);
                        if (diff >= PingDifferenceForChange)
                            ChangeHost(lowest);
                    }

                    _hostUpdateTimer = TimeSpan.Zero;
                }
            }

            if (IsChangingRules)
            {
                _changingRulesTimer += delta;
                if (_changingRulesTimer >= _changingRulesTime)
                {
                    GameRuleManager.MapInfo = GameServer.Instance.ResourceCache.GetMaps()[Options.MatchKey.Map];
                    GameRuleManager.GameRule = RoomManager.GameRuleFactory.Get(Options.MatchKey.GameRule, this);
                    Broadcast(new SChangeRuleAckMessage(Options.Map<RoomCreationOptions, ChangeRuleDto>()));
                    IsChangingRules = false;
                }
            }

            GameRuleManager.Update(delta);
        }

        public void Join(Player plr)
        {
            if (plr.Room != null)
                throw new RoomException("Player is already inside a room");

            if (_players.Count >= Options.MatchKey.PlayerLimit)
                throw new RoomLimitReachedException();

            if (_kickedPlayers.ContainsKey(plr.Account.Id))
                throw new RoomAccessDeniedException();

            using (_slotIdSync.Lock())
            {
                byte id = 3;
                while (Players.Values.Any(p => p.RoomInfo.Slot == id))
                    id++;

                plr.RoomInfo.Slot = id;
            }

            plr.RoomInfo.State = PlayerState.Lobby;
            plr.RoomInfo.Mode = PlayerGameMode.Normal;
            plr.RoomInfo.Stats = GameRuleManager.GameRule.GetPlayerRecord(plr);
            plr.RoomInfo.Reset();
            TeamManager.Join(plr);

            _players.TryAdd(plr.Account.Id, plr);
            plr.Room = this;
            plr.RoomInfo.IsConnecting = true;

            if (Master == null)
            {
                ChangeMaster(plr);
                ChangeHost(plr);
                Creator = plr;
            }

            Broadcast(new SEnteredPlayerAckMessage(plr.Map<Player, RoomPlayerDto>()));
            plr.Session.SendAsync(new SSuccessEnterRoomAckMessage(this.Map<Room, EnterRoomInfoDto>()));
            plr.Session.SendAsync(new SIdsInfoAckMessage(0, plr.RoomInfo.Slot));
            plr.Session.SendAsync(new SEnteredPlayerListAckMessage(_players.Values.Select(p => p.Map<Player, RoomPlayerDto>()).ToArray()));
            OnPlayerJoining(new RoomPlayerEventArgs(plr));
        }

        public void Leave(Player plr, RoomLeaveReason roomLeaveReason = RoomLeaveReason.Left)
        {
            if (plr.Room != this)
                return;

            Group.Leave(plr.RelaySession.HostId);
            Broadcast(new SLeavePlayerAckMessage(plr.Account.Id, plr.Account.Nickname, roomLeaveReason));

            if (roomLeaveReason == RoomLeaveReason.Kicked ||
                roomLeaveReason == RoomLeaveReason.ModeratorKick ||
                roomLeaveReason == RoomLeaveReason.VoteKick)
                _kickedPlayers.TryAdd(plr.Account.Id, null);

            plr.RoomInfo.PeerId = 0;
            plr.RoomInfo.Team.Leave(plr);
            _players.Remove(plr.Account.Id);
            plr.Room = null;
            plr.Session.SendAsync(new Network.Message.Game.SLeavePlayerAckMessage(plr.Account.Id));

            OnPlayerLeft(new RoomPlayerEventArgs(plr));

            if (_players.Count > 0)
            {
                if (Master == plr)
                    ChangeMaster(GetPlayerWithLowestPing());

                if (Host == plr)
                    ChangeHost(GetPlayerWithLowestPing());
            }
            else
            {
                RoomManager.Remove(this);
            }
        }

        public uint GetLatency()
        {
            // ToDo add this to config
            var good = 30;
            var bad = 190;

            var players = TeamManager.SelectMany(t => t.Value.Values).ToArray();
            var total = players.Sum(plr => plr.Session.UnreliablePing) / players.Length;

            if (total <= good)
                return 100;
            if (total >= bad)
                return 0;

            var result = (uint)(100f * total / bad);
            return 100 - result;
        }

        public void ChangeMaster(Player plr)
        {
            if (plr.Room != this || Master == plr)
                return;

            Master = plr;
            Broadcast(new SChangeMasterAckMessage(Master.Account.Id));
        }

        public void ChangeHost(Player plr)
        {
            if (plr.Room != this || Host == plr)
                return;

            // TODO Add Room extension?
            Logger.Debug("<Room {roomId}> Changing host to {nickname} - Ping:{ping} ms", Id, plr.Account.Nickname, plr.Session.UnreliablePing);
            Host = plr;
            Broadcast(new SChangeRefeReeAckMessage(Host.Account.Id));
        }

        public void ChangeRules(ChangeRuleDto options)
        {
            if (IsChangingRules)
                return;

            if (!RoomManager.GameRuleFactory.Contains(options.MatchKey.GameRule))
            {
                Logger.ForAccount(Master)
                    .Error("Game rule {gameRule} does not exist", options.MatchKey.GameRule);
                Master.Session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
                return;
            }

            var map = GameServer.Instance.ResourceCache.GetMaps().GetValueOrDefault(options.MatchKey.Map);
            if (map == null)
            {
                Logger.ForAccount(Master)
                    .Error("Map {map} does not exist", options.MatchKey.Map);
                Master.Session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
                return;
            }

            if (!map.GameRules.Contains(options.MatchKey.GameRule))
            {
                Logger.ForAccount(Master)
                    .Error("Map {mapId}({mapName}) is not available for game rule {gameRule}",
                        map.Id, map.Name, options.MatchKey.GameRule);
                Master.Session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
                return;
            }

            if (options.MatchKey.PlayerLimit < Players.Count)
            {
                Logger.ForAccount(Master)
                    .Error("Room has more players than the selected player limit");
                Master.Session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
                return;
            }

            _changingRulesTimer = TimeSpan.Zero;
            IsChangingRules = true;
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
        }

        private Player GetPlayerWithLowestPing()
        {
            return _players.Values.Aggregate((lowestPlayer, player) => (lowestPlayer == null || player.Session.UnreliablePing < lowestPlayer.Session.UnreliablePing ? player : lowestPlayer));
        }

        private void TeamManager_TeamChanged(object sender, TeamChangedEventArgs e)
        {
            RoomManager.Channel.Broadcast(new SUserDataAckMessage(e.Player.Map<Player, UserDataDto>()));
        }

        private void GameRuleManager_OnGameRuleChanged(object sender, EventArgs e)
        {
            GameRuleManager.GameRule.StateMachine.OnTransitioned(t => OnStateChanged());

            foreach (var plr in Players.Values)
            {
                plr.RoomInfo.Stats = GameRuleManager.GameRule.GetPlayerRecord(plr);
                var team = TeamManager[plr.RoomInfo.Team.Team];

                // Move spectators to normal when spectators are disabled
                if (plr.RoomInfo.Mode == PlayerGameMode.Spectate && !Options.MatchKey.IsObserveEnabled)
                    plr.RoomInfo.Mode = PlayerGameMode.Normal;

                // Try to rejoin the old team first then fallback to default join
                try
                {
                    if (team != null)
                        team.Join(plr);
                    else
                        TeamManager.Join(plr);
                }
                catch (TeamLimitReachedException)
                {
                    try
                    {
                        // Original team was full
                        // fallback to default join and try to join another team
                        TeamManager.Join(plr);
                    }
                    catch (TeamLimitReachedException) when (plr.RoomInfo.Mode == PlayerGameMode.Spectate)
                    {
                        // Should only happen when the spectator limit got reduced
                        // Move spectators to normal when spectator slots are filled
                        plr.RoomInfo.Mode = PlayerGameMode.Normal;
                        TeamManager.Join(plr);
                    }
                }
            }

            BroadcastBriefing();
        }

        #region Broadcast

        public void Broadcast(IGameMessage message)
        {
            foreach (var plr in _players.Values)
                plr.Session.SendAsync(message);
        }

        public void Broadcast(IGameRuleMessage message)
        {
            foreach (var plr in _players.Values)
                plr.Session.SendAsync(message);
        }

        public void Broadcast(IChatMessage message)
        {
            foreach (var plr in _players.Values)
                plr.Session.SendAsync(message);
        }

        public void BroadcastBriefing(bool isResult = false)
        {
            var gameRule = GameRuleManager.GameRule;
            //var isResult = gameRule.StateMachine.IsInState(GameRuleState.Result);
            Broadcast(new SBriefingAckMessage(isResult, false, gameRule.Briefing.ToArray(isResult)));
        }

        #endregion
    }
}
