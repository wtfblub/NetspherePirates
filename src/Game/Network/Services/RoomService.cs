using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BlubLib.DotNetty.Handlers.MessageHandling;
using Netsphere.Game.GameRules;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;
using ProudNet.Handlers;
using Serilog;
using Serilog.Core;

namespace Netsphere.Network.Services
{
    internal class RoomService : ProudMessageHandler
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ILogger Logger = Log.ForContext(Constants.SourceContextPropertyName, nameof(RoomService));

        [MessageHandler(typeof(CEnterPlayerReqMessage))]
        public void CEnterPlayerReq(GameSession session)
        {
            var plr = session.Player;

            plr.Room.Broadcast(new SEnterPlayerAckMessage(plr.Account.Id, plr.Account.Nickname, 0, plr.RoomInfo.Mode, 0));
            session.SendAsync(new SChangeMasterAckMessage(plr.Room.Master.Account.Id));
            session.SendAsync(new SChangeRefeReeAckMessage(plr.Room.Host.Account.Id));
            plr.Room.BroadcastBriefing();
        }

        [MessageHandler(typeof(CMakeRoomReqMessage))]
        public void CMakeRoomReq(GameSession session, CMakeRoomReqMessage message)
        {
            var plr = session.Player;
            if (!plr.Channel.RoomManager.GameRuleFactory.Contains(message.Room.MatchKey.GameRule))
            {
                Logger.ForAccount(plr)
                    .Error("Game rule {gameRule} does not exist", message.Room.MatchKey.GameRule);
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
                return;
            }

            var map = GameServer.Instance.ResourceCache.GetMaps().GetValueOrDefault(message.Room.MatchKey.Map);
            if (map == null)
            {
                Logger.ForAccount(plr)
                    .Error("Map {map} does not exist", message.Room.MatchKey.Map);
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
                return;
            }
            if (!map.GameRules.Contains(message.Room.MatchKey.GameRule))
            {
                Logger.ForAccount(plr)
                    .Error("Map {mapId}({mapName}) is not available for game rule {gameRule}", map.Id, map.Name, message.Room.MatchKey.GameRule);
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
                return;
            }

            var room = plr.Channel.RoomManager.Create(new RoomCreationOptions
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

                ServerEndPoint = new IPEndPoint(IPAddress.Parse(Config.Instance.IP), Config.Instance.RelayListener.Port)
            }, RelayServer.Instance.P2PGroupManager.Create(true));

