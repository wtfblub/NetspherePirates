using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using ExpressMapper.Extensions;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Chat;
using Netsphere.Network.Message.Game;
using NLog;
using NLog.Fluent;
using ProudNet.Handlers;

namespace Netsphere.Network.Services
{
    internal class ChannelService : ProudMessageHandler
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [MessageHandler(typeof(ChannelInfoReqMessage))]
        public void ChannelInfoReq(GameSession session, ChannelInfoReqMessage message)
        {
            switch (message.Request)
            {
                case ChannelInfoRequest.ChannelList:
                    session.SendAsync(new ChannelListInfoAckMessage(GameServer.Instance.ChannelManager.Select(c => c.Map<Channel, ChannelInfoDto>()).ToArray()));
                    break;

                case ChannelInfoRequest.RoomList:
                case ChannelInfoRequest.RoomList2:
                    if (session.Player.Channel == null)
                        return;

                    session.SendAsync(new RoomListInfoAckMessage(session.Player.Channel.RoomManager.Select(r => r.Map<Room, RoomDto>()).ToArray()));
                    break;

                default:
                    Logger.Error()
                        .Account(session)
                        .Message($"Invalid request {message.Request}")
                        .Write();
                    break;
            }
        }

        [MessageHandler(typeof(ChannelEnterReqMessage))]
        public void CChannelEnterReq(GameSession session, ChannelEnterReqMessage message)
        {
            var channel = GameServer.Instance.ChannelManager[message.Channel];
            if (channel == null)
            {
                session.SendAsync(new ServerResultAckMessage(ServerResult.NonExistingChannel));
                return;
            }

            try
            {
                channel.Join(session.Player);
            }
            catch (ChannelLimitReachedException)
            {
                session.SendAsync(new ServerResultAckMessage(ServerResult.ChannelLimitReached));
            }
        }

        [MessageHandler(typeof(ChannelLeaveReqMessage))]
        public void CChannelLeaveReq(GameSession session)
        {
            session.Player.Channel?.Leave(session.Player);
        }

        [MessageHandler(typeof(MessageChatReqMessage))]
        public void CChatMessageReq(ChatSession session, MessageChatReqMessage message)
        {
            switch (message.ChatType)
            {
                case ChatType.Channel:
                    session.Player.Channel.SendChatMessage(session.Player, message.Message);
                    break;

                case ChatType.Club:
                    // ToDo Change this when clans are implemented
                    session.SendAsync(new MessageChatAckMessage(ChatType.Club, session.Player.Account.Id, session.Player.Account.Nickname, message.Message));
                    break;

                default:
                    Logger.Warn()
                        .Account(session)
                        .Message($"Invalid chat type {message.ChatType}")
                        .Write();
                    break;
            }
        }

        [MessageHandler(typeof(MessageWhisperChatReqMessage))]
        public void CWhisperChatMessageReq(ChatSession session, MessageWhisperChatReqMessage message)
        {
            var toPlr = GameServer.Instance.PlayerManager.Get(message.ToNickname);

            // ToDo Is there an answer for this case?
            if (toPlr == null)
            {
                session.Player.ChatSession.SendAsync(new MessageChatAckMessage(ChatType.Channel, session.Player.Account.Id, "SYSTEM", $"{message.ToNickname} is not online"));
                return;
            }

            // ToDo Is there an answer for this case?
            if (toPlr.DenyManager.Contains(session.Player.Account.Id))
            {
                session.Player.ChatSession.SendAsync(new MessageChatAckMessage(ChatType.Channel, session.Player.Account.Id, "SYSTEM", $"{message.ToNickname} is ignoring you"));
                return;
            }

            toPlr.ChatSession.SendAsync(new MessageWhisperChatAckMessage(0, toPlr.Account.Nickname,
                session.Player.Account.Id, session.Player.Account.Nickname, message.Message));
        }

        [MessageHandler(typeof(RoomQuickStartReqMessage))]
        public Task CQuickStartReq(GameSession session, RoomQuickStartReqMessage message)
        {
            //ToDo - Logic
            return session.SendAsync(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
        }

        [MessageHandler(typeof(TaskReguestReqMessage))]
        public Task TaskRequestReq(GameSession session, TaskReguestReqMessage message)
        {
            //ToDo - Logic
            return session.SendAsync(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
        }
    }
}
