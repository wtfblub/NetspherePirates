using System.Linq;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Logging;
using Netsphere.Network;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Rules;
using Netsphere.Server.Game.Services;
using ProudNet;

namespace Netsphere.Server.Game.Handlers
{
    internal class ChannelHandler
        : IHandle<ChannelInfoReqMessage>, IHandle<ChannelEnterReqMessage>, IHandle<ChannelLeaveReqMessage>
    {
        private readonly ILogger _logger;
        private readonly ChannelService _channelService;

        public ChannelHandler(ILogger<ChannelHandler> logger, ChannelService channelService)
        {
            _logger = logger;
            _channelService = channelService;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        public async Task<bool> OnHandle(MessageContext context, ChannelInfoReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            switch (message.Request)
            {
                case ChannelInfoRequest.RoomList:
                case ChannelInfoRequest.RoomList2:
                    if (plr.Channel != null)
                    {
                        var rooms = plr.Channel.RoomManager.Select(x => x.Map<Room, RoomDto>()).ToArray();
                        session.Send(new SGameRoomListAckMessage(message.Request, rooms));
                    }

                    break;

                case ChannelInfoRequest.ChannelList:
                    if (plr.Channel == null)
                    {
                        var channels = _channelService.Select(x => x.Map<Channel, ChannelInfoDto>()).ToArray();
                        session.Send(new SChannelListInfoAckMessage(channels));
                    }

                    break;

                default:
                    plr.AddContextToLogger(_logger).Warning("Invalid channel info request {Request}", message.Request);

                    break;
            }

            return true;
        }

        [Firewall(typeof(MustBeInChannel), Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, ChannelEnterReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            var channel = _channelService[message.Channel];
            if (channel == null)
            {
                session.Send(new SServerResultInfoAckMessage(ServerResult.NonExistingChannel));
                return true;
            }

            var result = channel.Join(plr);
            switch (result)
            {
                case ChannelJoinError.OK:
                    plr.Session.Send(new SServerResultInfoAckMessage(ServerResult.ChannelEnter));
                    break;

                case ChannelJoinError.AlreadyInChannel:
                    plr.Session.Send(new SServerResultInfoAckMessage(ServerResult.JoinChannelFailed));
                    break;

                case ChannelJoinError.ChannelFull:
                    plr.Session.Send(new SServerResultInfoAckMessage(ServerResult.ChannelLimitReached));
                    break;
            }

            return true;
        }

        [Firewall(typeof(MustBeInChannel))]
        [Firewall(typeof(MustBeInRoom), Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, ChannelLeaveReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            plr.Channel.Leave(plr);
            plr.Session.Send(new SServerResultInfoAckMessage(ServerResult.ChannelLeave));
            return true;
        }
    }
}
