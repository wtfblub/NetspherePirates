using System;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.Collections.Generic;
using Logging;
using Microsoft.Extensions.Options;
using Netsphere.Common;
using Netsphere.Network;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;
using Netsphere.Server.Game.Rules;
using ProudNet;

namespace Netsphere.Server.Game.Handlers
{
    internal class RoomHandler
        : IHandle<CMakeRoomReqMessage>, IHandle<CEnterPlayerReqMessage>, IHandle<CGameRoomEnterReqMessage>,
          IHandle<CJoinTunnelInfoReqMessage>, IHandle<CChangeTeamReqMessage>, IHandle<CPlayerGameModeChangeReqMessage>,
          IHandle<CMixChangeTeamReqMessage>, IHandle<CBeginRoundReqMessage>, IHandle<CReadyRoundReqMessage>,
          IHandle<CEventMessageReqMessage>, IHandle<CItemsChangeReqMessage>, IHandle<CAvatarChangeReqMessage>,
          IHandle<CChangeRuleNotifyReqMessage>, IHandle<CLeavePlayerRequestReqMessage>, IHandle<CAutoMixingTeamReqMessage>,
          IHandle<CScoreKillReqMessage>,
          IHandle<CScoreKillAssistReqMessage>, IHandle<CScoreTeamKillReqMessage>, IHandle<CScoreHealAssistReqMessage>,
          IHandle<CScoreSuicideReqMessage>, IHandle<CScoreGoalReqMessage>, IHandle<CScoreReboundReqMessage>,
          IHandle<CScoreOffenseReqMessage>, IHandle<CScoreOffenseAssistReqMessage>, IHandle<CScoreDefenseReqMessage>,
          IHandle<CScoreDefenseAssistReqMessage>, IHandle<CMissionScoreReqMessage>
    {
        private readonly ILogger<RoomHandler> _logger;
        private readonly EquipValidator _equipValidator;
        private readonly AppOptions _appOptions;

        public RoomHandler(ILogger<RoomHandler> logger, IOptions<AppOptions> appOptions, EquipValidator equipValidator)
        {
            _logger = logger;
            _equipValidator = equipValidator;
            _appOptions = appOptions.Value;
        }

        [Firewall(typeof(MustBeInRoom))]
        public async Task<bool> OnHandle(MessageContext context, CEnterPlayerReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (!plr.IsConnectingToRoom)
                return true;

            room.Broadcast(new SEnterPlayerAckMessage(plr.Account.Id, plr.Account.Nickname, 0, plr.Mode, 0));
            session.Send(new SChangeMasterAckMessage(plr.Room.Master.Account.Id));
            session.Send(new SChangeRefeReeAckMessage(plr.Room.Host.Account.Id));
            plr.Room.BroadcastBriefing();
            plr.IsConnectingToRoom = false;
            plr.Room.OnPlayerJoined(plr);
            return true;
        }

        [Firewall(typeof(MustBeInChannel))]
        [Firewall(typeof(MustBeInRoom), Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, CMakeRoomReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var channel = plr.Channel;
            var roomMgr = channel.RoomManager;
            var logger = plr.AddContextToLogger(_logger).ForContext("Message", message.ToJson());

            if (message.Room.MatchKey.GameRule == GameRule.Practice)
                message.Room.MatchKey.PlayerLimit = 1;

            var (room, createError) = roomMgr.Create(new RoomCreationOptions
            {
                Name = message.Room.Name,
                MatchKey = message.Room.MatchKey,
                TimeLimit = TimeSpan.FromMinutes(message.Room.TimeLimit),
                ScoreLimit = message.Room.ScoreLimit,
                Password = message.Room.Password,
                IsFriendly = message.Room.IsFriendly,
                IsBalanced = message.Room.IsBalanced,
                MinLevel = message.Room.MinLevel,
                MaxLevel = message.Room.MaxLevel,
                EquipLimit = message.Room.EquipLimit,
                IsNoIntrusion = message.Room.IsNoIntrusion,
                RelayEndPoint = _appOptions.RelayEndPoint
            });

            switch (createError)
            {
                case RoomCreateError.OK:
                    break;

                case RoomCreateError.InvalidGameRule:
                    logger.Warning("Trying to create room with invalid gamerule");
                    session.Send(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
                    return true;

                case RoomCreateError.InvalidMap:
                    logger.Warning("Trying to create room with invalid map");
                    session.Send(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
                    return true;

                default:
                    logger.Warning("Unknown error={Error} when creating room", createError);
                    session.Send(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
                    return true;
            }

            room.Join(plr);

            return true;
        }

        [Firewall(typeof(MustBeInChannel))]
        [Firewall(typeof(MustBeInRoom), Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, CGameRoomEnterReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var channel = plr.Channel;
            var roomMgr = channel.RoomManager;
            var room = roomMgr[message.RoomId];

            if (room == null)
            {
                session.Send(new SServerResultInfoAckMessage(ServerResult.ImpossibleToEnterRoom));
                session.Send(new SDisposeGameRoomAckMessage(message.RoomId));
                return true;
            }

            if (!string.IsNullOrWhiteSpace(room.Options.Password) &&
                !room.Options.Password.Equals(message.Password) &&
                !plr.IsInGMMode)
            {
                session.Send(new SServerResultInfoAckMessage(ServerResult.PasswordError));
                return true;
            }

            var error = room.Join(plr);
            switch (error)
            {
                case RoomJoinError.AlreadyInRoom:
                case RoomJoinError.RoomFull:
                case RoomJoinError.KickedPreviously:
                case RoomJoinError.NoIntrusion:
                    session.Send(new SServerResultInfoAckMessage(ServerResult.CantEnterRoom));
                    break;

                case RoomJoinError.ChangingRules:
                    session.Send(new SServerResultInfoAckMessage(ServerResult.RoomChangingRules));
                    break;
            }

            return true;
        }

        [Firewall(typeof(MustBeInRoom))]
        public Task<bool> OnHandle(MessageContext context, CJoinTunnelInfoReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            room.Leave(plr);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Waiting)]
        public async Task<bool> OnHandle(MessageContext context, CChangeTeamReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            // No reason for gm mode to change teams
            // so just block it to make sure the clients dont break
            if (plr.IsInGMMode)
                return true;

            var error = room.TeamManager.ChangeTeam(plr, message.Team);
            switch (error)
            {
                case TeamChangeError.Full:
                    session.Send(new SChangeTeamFailAckMessage(ChangeTeamResult.Full));
                    break;

                case TeamChangeError.PlayerIsReady:
                    session.Send(new SChangeTeamFailAckMessage(ChangeTeamResult.AlreadyReady));
                    break;
            }

            return true;
        }

        [Firewall(typeof(MustBeInRoom))]
        public async Task<bool> OnHandle(MessageContext context, CPlayerGameModeChangeReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            // Can only change between modes in lobby
            if (plr.State != PlayerState.Lobby)
                return true;

            // gm mode should always be spectator
            if (plr.IsInGMMode)
                return true;

            var error = room.TeamManager.ChangeMode(plr, message.Mode);
            switch (error)
            {
                case TeamChangeModeError.Full:
                    session.Send(new SChangeTeamFailAckMessage(ChangeTeamResult.Full));
                    break;

                case TeamChangeModeError.PlayerIsReady:
                    session.Send(new SChangeTeamFailAckMessage(ChangeTeamResult.AlreadyReady));
                    break;
            }

            return true;
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeMaster))]
        [Firewall(typeof(MustBeGameState), GameState.Waiting)]
        public async Task<bool> OnHandle(MessageContext context, CMixChangeTeamReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;
            var plrToMove = room.Players.GetValueOrDefault(message.PlayerToMove);
            var plrToReplace = room.Players.GetValueOrDefault(message.PlayerToReplace);
            var fromTeam = room.TeamManager[message.FromTeam];
            var toTeam = room.TeamManager[message.ToTeam];

            if (fromTeam == null || toTeam == null || plrToMove == null ||
                fromTeam != plrToMove.Team ||
                plrToReplace != null && toTeam != plrToReplace.Team)
            {
                session.Send(new SMixChangeTeamFailAckMessage());
                return true;
            }

            if (plrToReplace == null)
            {
                var error = toTeam.Join(plrToMove);

                if (error != TeamJoinError.OK)
                    session.Send(new SMixChangeTeamFailAckMessage());
            }
            else
            {
                room.TeamManager.SwapPlayer(plrToMove, plrToReplace);
                plr.Room.Broadcast(new SMixChangeTeamAckMessage(
                    plrToMove.Account.Id, plrToReplace.Account.Id,
                    fromTeam.Id, toTeam.Id));

                // SMixChangeTeamAckMessage alone doesn't seem to change the player list
                plr.Room.BroadcastBriefing();
            }

            return true;
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeMaster))]
        [Firewall(typeof(MustBeGameState), GameState.Waiting)]
        public async Task<bool> OnHandle(MessageContext context, CBeginRoundReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (!_equipValidator.IsValid(plr.CharacterManager.CurrentCharacter))
            {
                session.Send(new SServerResultInfoAckMessage(ServerResult.WearingUnusableItem));
                return true;
            }

            if (!room.GameRule.StateMachine.StartGame())
                session.Send(new SEventMessageAckMessage(GameEventMessage.CantStartGame, 0, 0, 0, ""));

            return true;
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Waiting)]
        public Task<bool> OnHandle(MessageContext context, CReadyRoundReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (!_equipValidator.IsValid(plr.CharacterManager.CurrentCharacter))
            {
                session.Send(new SServerResultInfoAckMessage(ServerResult.WearingUnusableItem));
                return Task.FromResult(true);
            }

            plr.IsReady = !plr.IsReady;
            room.Broadcast(new SReadyRoundAckMessage(plr.Account.Id, plr.IsReady));
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        public Task<bool> OnHandle(MessageContext context, CEventMessageReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            // Client is trying to enter the game while its running
            if (room.GameRule.StateMachine.GameState == GameState.Playing && plr.State == PlayerState.Lobby)
            {
                if (!_equipValidator.IsValid(plr.CharacterManager.CurrentCharacter))
                {
                    session.Send(new SServerResultInfoAckMessage(ServerResult.WearingUnusableItem));
                    return Task.FromResult(true);
                }

                for (var i = 0; i < plr.CharacterStartPlayTime.Length; ++i)
                    plr.CharacterStartPlayTime[i] = default;

                plr.CharacterStartPlayTime[plr.CharacterManager.CurrentSlot] = DateTimeOffset.Now;
                plr.StartPlayTime = DateTimeOffset.Now;
                plr.IsReady = false;
                plr.Score.Reset();
                plr.State = plr.Mode == PlayerGameMode.Normal
                    ? PlayerState.Alive
                    : PlayerState.Spectating;

                room.BroadcastBriefing();
            }

            room.Broadcast(new SEventMessageAckMessage(message.Event, session.Player.Account.Id,
                message.Unk1, message.Value, ""));

            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        public Task<bool> OnHandle(MessageContext context, CItemsChangeReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;
            var logger = plr.AddContextToLogger(_logger);

            // Can only change items in lobby and when not ready
            if (plr.State != PlayerState.Lobby || plr.IsReady)
            {
                plr.Disconnect();
                return Task.FromResult(true);
            }

            logger.Debug("Item sync unk1={Unk1}", message.Unk1);

            if (message.Unk2.Length > 0)
                logger.Warning("Item sync unk2={Unk2}", (object)message.Unk2);

            room.Broadcast(new SItemsChangeAckMessage(message.Unk1, message.Unk2));
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        public Task<bool> OnHandle(MessageContext context, CAvatarChangeReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;
            var logger = plr.AddContextToLogger(_logger);

            // Can only change characters in lobby, during half time or when not ready
            if (plr.State != PlayerState.Lobby &&
                room.GameRule.StateMachine.TimeState != GameTimeState.HalfTime ||
                plr.IsReady)
            {
                plr.Disconnect();
                return Task.FromResult(true);
            }

            logger.Debug("Avatar sync unk1={Unk1}", message.Unk1);

            if (message.Unk2.Length > 0)
                logger.Warning("Avatar sync unk2={Unk2}", (object)message.Unk2);

            room.Broadcast(new SAvatarChangeAckMessage(message.Unk1, message.Unk2));
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeMaster))]
        [Firewall(typeof(MustBeGameState), GameState.Waiting)]
        public async Task<bool> OnHandle(MessageContext context, CChangeRuleNotifyReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            var error = room.ChangeRules(message.Settings);
            if (error != RoomChangeRulesError.OK)
                session.Send(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));

            return true;
        }

