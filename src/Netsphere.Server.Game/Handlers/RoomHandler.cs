using System;
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
          IHandle<CChangeRuleNotifyReqMessage>, IHandle<CLeavePlayerRequestReqMessage>, IHandle<CScoreKillReqMessage>,
          IHandle<CScoreKillAssistReqMessage>, IHandle<CScoreTeamKillReqMessage>, IHandle<CScoreHealAssistReqMessage>,
          IHandle<CScoreSuicideReqMessage>, IHandle<CScoreGoalReqMessage>, IHandle<CScoreReboundReqMessage>,
          IHandle<CScoreOffenseReqMessage>, IHandle<CScoreOffenseAssistReqMessage>, IHandle<CScoreDefenseReqMessage>,
          IHandle<CScoreDefenseAssistReqMessage>
    {
        private readonly ILogger<RoomHandler> _logger;
        private readonly AppOptions _appOptions;
        private readonly GameRuleManager _gameRuleManager;

        public RoomHandler(ILogger<RoomHandler> logger, IOptions<AppOptions> appOptions, GameRuleManager gameRuleManager)
        {
            _logger = logger;
            _gameRuleManager = gameRuleManager;
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
            await session.SendAsync(new SChangeMasterAckMessage(plr.Room.Master.Account.Id));
            await session.SendAsync(new SChangeRefeReeAckMessage(plr.Room.Host.Account.Id));
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
                ItemLimit = message.Room.EquipLimit,
                IsNoIntrusion = message.Room.IsNoIntrusion,
                RelayEndPoint = _appOptions.RelayEndPoint,
                GameRuleResolver = new DefaultGameRuleResolver(_gameRuleManager)
            });

            switch (createError)
            {
                case RoomCreateError.OK:
                    break;

                case RoomCreateError.InvalidGameRule:
                    logger.Warning("Trying to create room with invalid gamerule");
                    await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
                    return true;

                case RoomCreateError.InvalidMap:
                    logger.Warning("Trying to create room with invalid map");
                    await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
                    return true;

                default:
                    logger.Warning("Unknown error={Error} when creating room", createError);
                    await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
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
                await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.ImpossibleToEnterRoom));
                await session.SendAsync(new SDisposeGameRoomAckMessage(message.RoomId));
                return true;
            }

            if (!string.IsNullOrWhiteSpace(room.Options.Password) &&
                !room.Options.Password.Equals(message.Password))
            {
                await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.PasswordError));
                return true;
            }

            var error = room.Join(plr);
            switch (error)
            {
                case RoomJoinError.AlreadyInRoom:
                case RoomJoinError.RoomFull:
                case RoomJoinError.KickedPreviously:
                    await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.CantEnterRoom));
                    break;

                case RoomJoinError.ChangingRules:
                    await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.RoomChangingRules));
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

            var error = room.TeamManager.ChangeTeam(plr, message.Team);
            switch (error)
            {
                case TeamChangeError.Full:
                    await session.SendAsync(new SChangeTeamFailAckMessage(ChangeTeamResult.Full));
                    break;

                case TeamChangeError.PlayerIsReady:
                    await session.SendAsync(new SChangeTeamFailAckMessage(ChangeTeamResult.AlreadyReady));
                    break;
            }

            return true;
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Waiting)]
        public async Task<bool> OnHandle(MessageContext context, CPlayerGameModeChangeReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            var error = room.TeamManager.ChangeMode(plr, message.Mode);
            switch (error)
            {
                case TeamChangeModeError.Full:
                    await session.SendAsync(new SChangeTeamFailAckMessage(ChangeTeamResult.Full));
                    break;

                case TeamChangeModeError.PlayerIsReady:
                    await session.SendAsync(new SChangeTeamFailAckMessage(ChangeTeamResult.AlreadyReady));
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
                await session.SendAsync(new SMixChangeTeamFailAckMessage());
                return true;
            }

            if (plrToReplace == null)
            {
                var error = toTeam.Join(plrToMove);

                if (error != TeamJoinError.OK)
                    await session.SendAsync(new SMixChangeTeamFailAckMessage());
            }
            else
            {
                fromTeam.Leave(plrToMove);
                toTeam.Leave(plrToReplace);
                fromTeam.Join(plrToReplace);
                toTeam.Join(plrToMove);

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

            if (!room.GameRule.StateMachine.StartGame())
                await session.SendAsync(new SEventMessageAckMessage(GameEventMessage.CantStartGame, 0, 0, 0, ""));

            return true;
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Waiting)]
        public Task<bool> OnHandle(MessageContext context, CReadyRoundReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

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

            room.Broadcast(new SEventMessageAckMessage(message.Event, session.Player.Account.Id,
                message.Unk1, message.Value, ""));

            // Client is trying to enter the game while its running
            if (room.GameRule.StateMachine.GameState == GameState.Playing && plr.State == PlayerState.Lobby)
            {
                plr.State = plr.Mode == PlayerGameMode.Normal
                    ? PlayerState.Alive
                    : PlayerState.Spectating;

                room.BroadcastBriefing();
            }

            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Waiting)]
        public Task<bool> OnHandle(MessageContext context, CItemsChangeReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;
            var logger = plr.AddContextToLogger(_logger);

            logger.Debug("Item sync unk1={Unk1}", message.Unk1);

            if (message.Unk2.Length > 0)
                logger.Warning("Item sync unk2={Unk2}", (object)message.Unk2);

            room.Broadcast(new SItemsChangeAckMessage(message.Unk1, message.Unk2));
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        // [Firewall(typeof(MustBeGameState), GameState.Waiting)]
        public Task<bool> OnHandle(MessageContext context, CAvatarChangeReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;
            var logger = plr.AddContextToLogger(_logger);

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
                await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));

            return true;
        }

        [Firewall(typeof(MustBeInRoom))]
        public Task<bool> OnHandle(MessageContext context, CLeavePlayerRequestReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

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

            var targetPlr = room.Players.GetValueOrDefault(message.AccountId);
            if (targetPlr == null)
                return Task.FromResult(true);

            room.Leave(targetPlr, message.Reason);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
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
        public Task<bool> OnHandle(MessageContext context, CScoreGoalReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            room.GameRule.OnScoreTouchdown(plr);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeMaster))]
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
    }
}
