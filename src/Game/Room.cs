﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BlubLib.Threading.Tasks;
using ExpressMapper.Extensions;
using Netsphere.Game.Systems;
using Netsphere.Network;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Message;
using Netsphere.Network.Message.Chat;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;
using NLog;
using NLog.Fluent;
using ProudNet;
using ChatMessage = Netsphere.Network.Message.ChatMessage;
using SLeavePlayerAckMessage = Netsphere.Network.Message.GameRule.SLeavePlayerAckMessage;

namespace Netsphere
{
    internal class Room
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly AsyncLock _slotIdSync = new AsyncLock();

        private readonly ConcurrentDictionary<ulong, Player> _players = new ConcurrentDictionary<ulong, Player>();
        private List<Player> _kickedPlayers = new List<Player>();
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

        public ServerP2PGroup Group { get; }

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
            RoomManager.Channel.BroadcastChat(new SUserDataAckMessage(e.Player.Map<Player, UserDataDto>()));
        }

        internal virtual void OnPlayerJoined(RoomPlayerEventArgs e)
        {
            PlayerJoined?.Invoke(this, e);
            RoomManager.Channel.BroadcastChat(new SUserDataAckMessage(e.Player.Map<Player, UserDataDto>()));
        }

        protected virtual void OnPlayerLeft(RoomPlayerEventArgs e)
        {
            PlayerLeft?.Invoke(this, e);
            RoomManager.Channel.Broadcast(new SChangeGameRoomAckMessage(this.Map<Room, RoomDto>()));
            RoomManager.Channel.BroadcastChat(new SUserDataAckMessage(e.Player.Map<Player, UserDataDto>()));
        }

        protected virtual void OnStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
            RoomManager.Channel.Broadcast(new SChangeGameRoomAckMessage(this.Map<Room, RoomDto>()));
        }

        #endregion

        public Room(RoomManager roomManager, uint id, RoomCreationOptions options, ServerP2PGroup group)
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

            Group.Join(2, false);
        }

        public void Update(TimeSpan delta)
        {
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

            if (_kickedPlayers.Contains(plr))
            {
                plr.Session.Send(new SServerResultInfoAckMessage(ServerResult.CantEnterRoom));
                return;
            }

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
            plr.Session.Send(new SSuccessEnterRoomAckMessage(this.Map<Room, EnterRoomInfoDto>()));
            plr.Session.Send(new SIdsInfoAckMessage(0, plr.RoomInfo.Slot));
            plr.Session.Send(new SEnteredPlayerListAckMessage(_players.Values.Select(p => p.Map<Player, RoomPlayerDto>()).ToArray()));
            OnPlayerJoining(new RoomPlayerEventArgs(plr));
        }

        public void Leave(Player plr, RoomLeaveReason roomLeaveReason = RoomLeaveReason.Left)
        {
            if (plr.Room != this)
                return;

            Group.Leave(plr.RoomInfo.Slot);
            Broadcast(new SLeavePlayerAckMessage(plr.Account.Id, plr.Account.Nickname, roomLeaveReason));

            if (roomLeaveReason == RoomLeaveReason.Kicked)
                _kickedPlayers.Add(plr);

            plr.RoomInfo.PeerId = 0;
            plr.RoomInfo.Team.Leave(plr);
            _players.Remove(plr.Account.Id);
            plr.Room = null;
            plr.Session.Send(new Network.Message.Game.SLeavePlayerAckMessage(plr.Account.Id));

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

            Logger.Debug()
                .Message("<Room {0}> Changing host to {1} - Ping:{2} ms", Id, plr.Account.Nickname, plr.Session.UnreliablePing)
                .Write();
            Host = plr;
            Broadcast(new SChangeRefeReeAckMessage(Host.Account.Id));
        }

        public void ChangeRules(ChangeRuleDto options)
        {
            if (IsChangingRules)
                return;

            if (!RoomManager.GameRuleFactory.Contains(options.MatchKey.GameRule))
            {
                Logger.Error()
                    .Account(Master)
                    .Message($"Game rule {options.MatchKey.GameRule} does not exist")
                    .Write();
                Master.Session.Send(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
                return;
            }

            var map = GameServer.Instance.ResourceCache.GetMaps().GetValueOrDefault(options.MatchKey.Map);
            if (map == null)
            {
                Logger.Error()
                    .Account(Master).Message($"Map {options.MatchKey.Map} does not exist")
                    .Write();
                Master.Session.Send(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
                return;
            }

            if (!map.GameRules.Contains(options.MatchKey.GameRule))
            {
                Logger.Error()
                    .Account(Master)
                    .Message($"Map {map.Id}({map.Name}) is not available for game rule {options.MatchKey.GameRule}")
                    .Write();
                Master.Session.Send(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
                return;
            }

            // ToDo check if current player count is not above the new player limit
            {
                if (_players.Count() > options.MatchKey.PlayerLimit)
                {
                    Logger.Error()
                        .Account(Master)
                        .Message($"Players count ({_players.Count()}) is bigger than room limit ({options.MatchKey.PlayerLimit})")
                        .Write();
                    Master.Session.Send(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
                    return;
                }
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
            RoomManager.Channel.BroadcastChat(new SUserDataAckMessage(e.Player.Map<Player, UserDataDto>()));
        }

        private void GameRuleManager_OnGameRuleChanged(object sender, EventArgs e)
        {
            GameRuleManager.GameRule.StateMachine.OnTransitioned(t => OnStateChanged());

            foreach (var plr in Players.Values)
            {
                plr.RoomInfo.Stats = GameRuleManager.GameRule.GetPlayerRecord(plr);
                TeamManager.Join(plr);
            }
            BroadcastBriefing();
        }

        #region Broadcast

        public void Broadcast(GameMessage message)
        {
            foreach (var plr in _players.Values)
                plr.Session.Send(message);
        }

        public void Broadcast(GameRuleMessage message)
        {
            foreach (var plr in _players.Values)
                plr.Session.Send(message);
        }

        public void Broadcast(ChatMessage message)
        {
            foreach (var plr in _players.Values)
                plr.Session.Send(message);
        }

        //public void Broadcast(uint sender, P2PMessage message)
        //{
        //    var peerMessage = new PacketMessage(false, message.ToArray());
        //    foreach (var plr in _players.Values.Where(plr => plr.RoomInfo.PeerId.PeerId.Slot != sender))
        //    {
        //        peerMessage.SenderHostId = sender;
        //        peerMessage.TargetHostId = plr.RelaySession.HostId;
        //        peerMessage.IsRelayed = true;
        //        plr.Session.Send(peerMessage);
        //    }
        //}

        public void BroadcastBriefing(bool isResult = false)
        {
            var gameRule = GameRuleManager.GameRule;
            //var isResult = gameRule.StateMachine.IsInState(GameRuleState.Result);
            Broadcast(new SBriefingAckMessage(isResult, false, gameRule.Briefing.ToArray(isResult)));
        }

        #endregion
    }
}
