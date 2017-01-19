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

        [MessageHandler(typeof(CGetChannelInfoReqMessage))]
        public async Task CGetChannelInfoReq(GameSession session, CGetChannelInfoReqMessage message)
        {
            switch (message.Request)
            {
                case ChannelInfoRequest.ChannelList:
                    await session.SendAsync(new SChannelListInfoAckMessage(GameServer.Instance.ChannelManager.Select(c => c.Map<Channel, ChannelInfoDto>()).ToArray()))
                        .ConfigureAwait(false);
                    break;

                case ChannelInfoRequest.RoomList:
                case ChannelInfoRequest.RoomList2:
                    if (session.Player.Channel == null)
                        return;
                    await session.SendAsync(new SGameRoomListAckMessage(session.Player.Channel.RoomManager.Select(r => r.Map<Room, RoomDto>()).ToArray()))
                        .ConfigureAwait(false);
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
        public async Task CChannelEnterReq(GameSession session, CChannelEnterReqMessage message)
        {
            var channel = GameServer.Instance.ChannelManager[message.Channel];
            if (channel == null)
            {
                await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.NonExistingChannel))
                    .ConfigureAwait(false);
                return;
            }

            try
            {
                channel.Join(session.Player);
            }
            catch (ChannelLimitReachedException)
            {
                await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.ChannelLimitReached))
                    .ConfigureAwait(false);
            }
        }

        [MessageHandler(typeof(CChannelLeaveReqMessage))]
        public void CChannelLeaveReq(GameSession session)
        {
            session.Player.Channel?.Leave(session.Player);
        }

        [MessageHandler(typeof(CChatMessageReqMessage))]
        public async Task CChatMessageReq(ChatSession session, CChatMessageReqMessage message)
        {
            switch (message.ChatType)
            {
                case ChatType.Channel:
                    await session.Player.Channel.SendChatMessageAsync(session.Player, message.Message)
                        .ConfigureAwait(false);
                    break;

                case ChatType.Club:
                    // ToDo Change this when clans are implemented
                    await session.SendAsync(new SChatMessageAckMessage(ChatType.Club, session.Player.Account.Id, session.Player.Account.Nickname, message.Message))
                        .ConfigureAwait(false);
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
        public async Task CWhisperChatMessageReq(ChatSession session, CWhisperChatMessageReqMessage message)
        {
            var toPlr = GameServer.Instance.PlayerManager.Get(message.ToNickname);

            // ToDo Is there an answer for this case?
            if (toPlr == null)
            {
                await session.Player.ChatSession.SendAsync(new SChatMessageAckMessage(ChatType.Channel, session.Player.Account.Id, "SYSTEM", $"{message.ToNickname} is not online"))
                    .ConfigureAwait(false);
                return;
            }

            // ToDo Is there an answer for this case?
            if (toPlr.DenyManager.Contains(session.Player.Account.Id))
            {
                await session.Player.ChatSession.SendAsync(new SChatMessageAckMessage(ChatType.Channel, session.Player.Account.Id, "SYSTEM", $"{message.ToNickname} is ignoring you"))
                    .ConfigureAwait(false);
                return;
            }

            await toPlr.ChatSession.SendAsync(new SWhisperChatMessageAckMessage(0, toPlr.Account.Nickname,
                session.Player.Account.Id, session.Player.Account.Nickname, message.Message))
                .ConfigureAwait(false);
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
