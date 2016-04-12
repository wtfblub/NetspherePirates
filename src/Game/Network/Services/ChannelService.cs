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
    internal class ChannelService : Service
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

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
                    _logger.Error()
                        .Account(session)
                        .Message("Invalid request {0}", message.Request)
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
            catch (ChannelException ex)
            {
                _logger.Error()
                    .Account(session)
                    .Exception(ex)
                    .Write();
                session.Send(new SServerResultInfoAckMessage(ServerResult.JoinChannelFailed));
            }
        }

        [MessageHandler(typeof(CChannelLeaveReqMessage))]
        public void CChannelLeaveReq(GameSession session)
        {
            if (session.Player.Channel == null)
                return;

            session.Player.Channel.Leave(session.Player);
            session.Send(new SServerResultInfoAckMessage(ServerResult.ChannelLeave));
        }

        [MessageHandler(typeof(CChatMessageReqMessage))]
        public void CChatMessageReq(ChatSession session, CChatMessageReqMessage message)
        {
            if (message.ChatType == ChatType.Channel)
            {
                session.Player.Channel.SendChatMessage(session.Player, message.Message);
            }
            else
            {
                session.Send(new SChatMessageAckMessage(ChatType.Club, session.Player.Account.Id, session.Player.Account.Nickname, message.Message));
            }
        }

        [MessageHandler(typeof(CWhisperChatMessageReqMessage))]
        public void CWhisperChatMessageReq(ChatSession session, CWhisperChatMessageReqMessage message)
        {
            var toPlr = GameServer.Instance.PlayerManager.Get(message.ToNickname);
            toPlr?.ChatSession.Send(new SWhisperChatMessageAckMessage(0, toPlr.Account.Nickname, session.Player.Account.Id, session.Player.Account.Nickname, message.Message));
        }
    }
}
