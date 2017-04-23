using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using Netsphere.Game.GameRules;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;
using Newtonsoft.Json;
using NLog;
using NLog.Fluent;
using ProudNet.Handlers;

namespace Netsphere.Network.Services
{
    internal class RoomService : ProudMessageHandler
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [MessageHandler(typeof(RoomEnterPlayerReqMessage))]
        public void CEnterPlayerReq(GameSession session)
        {
            var plr = session.Player;

            plr.Room.Broadcast(new RoomEnterPlayerAckMessage(plr.Account.Id, plr.Account.Nickname, 0, plr.RoomInfo.Mode, 0));
            session.SendAsync(new RoomChangeMasterAckMessage(plr.Room.Master.Account.Id));
            session.SendAsync(new RoomChangeRefereeAckMessage(plr.Room.Host.Account.Id));
            plr.Room.BroadcastBriefing();
        }

        [MessageHandler(typeof(RoomMakeReqMessage))]
        public void CMakeRoomReq(GameSession session, RoomMakeReqMessage message)
        {
            var plr = session.Player;
            if (!plr.Channel.RoomManager.GameRuleFactory.Contains(message.Room.MatchKey.GameRule))
            {
                Logger.Error()
                    .Account(plr)
                    .Message($"Game rule {message.Room.MatchKey.GameRule} does not exist")
                    .Write();
                session.SendAsync(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
                return;
            }

            var map = GameServer.Instance.ResourceCache.GetMaps().GetValueOrDefault(message.Room.MatchKey.Map);
            if (map == null)
            {
                Logger.Error()
                    .Account(plr).Message($"Map {message.Room.MatchKey.Map} does not exist")
                    .Write();
                session.SendAsync(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
                return;
            }
            if (!map.GameRules.Contains(message.Room.MatchKey.GameRule))
            {
                Logger.Error()
                    .Account(plr)
                    .Message($"Map {map.Id}({map.Name}) is not available for game rule {message.Room.MatchKey.GameRule}")
                    .Write();
                session.SendAsync(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
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

        [MessageHandler(typeof(RoomEnterReqMessage))]
        public void CGameRoomEnterReq(GameSession session, RoomEnterReqMessage message)
        {
            Logger.Debug(JsonConvert.SerializeObject(message, Formatting.Indented));

            var plr = session.Player;
            var room = plr.Channel.RoomManager[message.RoomId];
            if (room == null)
            {
                Logger.Error()
                    .Account(plr)
                    .Message($"Room {message.RoomId} in channel {plr.Channel.Id} not found")
                    .Write();
                session.SendAsync(new ServerResultAckMessage(ServerResult.ImpossibleToEnterRoom));
                return;
            }

            if (room.IsChangingRules)
            {
                session.SendAsync(new ServerResultAckMessage(ServerResult.RoomChangingRules));
                return;
            }

            if (!string.IsNullOrEmpty(room.Options.Password) && !room.Options.Password.Equals(message.Password))
            {
                session.SendAsync(new ServerResultAckMessage(ServerResult.PasswordError));
                return;
            }

            try
            {
                room.Join(plr);
            }
            catch (RoomAccessDeniedException)
            {
                session.SendAsync(new ServerResultAckMessage(ServerResult.CantEnterRoom));
            }
            catch (RoomLimitReachedException)
            {
                session.SendAsync(new ServerResultAckMessage(ServerResult.CantEnterRoom));
            }
            catch (RoomException)
            {
                session.SendAsync(new ServerResultAckMessage(ServerResult.ImpossibleToEnterRoom));
            }
        }

        [MessageHandler(typeof(RoomLeaveReqMessage))]
        public void CJoinTunnelInfoReq(GameSession session)
        {
            var plr = session.Player;
            plr.Room.Leave(plr);
        }

        [MessageHandler(typeof(RoomTeamChangeReqMessage))]
        public void CChangeTeamReq(GameSession session, RoomTeamChangeReqMessage message)
        {
            var plr = session.Player;

            try
            {
                plr.Room.TeamManager.ChangeTeam(plr, message.Team);
            }
            catch (RoomException ex)
            {
                Logger.Error()
                    .Account(plr)
                    .Exception(ex).Message("Failed to change team to {0}", message.Team)
                    .Write();
            }
        }

        [MessageHandler(typeof(RoomPlayModeChangeReqMessage))]
        public void CPlayerGameModeChangeReq(GameSession session, RoomPlayModeChangeReqMessage message)
        {
            var plr = session.Player;

            try
            {
                plr.Room.TeamManager.ChangeMode(plr, message.Mode);
            }
            catch (RoomException ex)
            {
                Logger.Error()
                    .Account(plr)
                    .Exception(ex)
                    .Message($"Failed to change mode to {message.Mode}")
                    .Write();
            }
        }

        [MessageHandler(typeof(RoomBeginRoundReqMessage))]
        public void CBeginRoundReq(GameSession session)
        {
            var plr = session.Player;
            var stateMachine = plr.Room.GameRuleManager.GameRule.StateMachine;

            if (stateMachine.CanFire(GameRuleStateTrigger.StartGame))
                stateMachine.Fire(GameRuleStateTrigger.StartGame);
            else
                session.SendAsync(new GameEventMessageAckMessage(GameEventMessage.CantStartGame, 0, 0, 0, ""));
        }

        [MessageHandler(typeof(RoomReadyRoundReqMessage))]
        public void CReadyRoundReq(GameSession session)
        {
            var plr = session.Player;

            plr.RoomInfo.IsReady = !plr.RoomInfo.IsReady;
            plr.Room.Broadcast(new RoomReadyRoundAckMessage(plr.Account.Id, plr.RoomInfo.IsReady));
        }

        [MessageHandler(typeof(GameEventMessageReqMessage))]
        public void CEventMessageReq(GameSession session, GameEventMessageReqMessage message)
        {
            var plr = session.Player;

            plr.Room.Broadcast(new GameEventMessageAckMessage(message.Event, session.Player.Account.Id, message.Unk1, message.Value, ""));
            //if (message.Event == GameEventMessage.BallReset && plr == plr.Room.Host)
            //{
            //    plr.Room.Broadcast(new GameEventMessageAckMessage(GameEventMessage.BallReset, 0, 0, 0, ""));
            //    return;
            //}

            //if (message.Event != GameEventMessage.StartGame)
            //    return;

            if (plr.Room.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Playing) && plr.RoomInfo.State == PlayerState.Lobby)
            {
                plr.RoomInfo.State = plr.RoomInfo.Mode == PlayerGameMode.Normal
                    ? PlayerState.Alive
                    : PlayerState.Spectating;

                plr.Room.BroadcastBriefing();
            }
        }

        [MessageHandler(typeof(RoomItemChangeReqMessage))]
        public void CItemsChangeReq(GameSession session, RoomItemChangeReqMessage message)
        {
            var plr = session.Player;

            Logger.Debug()
                .Account(session)
                .Message($"Item sync - {JsonConvert.SerializeObject(message.Unk1, Formatting.Indented)}")
                .Write();

            if (message.Unk2.Length > 0)
            {
                Logger.Warn()
                    .Account(session)
                    .Message($"Unk2: {JsonConvert.SerializeObject(message.Unk2, Formatting.Indented)}")
                    .Write();
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

            plr.Room.Broadcast(new RoomChangeItemAckMessage(unk1, message.Unk2));
        }

        [MessageHandler(typeof(GameAvatarChangeReqMessage))]
        public void CAvatarChangeReq(GameSession session, GameAvatarChangeReqMessage message)
        {
            var plr = session.Player;

            Logger.Debug()
                .Account(session)
                .Message($"Avatar sync - {JsonConvert.SerializeObject(message.Unk1, Formatting.Indented)}")
                .Write();

            if (message.Unk2.Length > 0)
            {
                Logger.Warn()
                    .Account(session)
                    .Message($"Unk2: {JsonConvert.SerializeObject(message.Unk2, Formatting.Indented)}")
                    .Write();
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

            plr.Room.Broadcast(new GameAvatarChangeAckMessage(unk1, message.Unk2));
        }

        [MessageHandler(typeof(RoomChangeRuleNotifyReqMessage))]
        public void CChangeRuleNotifyReq(GameSession session, RoomChangeRuleNotifyReqMessage message)
        {
            session.Player.Room.ChangeRules(message.Settings);
        }

        [MessageHandler(typeof(RoomLeaveReguestReqMessage))]
        public void CLeavePlayerRequestReq(GameSession session, RoomLeaveReguestReqMessage message)
        {
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

        [MessageHandler(typeof(ScoreKillReqMessage))]
        public void CScoreKillReq(GameSession session, ScoreKillReqMessage message)
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

        [MessageHandler(typeof(ScoreKillAssistReqMessage))]
        public void CScoreKillAssistReq(GameSession session, ScoreKillAssistReqMessage message)
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

        [MessageHandler(typeof(ScoreOffenseReqMessage))]
        public void CScoreOffenseReq(GameSession session, ScoreOffenseReqMessage message)
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

        [MessageHandler(typeof(ScoreOffenseAssistReqMessage))]
        public void CScoreOffenseAssistReq(GameSession session, ScoreOffenseAssistReqMessage message)
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

        [MessageHandler(typeof(ScoreDefenseReqMessage))]
        public void CScoreDefenseReq(GameSession session, ScoreDefenseReqMessage message)
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

        [MessageHandler(typeof(ScoreDefenseAssistReqMessage))]
        public void CScoreDefenseAssistReq(GameSession session, ScoreDefenseAssistReqMessage message)
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

        [MessageHandler(typeof(ScoreTeamKillReqMessage))]
        public void CScoreTeamKillReq(GameSession session, ScoreTeamKillReqMessage message)
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

        [MessageHandler(typeof(ScoreHealAssistReqMessage))]
        public void CScoreHealAssistReq(GameSession session, ScoreHealAssistReqMessage message)
        {
            var plr = session.Player;
            plr.RoomInfo.PeerId = message.Id;

            var room = plr.Room;
            room.GameRuleManager.GameRule.OnScoreHeal(plr);
        }

        [MessageHandler(typeof(ScoreSuicideReqMessage))]
        public void CScoreSuicideReq(GameSession session, ScoreSuicideReqMessage message)
        {
            var plr = session.Player;
            plr.RoomInfo.PeerId = message.Id;

            var room = plr.Room;
            room.GameRuleManager.GameRule.OnScoreSuicide(plr);
        }

        [MessageHandler(typeof(ScoreReboundReqMessage))]
        public void CScoreReboundReq(GameSession session, ScoreReboundReqMessage message)
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

        [MessageHandler(typeof(ScoreGoalReqMessage))]
        public void CScoreGoalReq(GameSession session, ScoreGoalReqMessage message)
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
