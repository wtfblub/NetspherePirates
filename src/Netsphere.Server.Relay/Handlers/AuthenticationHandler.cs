using System;
using System.Threading.Tasks;
using Foundatio.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

            using (_logger.BeginScope("RemoteEndPoint={RemoteEndPoint} Message={@Message}",
                session.RemoteEndPoint.ToString(), message.ToJson()))
            {
                _logger.LogDebug("Login from {RemoteEndPoint}");

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
                    await session.SendAsync(new SNotifyLoginResultMessage(1));
                    return true;
                }

                if (_playerManager[message.AccountId] != null)
                {
                    _logger.LogInformation("Already logged in");
                    await session.SendAsync(new SNotifyLoginResultMessage(2));
                    return true;
                }

                var plr = _serviceProvider.GetRequiredService<Player>();
                plr.Initialize(session, response.Account);
                session.Player = plr;
                var room = _roomManager.GetOrCreate(message.RoomLocation.RoomId);
                await room.Join(plr);
                await session.SendAsync(new SNotifyLoginResultMessage(0));
            }

            return true;
        }
    }
}
