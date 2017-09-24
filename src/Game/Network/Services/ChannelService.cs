using System.Linq;
using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using ExpressMapper.Extensions;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Chat;
using Netsphere.Network.Message.Game;
using ProudNet.Handlers;
using Serilog;
using Serilog.Core;

namespace Netsphere.Network.Services
{
    internal class ChannelService : ProudMessageHandler
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ILogger Logger = Log.ForContext(Constants.SourceContextPropertyName, nameof(ChannelService));

        [MessageHandler(typeof(CGetChannelInfoReqMessage))]
        public void CGetChannelInfoReq(GameSession session, CGetChannelInfoReqMessage message)
        {
            switch (message.Request)
            {
                case ChannelInfoRequest.ChannelList:
                    session.SendAsync(new SChannelListInfoAckMessage(GameServer.Instance.ChannelManager.Select(c => c.Map<Channel, ChannelInfoDto>()).ToArray()));
                    break;

                case ChannelInfoRequest.RoomList:
                case ChannelInfoRequest.RoomList2:
                    if (session.Player.Channel == null)
                        return;
                    session.SendAsync(new SGameRoomListAckMessage(session.Player.Channel.RoomManager.Select(r => r.Map<Room, RoomDto>()).ToArray()));
                    break;

                default:
                    Logger.ForAccount(session)
                        .Error("Invalid request {request}", message.Request);
                    break;
            }
        }

        [MessageHandler(typeof(CChannelEnterReqMessage))]
        public void CChannelEnterReq(GameSession session, CChannelEnterReqMessage message)
        {
            var channel = GameServer.Instance.ChannelManager[message.Channel];
            if (channel == null)
            {
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.NonExistingChannel));
                return;
            }

            try
            {
                channel.Join(session.Player);
            }
            catch (ChannelLimitReachedException)
            {
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.ChannelLimitReached));
            }
        }

        [MessageHandler(typeof(CChannelLeaveReqMessage))]
        public void CChannelLeaveReq(GameSession session)
        {
            session.Player.Channel?.Leave(session.Player);
        }

        [MessageHandler(typeof(CChatMessageReqMessage))]
        public void CChatMessageReq(ChatSession session, CChatMessageReqMessage message)
        {
            switch (message.ChatType)
            {
                case ChatType.Channel:
                    session.Player.Channel.SendChatMessage(session.Player, message.Message);
                    break;

                case ChatType.Club:
                    // ToDo Change this when clans are implemented
                    session.SendAsync(new SChatMessageAckMessage(ChatType.Club, session.Player.Account.Id, session.Player.Account.Nickname, message.Message));
                    break;

                default:
                    Logger.ForAccount(session)
                        .Warning("Invalid chat type {chatType}", message.ChatType);
                    break;
            }
        }

        [MessageHandler(typeof(CWhisperChatMessageReqMessage))]
        public void CWhisperChatMessageReq(ChatSession session, CWhisperChatMessageReqMessage message)
        {
            var toPlr = GameServer.Instance.PlayerManager.Get(message.ToNickname);

            // ToDo Is there an answer for this case?
            if (toPlr == null)
            {
                session.Player.ChatSession.SendAsync(new SChatMessageAckMessage(ChatType.Channel, session.Player.Account.Id, "SYSTEM", $"{message.ToNickname} is not online"));
                return;
            }

            // ToDo Is there an answer for this case?
            if (toPlr.DenyManager.Contains(session.Player.Account.Id))
            {
                session.Player.ChatSession.SendAsync(new SChatMessageAckMessage(ChatType.Channel, session.Player.Account.Id, "SYSTEM", $"{message.ToNickname} is ignoring you"));
                return;
            }

            toPlr.ChatSession.SendAsync(new SWhisperChatMessageAckMessage(0, toPlr.Account.Nickname,
                session.Player.Account.Id, session.Player.Account.Nickname, message.Message));
        }

        [MessageHandler(typeof(CQuickStartReqMessage))]
        public Task CQuickStartReq(GameSession session, CQuickStartReqMessage message)
        {
            System.Collections.Generic.Dictionary<Room, int> rooms = new System.Collections.Generic.Dictionary<Room, int>();
            foreach(Room room in session.Player.Channel.RoomManager)
            {
                if (room.Options.Password == "")
                {
                    if (room.Options.MatchKey.GameRule.Equals(message.GameRule))
                    {
                        if (room.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Waiting) || (room.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Playing) && !room.Options.IsNoIntrusion)){
                            var priority = 0;
                            priority += System.Math.Abs(room.TeamManager[Team.Alpha].Players.Count() - room.TeamManager[Team.Beta].Players.Count()); // Calculating team balance

                            if (room.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.SecondHalf))
                            {
                                if (room.Options.TimeLimit.TotalSeconds / 2 - room.GameRuleManager.GameRule.RoundTime.TotalSeconds <= 15) // If only 15 seconds are left...
                                {
                                    priority -= 3; // ...lower the room priority
                                }
                            }

                            rooms.Add(room, priority);
                        }
                    }
                }
            }

            var roomList = rooms.ToList();

            if (roomList.Count() > 0)
            {
                roomList.Sort((room1, room2) => room2.Value.CompareTo(room1.Value));
                roomList.First().Key.Join(session.Player);

                return Task.FromResult(0); // We don't message the Client here, because "Room.Join(...)" already does it.
            }

            return session.SendAsync(new SServerResultInfoAckMessage(ServerResult.QuickJoinFailed));
        }

        [MessageHandler(typeof(CTaskRequestReqMessage))]
        public Task TaskRequestReq(GameSession session, CTaskRequestReqMessage message)
        {
            //ToDo - Logic
            return session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
        }
    }
}