        [Firewall(typeof(MustBeInRoom))]
        public Task<bool> OnHandle(MessageContext context, CLeavePlayerRequestReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            var targetPlr = room.Players.GetValueOrDefault(message.AccountId);
            if (targetPlr == null)
                return Task.FromResult(true);

            // Cant kick people in gm mode also disables AFK kick
            if (targetPlr.IsInGMMode)
                return Task.FromResult(true);

            switch (message.Reason)
            {
                case RoomLeaveReason.Kicked:
                    // Only the master can kick people and kick is only allowed in the lobby
                    if (room.Master != plr && room.GameRule.StateMachine.GameState != GameState.Waiting)
                        return Task.FromResult(true);

                    break;

                case RoomLeaveReason.AFK:
                    // The client kicks itself when afk is detected
                    if (message.AccountId != plr.Account.Id)
                        return Task.FromResult(true);

                    break;

                default:
                    // Dont allow any other reasons for now
                    return Task.FromResult(true);
            }

            room.Leave(targetPlr, message.Reason);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeMaster))]
        [Firewall(typeof(MustBeGameState), GameState.Waiting)]
        public Task<bool> OnHandle(MessageContext context, CAutoMixingTeamReqMessage message)
        {
            var session = context.GetSession<Session>();
            var room = session.Player.Room;

            var players = room.Players.Values
                .Where(x => x.Mode == PlayerGameMode.Normal)
                .Select(x => (plr: x, team: x.Team))
                .ToList();
            var rng = new Random(Guid.NewGuid().GetHashCode());

            foreach (var (plr, _) in players)
                plr.Team.Leave(plr);

            while (players.Count > 0)
            {
                var i = rng.Next(0, players.Count);
                var (plr, oldTeam) = players[i];
                players.RemoveAt(i);
                room.TeamManager.Join(plr);

                _logger.Debug("Shuffle team PlayerId={PlayerId} OldTeam={OldTeam} NewTeam={NewTeam}",
                    plr.Account.Id, oldTeam?.Id, plr.Team?.Id);

                if (plr.Team != oldTeam)
                    room.Broadcast(new SMixChangeTeamAckMessage(plr.Account.Id, 0, oldTeam.Id, plr.Team.Id));
            }

            room.BroadcastBriefing();
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, CScoreKillReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return Task.FromResult(true);

            room.GameRule.OnScoreKill(
                new ScoreContext(killer, message.Score.Killer.PeerId.ObjectType != 1 ? message.Score.Killer : null),
                null,
                new ScoreContext(plr, message.Score.Target.PeerId.ObjectType != 1 ? message.Score.Target : null),
                message.Score.Weapon);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, CScoreKillAssistReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return Task.FromResult(true);

            var assist = room.Players.GetValueOrDefault(message.Score.Assist.AccountId);
            if (assist == null)
                return Task.FromResult(true);

            room.GameRule.OnScoreKill(
                new ScoreContext(killer, message.Score.Killer.PeerId.ObjectType != 1 ? message.Score.Killer : null),
                new ScoreContext(assist, message.Score.Assist.PeerId.ObjectType != 1 ? message.Score.Assist : null),
                new ScoreContext(plr, message.Score.Target.PeerId.ObjectType != 1 ? message.Score.Target : null),
                message.Score.Weapon);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, CScoreTeamKillReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return Task.FromResult(true);

            room.GameRule.OnScoreTeamKill(
                new ScoreContext(killer, message.Score.Killer.PeerId.ObjectType != 1 ? message.Score.Killer : null),
                new ScoreContext(plr, message.Score.Target.PeerId.ObjectType != 1 ? message.Score.Target : null),
                message.Score.Weapon);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, CScoreHealAssistReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            room.GameRule.OnScoreHeal(plr);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, CScoreSuicideReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            room.GameRule.OnScoreSuicide(plr);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeMaster))]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, CScoreGoalReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            var td = room.Players.GetValueOrDefault(message.PeerId.AccountId);
            if (td == null)
                return Task.FromResult(true);

            room.GameRule.OnScoreTouchdown(td);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeMaster))]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, CScoreReboundReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            var newPlr = room.Players.GetValueOrDefault(message.NewId.AccountId);
            var oldPlr = room.Players.GetValueOrDefault(message.OldId.AccountId);
            room.GameRule.OnScoreFumbi(newPlr, oldPlr);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, CScoreOffenseReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return Task.FromResult(true);

            room.GameRule.OnScoreOffense(
                new ScoreContext(killer, message.Score.Killer.PeerId.ObjectType != 1 ? message.Score.Killer : null),
                null,
                new ScoreContext(plr, message.Score.Target.PeerId.ObjectType != 1 ? message.Score.Target : null),
                message.Score.Weapon);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, CScoreOffenseAssistReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return Task.FromResult(true);

            var assist = room.Players.GetValueOrDefault(message.Score.Assist.AccountId);
            if (assist == null)
                return Task.FromResult(true);

            room.GameRule.OnScoreOffense(
                new ScoreContext(killer, message.Score.Killer.PeerId.ObjectType != 1 ? message.Score.Killer : null),
                new ScoreContext(assist, message.Score.Assist.PeerId.ObjectType != 1 ? message.Score.Assist : null),
                new ScoreContext(plr, message.Score.Target.PeerId.ObjectType != 1 ? message.Score.Target : null),
                message.Score.Weapon);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, CScoreDefenseReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return Task.FromResult(true);

            room.GameRule.OnScoreDefense(
                new ScoreContext(killer, message.Score.Killer.PeerId.ObjectType != 1 ? message.Score.Killer : null),
                null,
                new ScoreContext(plr, message.Score.Target.PeerId.ObjectType != 1 ? message.Score.Target : null),
                message.Score.Weapon);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, CScoreDefenseAssistReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return Task.FromResult(true);

            var assist = room.Players.GetValueOrDefault(message.Score.Assist.AccountId);
            if (assist == null)
                return Task.FromResult(true);

            room.GameRule.OnScoreDefense(
                new ScoreContext(killer, message.Score.Killer.PeerId.ObjectType != 1 ? message.Score.Killer : null),
                new ScoreContext(assist, message.Score.Assist.PeerId.ObjectType != 1 ? message.Score.Assist : null),
                new ScoreContext(plr, message.Score.Target.PeerId.ObjectType != 1 ? message.Score.Target : null),
                message.Score.Weapon);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, CMissionScoreReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            room.GameRule.OnScoreMission(plr);
            return Task.FromResult(true);
        }
    }
}
