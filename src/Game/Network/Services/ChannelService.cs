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
            //ToDo - Logic
            return session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
        }

        [MessageHandler(typeof(CTaskRequestReqMessage))]
        public Task TaskRequestReq(GameSession session, CTaskRequestReqMessage message)
        {
            //ToDo - Logic
            return session.SendAsync(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
        }
    }
}
