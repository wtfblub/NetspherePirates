using System.Linq;
using BlubLib.Network.Pipes;
using ExpressMapper.Extensions;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Chat;
using Netsphere.Network.Message.Game;
using NLog;
using NLog.Fluent;

namespace Netsphere.Network.Services
{
    internal class ChannelService : MessageHandler
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [MessageHandler(typeof(CGetChannelInfoReqMessage))]
        public void CGetChannelInfoReq(GameSession session, CGetChannelInfoReqMessage message)
        {
            switch (message.Request)
            {
                case ChannelInfoRequest.ChannelList:
                    session.Send(new SChannelListInfoAckMessage(GameServer.Instance.ChannelManager.Select(c => c.Map<Channel, ChannelInfoDto>()).ToArray()));
                    break;

                case ChannelInfoRequest.RoomList:
                case ChannelInfoRequest.RoomList2:
                    if (session.Player.Channel == null)
                        return;
                    session.Send(new SGameRoomListAckMessage(session.Player.Channel.RoomManager.Select(r => r.Map<Room, RoomDto>()).ToArray()));
                    break;

                default:
                    Logger.Error()
                        .Account(session)
                        .Message($"Invalid request {message.Request}")
                        .Write();
                    break;
            }
        }

        [MessageHandler(typeof(CChannelEnterReqMessage))]
        public void CChannelEnterReq(GameSession session, CChannelEnterReqMessage message)
        {
            var channel = GameServer.Instance.ChannelManager[message.Channel];
            if (channel == null)
            {
                session.Send(new SServerResultInfoAckMessage(ServerResult.NonExistingChannel));
                return;
            }

            try
            {
                channel.Join(session.Player);
            }
            catch (ChannelLimitReachedException)
            {
                session.Send(new SServerResultInfoAckMessage(ServerResult.ChannelLimitReached));
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
                    session.Send(new SChatMessageAckMessage(ChatType.Club, session.Player.Account.Id, session.Player.Account.Nickname, message.Message));
                    break;

                default:
                    Logger.Warn()
                        .Account(session)
                        .Message($"Invalid chat type {message.ChatType}")
                        .Write();
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
                session.Player.ChatSession.Send(new SChatMessageAckMessage(ChatType.Channel, session.Player.Account.Id, "SYSTEM", $"{message.ToNickname} is not online"));
                return;
            }

            // ToDo Is there an answer for this case?
            if (toPlr.DenyManager.Contains(session.Player.Account.Id))
            {
                session.Player.ChatSession.Send(new SChatMessageAckMessage(ChatType.Channel, session.Player.Account.Id, "SYSTEM", $"{message.ToNickname} is ignoring you"));
                return;
            }

            toPlr.ChatSession.Send(new SWhisperChatMessageAckMessage(0, toPlr.Account.Nickname,
                session.Player.Account.Id, session.Player.Account.Nickname, message.Message));
        }

        [MessageHandler(typeof(CQuickStartReqMessage))]
        public void CQuickStartReq(GameSession session, CQuickStartReqMessage message)
        {
            //ToDo - Logic
            session.Send(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
        }

        [MessageHandler(typeof(CTaskRequestReqMessage))]
        public void TaskRequestReq(GameSession session, CTaskRequestReqMessage message)
        {
            //ToDo - Logic
            session.Send(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
        }
    }
}