            room.Join(plr);
        }

        [MessageHandler(typeof(CGameRoomEnterReqMessage))]
        public void CGameRoomEnterReq(GameSession session, CGameRoomEnterReqMessage message)
        {
            var plr = session.Player;
            var room = plr.Channel.RoomManager[message.RoomId];
            if (room == null)
            {
                Logger.ForAccount(plr)
                    .Error("Room {roomId} in channel {channelId} not found", message.RoomId, plr.Channel.Id);
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.ImpossibleToEnterRoom));
                return;
            }

            if (room.IsChangingRules)
            {
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.RoomChangingRules));
                return;
            }

            if (!string.IsNullOrEmpty(room.Options.Password) && !room.Options.Password.Equals(message.Password))
            {
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.PasswordError));
                return;
            }

            try
            {
                room.Join(plr);
            }
            catch (RoomAccessDeniedException)
            {
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.CantEnterRoom));
            }
            catch (RoomLimitReachedException)
            {
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.CantEnterRoom));
            }
            catch (RoomException)
            {
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.ImpossibleToEnterRoom));
            }
        }

        [MessageHandler(typeof(CJoinTunnelInfoReqMessage))]
        public void CJoinTunnelInfoReq(GameSession session)
        {
            var plr = session.Player;
            plr.Room.Leave(plr);
        }

        [MessageHandler(typeof(CChangeTeamReqMessage))]
        public void CChangeTeamReq(GameSession session, CChangeTeamReqMessage message)
        {
            var plr = session.Player;

            try
            {
                plr.Room.TeamManager.ChangeTeam(plr, message.Team);
            }
            catch (RoomException ex)
            {
                Logger.ForAccount(plr)
                    .Error(ex, "Failed to change team to {team}", message.Team);
            }
        }

        [MessageHandler(typeof(CPlayerGameModeChangeReqMessage))]
        public void CPlayerGameModeChangeReq(GameSession session, CPlayerGameModeChangeReqMessage message)
        {
            var plr = session.Player;

            try
            {
                plr.Room.TeamManager.ChangeMode(plr, message.Mode);
            }
            catch (RoomException ex)
            {
                Logger.ForAccount(plr)
                    .Error(ex, "Failed to change mode to {mode}", message.Mode);
            }
        }

        [MessageHandler(typeof(CBeginRoundReqMessage))]
        public void CBeginRoundReq(GameSession session)
        {
            var plr = session.Player;
            var stateMachine = plr.Room.GameRuleManager.GameRule.StateMachine;

            if (stateMachine.CanFire(GameRuleStateTrigger.StartGame))
                stateMachine.Fire(GameRuleStateTrigger.StartGame);
            else
                session.SendAsync(new SEventMessageAckMessage(GameEventMessage.CantStartGame, 0, 0, 0, ""));
        }

        [MessageHandler(typeof(CReadyRoundReqMessage))]
        public void CReadyRoundReq(GameSession session)
        {
            var plr = session.Player;
            var gameRule = plr.Room.GameRuleManager.GameRule;
            if (gameRule.StateMachine.IsInState(GameRuleState.Starting) && plr.RoomInfo.State == PlayerState.Lobby)
            {
                plr.RoomInfo.State = plr.RoomInfo.Mode == PlayerGameMode.Normal
                    ? PlayerState.Alive
                    : PlayerState.Spectating;

                plr.Room.BroadcastBriefing();
                plr.Session.SendAsync(new SEventMessageAckMessage(GameEventMessage.HalfTimeIn, 1, 0, 0, ""));
                plr.Session.SendAsync(new SEventMessageAckMessage(GameEventMessage.NextRoundIn,
                    (ulong)(GameRuleBase.StartingWaitTime - gameRule.RoundTime).TotalMilliseconds, 0, 0, ""));

                // Remove the "Requesting" box
                plr.Session.SendAsync(new SReadyRoundAckMessage(plr.Account.Id, false));
            }
            else
            {
                plr.RoomInfo.IsReady = !plr.RoomInfo.IsReady;
                plr.Room.Broadcast(new SReadyRoundAckMessage(plr.Account.Id, plr.RoomInfo.IsReady));
            }
        }

        [MessageHandler(typeof(CEventMessageReqMessage))]
        public void CEventMessageReq(GameSession session, CEventMessageReqMessage message)
        {
            var plr = session.Player;
            var gameRule = plr.Room.GameRuleManager.GameRule;

            plr.Room.Broadcast(new SEventMessageAckMessage(message.Event, session.Player.Account.Id, message.Unk1, message.Value, ""));
            //if (message.Event == GameEventMessage.BallReset && plr == plr.Room.Host)
            //{
            //    plr.Room.Broadcast(new SEventMessageAckMessage(GameEventMessage.BallReset, 0, 0, 0, ""));
            //    return;
            //}

            //if (message.Event != GameEventMessage.StartGame)
            //    return;

            if (gameRule.StateMachine.IsInState(GameRuleState.Playing) && plr.RoomInfo.State == PlayerState.Lobby)
            {
                plr.RoomInfo.State = plr.RoomInfo.Mode == PlayerGameMode.Normal
                    ? PlayerState.Alive
                    : PlayerState.Spectating;

                plr.Room.BroadcastBriefing();

                if (gameRule.StateMachine.IsInState(GameRuleState.Playing))
                {
                    plr.Session.SendAsync(new SEventMessageAckMessage(GameEventMessage.HalfTimeIn, 1, 0, 0, ""));
                    plr.Session.SendAsync(new SEventMessageAckMessage(GameEventMessage.NextRoundIn,
                        (ulong)(GameRuleBase.StartingWaitTime - gameRule.RoundTime).TotalMilliseconds, 0, 0, ""));
                }
            }
        }

        [MessageHandler(typeof(CItemsChangeReqMessage))]
        public void CItemsChangeReq(GameSession session, CItemsChangeReqMessage message)
        {
            var plr = session.Player;

            Logger.ForAccount(session)
                .Debug("Item sync - {unk1}", message.Unk1);

            if (message.Unk2.Length > 0)
            {
                Logger.ForAccount(session)
                    .Warning("{unk2}", message.Unk2);
            }

            var @char = plr.CharacterManager.CurrentCharacter;
            var unk1 = new ChangeItemsUnkDto
            {
                AccountId = plr.Account.Id,
                Skills = @char.Skills.GetItems().Select(item => item?.ItemNumber ?? 0).ToArray(),
                Weapons = @char.Weapons.GetItems().Select(item => item?.ItemNumber ?? 0).ToArray(),
                Unk4 = message.Unk1.Unk4,
                Unk5 = message.Unk1.Unk5,
                Unk6 = message.Unk1.Unk6,
                HP = plr.GetMaxHP(),
                Unk8 = message.Unk1.Unk8
            };

            plr.Room.Broadcast(new SItemsChangeAckMessage(unk1, message.Unk2));
        }

        [MessageHandler(typeof(CAvatarChangeReqMessage))]
        public void CAvatarChangeReq(GameSession session, CAvatarChangeReqMessage message)
        {
            var plr = session.Player;

            Logger.ForAccount(session)
                .Debug("Avatar sync - {unk1}", message.Unk1);

            if (message.Unk2.Length > 0)
            {
                Logger.ForAccount(session)
                    .Warning("{unk2}", message.Unk2);
            }

            var @char = plr.CharacterManager.CurrentCharacter;
            var unk1 = new ChangeAvatarUnk1Dto
            {
                AccountId = plr.Account.Id,
                Skills = @char.Skills.GetItems().Select(item => item?.ItemNumber ?? 0).ToArray(),
                Weapons = @char.Weapons.GetItems().Select(item => item?.ItemNumber ?? 0).ToArray(),
                Costumes = new ItemNumber[(int)CostumeSlot.Max],
                Unk5 = message.Unk1.Unk5,
                Unk6 = message.Unk1.Unk6,
                Unk7 = message.Unk1.Unk7,
                Unk8 = message.Unk1.Unk8,
                Gender = plr.CharacterManager.CurrentCharacter.Gender,
                HP = plr.GetMaxHP(),
                Unk11 = message.Unk1.Unk11
            };

            // If no item equipped use the default item the character was created with
            for (CostumeSlot slot = 0; slot < CostumeSlot.Max; slot++)
            {
                var item = plr.CharacterManager.CurrentCharacter.Costumes.GetItem(slot)?.ItemNumber ?? 0;
                switch (slot)
                {
                    case CostumeSlot.Hair:
                        if (item == 0)
                            item = @char.Hair.ItemNumber;
                        break;

                    case CostumeSlot.Face:
                        if (item == 0)
                            item = @char.Face.ItemNumber;
                        break;

                    case CostumeSlot.Shirt:
                        if (item == 0)
                            item = @char.Shirt.ItemNumber;
                        break;

                    case CostumeSlot.Pants:
                        if (item == 0)
                            item = @char.Pants.ItemNumber;
                        break;

                    case CostumeSlot.Gloves:
                        if (item == 0)
                            item = @char.Gloves.ItemNumber;
                        break;

                    case CostumeSlot.Shoes:
                        if (item == 0)
                            item = @char.Shoes.ItemNumber;
                        break;
                }
                unk1.Costumes[(int)slot] = item;
            }

            plr.Room.Broadcast(new SAvatarChangeAckMessage(unk1, message.Unk2));
        }

        [MessageHandler(typeof(CChangeRuleNotifyReqMessage))]
        public void CChangeRuleNotifyReq(GameSession session, CChangeRuleNotifyReqMessage message)
        {
            session.Player.Room.ChangeRules(message.Settings);
        }

        [MessageHandler(typeof(CLeavePlayerRequestReqMessage))]
        public void CLeavePlayerRequestReq(GameSession session, CLeavePlayerRequestReqMessage message)
        {
            return; // TODO JUST FOR DEBUGGING - REMOVE BEFORE COMMIT
            var plr = session.Player;
            var room = plr.Room;

            switch (message.Reason)
            {
                case RoomLeaveReason.Kicked:
                    // Only the master can kick people and kick is only allowed in the lobby
                    if (room.Master != plr &&
                        !room.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Waiting))
                        return;
                    break;

                case RoomLeaveReason.AFK:
                    // The client kicks itself when afk is detected
                    if (message.AccountId != plr.Account.Id)
                        return;
                    break;

                default:
                    // Dont allow any other reasons for now
                    return;
            }

            var targetPlr = room.Players.GetValueOrDefault(message.AccountId);
            if (targetPlr == null)
                return;

            room.Leave(targetPlr, message.Reason);
        }

        #region Scores

        [MessageHandler(typeof(CScoreKillReqMessage))]
        public void CScoreKillReq(GameSession session, CScoreKillReqMessage message)
        {
            var plr = session.Player;
            plr.RoomInfo.PeerId = message.Score.Target;

            var room = plr.Room;
            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return;
            killer.RoomInfo.PeerId = message.Score.Killer;

            room.GameRuleManager.GameRule.OnScoreKill(killer, null, plr, message.Score.Weapon);
        }

        [MessageHandler(typeof(CScoreKillAssistReqMessage))]
        public void CScoreKillAssistReq(GameSession session, CScoreKillAssistReqMessage message)
        {
            var plr = session.Player;
            plr.RoomInfo.PeerId = message.Score.Target;

            var room = plr.Room;
            var assist = room.Players.GetValueOrDefault(message.Score.Assist.AccountId);
            if (assist == null)
                return;
            assist.RoomInfo.PeerId = message.Score.Assist;

            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return;
            killer.RoomInfo.PeerId = message.Score.Killer;

            room.GameRuleManager.GameRule.OnScoreKill(killer, assist, plr, message.Score.Weapon);
        }

        [MessageHandler(typeof(CScoreOffenseReqMessage))]
        public void CScoreOffenseReq(GameSession session, CScoreOffenseReqMessage message)
        {
            var plr = session.Player;
            plr.RoomInfo.PeerId = message.Score.Target;

            var room = plr.Room;
            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return;
            killer.RoomInfo.PeerId = message.Score.Killer;

            if (room.Options.MatchKey.GameRule == GameRule.Touchdown)
                ((TouchdownGameRule)room.GameRuleManager.GameRule).OnScoreOffense(killer, null, plr, message.Score.Weapon);
        }

        [MessageHandler(typeof(CScoreOffenseAssistReqMessage))]
        public void CScoreOffenseAssistReq(GameSession session, CScoreOffenseAssistReqMessage message)
        {
            var plr = session.Player;
            plr.RoomInfo.PeerId = message.Score.Target;

            var room = plr.Room;
            var assist = room.Players.GetValueOrDefault(message.Score.Assist.AccountId);
            if (assist == null)
                return;
            assist.RoomInfo.PeerId = message.Score.Assist;

            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return;
            killer.RoomInfo.PeerId = message.Score.Killer;

            if (room.Options.MatchKey.GameRule == GameRule.Touchdown)
                ((TouchdownGameRule)room.GameRuleManager.GameRule).OnScoreOffense(killer, assist, plr, message.Score.Weapon);
        }

        [MessageHandler(typeof(CScoreDefenseReqMessage))]
        public void CScoreDefenseReq(GameSession session, CScoreDefenseReqMessage message)
        {
            var plr = session.Player;
            plr.RoomInfo.PeerId = message.Score.Target;

            var room = plr.Room;
            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return;
            killer.RoomInfo.PeerId = message.Score.Killer;

            if (room.Options.MatchKey.GameRule == GameRule.Touchdown)
                ((TouchdownGameRule)room.GameRuleManager.GameRule).OnScoreDefense(killer, null, plr, message.Score.Weapon);
        }

        [MessageHandler(typeof(CScoreDefenseAssistReqMessage))]
        public void CScoreDefenseAssistReq(GameSession session, CScoreDefenseAssistReqMessage message)
        {
            var plr = session.Player;
            plr.RoomInfo.PeerId = message.Score.Target;

            var room = plr.Room;
            var assist = room.Players.GetValueOrDefault(message.Score.Assist.AccountId);
            if (assist == null)
                return;
            assist.RoomInfo.PeerId = message.Score.Assist;

            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return;
            killer.RoomInfo.PeerId = message.Score.Killer;

            if (room.Options.MatchKey.GameRule == GameRule.Touchdown)
                ((TouchdownGameRule)room.GameRuleManager.GameRule).OnScoreDefense(killer, assist, plr, message.Score.Weapon);
        }

        [MessageHandler(typeof(CScoreTeamKillReqMessage))]
        public void CScoreTeamKillReq(GameSession session, CScoreTeamKillReqMessage message)
        {
            var plr = session.Player;
            plr.RoomInfo.PeerId = message.Score.Target;

            var room = plr.Room;
            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return;
            killer.RoomInfo.PeerId = message.Score.Killer;

            room.GameRuleManager.GameRule.OnScoreKill(killer, null, plr, message.Score.Weapon);
        }

        [MessageHandler(typeof(CScoreHealAssistReqMessage))]
        public void CScoreHealAssistReq(GameSession session, CScoreHealAssistReqMessage message)
        {
            var plr = session.Player;
            plr.RoomInfo.PeerId = message.Id;

            var room = plr.Room;
            room.GameRuleManager.GameRule.OnScoreHeal(plr);
        }

        [MessageHandler(typeof(CScoreSuicideReqMessage))]
        public void CScoreSuicideReq(GameSession session, CScoreSuicideReqMessage message)
        {
            var plr = session.Player;
            plr.RoomInfo.PeerId = message.Id;

            var room = plr.Room;
            room.GameRuleManager.GameRule.OnScoreSuicide(plr);
        }

        [MessageHandler(typeof(CScoreReboundReqMessage))]
        public void CScoreReboundReq(GameSession session, CScoreReboundReqMessage message)
        {
            var plr = session.Player;
            var room = plr.Room;

            Player newPlr = null;
            Player oldPlr = null;

            if (message.NewId != 0)
                newPlr = room.Players.GetValueOrDefault(message.NewId.AccountId);

            if (message.OldId != 0)
                oldPlr = room.Players.GetValueOrDefault(message.OldId.AccountId);

            if (newPlr != null)
                newPlr.RoomInfo.PeerId = message.NewId;

            if (oldPlr != null)
                oldPlr.RoomInfo.PeerId = message.OldId;

            if (room.Options.MatchKey.GameRule == GameRule.Touchdown)
                ((TouchdownGameRule)room.GameRuleManager.GameRule).OnScoreRebound(newPlr, oldPlr);
        }

        [MessageHandler(typeof(CScoreGoalReqMessage))]
        public void CScoreGoalReq(GameSession session, CScoreGoalReqMessage message)
        {
            var plr = session.Player;
            var room = plr.Room;

            var target = room.Players.GetValueOrDefault(message.PeerId.AccountId);
            if (target == null)
                return;
            target.RoomInfo.PeerId = message.PeerId;

            if (room.Options.MatchKey.GameRule == GameRule.Touchdown)
                ((TouchdownGameRule)room.GameRuleManager.GameRule).OnScoreGoal(target);
        }

        #endregion
    }
}
