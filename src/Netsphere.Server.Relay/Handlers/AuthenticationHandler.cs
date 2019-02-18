using System;
using System.Threading.Tasks;
using Foundatio.Messaging;
using Logging;
using Microsoft.Extensions.DependencyInjection;
using Netsphere.Common;
using Netsphere.Common.Messaging;
using Netsphere.Network.Message.Relay;
using ProudNet;

namespace Netsphere.Server.Relay.Handlers
{
    internal class AuthenticationHandler : IHandle<CRequestLoginMessage>
    {
        private readonly ILogger _logger;
        private readonly IMessageBus _messageBus;
        private readonly RoomManager _roomManager;
        private readonly PlayerManager _playerManager;
        private readonly IServiceProvider _serviceProvider;

        public AuthenticationHandler(ILogger<AuthenticationHandler> logger, IMessageBus messageBus,
            RoomManager roomManager, PlayerManager playerManager, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _messageBus = messageBus;
            _roomManager = roomManager;
            _playerManager = playerManager;
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> OnHandle(MessageContext context, CRequestLoginMessage message)
        {
            var session = context.GetSession<Session>();
            var logger = _logger.ForContext(
                ("RemoteEndPoint", session.RemoteEndPoint.ToString()),
                ("Message", message.ToJson()));

            logger.Debug("Login from {RemoteEndPoint}");

            var response = await _messageBus.PublishRequestAsync<RelayLoginRequest, RelayLoginResponse>(new RelayLoginRequest
            {
                AccountId = message.AccountId,
                Nickname = message.Nickname,
                Address = session.RemoteEndPoint.Address,
                ServerId = message.RoomLocation.ServerId,
                ChannelId = message.RoomLocation.ChannelId,
                RoomId = message.RoomLocation.RoomId
            });

            if (!response.OK)
            {
                session.Send(new SNotifyLoginResultMessage(1));
                return true;
            }

            if (_playerManager[message.AccountId] != null)
            {
                logger.Information("Already logged in");
                session.Send(new SNotifyLoginResultMessage(2));
                return true;
            }

            var roomId = (uint)((message.RoomLocation.ChannelId << 8) | message.RoomLocation.RoomId);

            var plr = _serviceProvider.GetRequiredService<Player>();
            plr.Initialize(session, response.Account);
            session.Player = plr;
            _playerManager.Add(plr);

            var room = _roomManager.GetOrCreate(roomId);
            room.Join(plr);
            session.Send(new SNotifyLoginResultMessage(0));
            return true;
        }
    }
}
