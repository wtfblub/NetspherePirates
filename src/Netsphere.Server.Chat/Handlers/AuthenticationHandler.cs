using System;
using System.Linq;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Foundatio.Messaging;
using LinqToDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netsphere.Common;
using Netsphere.Common.Configuration;
using Netsphere.Common.Messaging;
using Netsphere.Database;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;
using ProudNet;

namespace Netsphere.Server.Chat.Handlers
{
    internal class AuthenticationHandler : IHandle<CLoginReqMessage>
    {
        private readonly ILogger _logger;
        private readonly NetworkOptions _networkOptions;
        private readonly IMessageBus _messageBus;
        private readonly ISessionManager _sessionManager;
        private readonly IDatabaseProvider _databaseProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly PlayerManager _playerManager;

        public AuthenticationHandler(ILogger<AuthenticationHandler> logger, IOptions<NetworkOptions> networkOptions,
            IMessageBus messageBus, ISessionManager sessionManager, IDatabaseProvider databaseProvider,
            IServiceProvider serviceProvider, PlayerManager playerManager)
        {
            _logger = logger;
            _networkOptions = networkOptions.Value;
            _messageBus = messageBus;
            _sessionManager = sessionManager;
            _databaseProvider = databaseProvider;
            _serviceProvider = serviceProvider;
            _playerManager = playerManager;
        }

        public async Task<bool> OnHandle(MessageContext context, CLoginReqMessage message)
        {
            var session = context.GetSession<Session>();

            using (_logger.BeginScope(
                "RemoteEndPoint={RemoteEndPoint} Nickname={Nickname} AccountId={AccountId} SessionId={SessionId}",
                session.RemoteEndPoint.ToString(), message.Nickname, message.AccountId, message.SessionId))
            {
                _logger.LogDebug("Login from {RemoteEndPoint}");

                if (_sessionManager.Sessions.Count >= _networkOptions.MaxSessions)
                {
                    await session.SendAsync(new SLoginAckMessage(1));
                    return true;
                }

                var response = await _messageBus.PublishRequestAsync<ChatLoginRequest, ChatLoginResponse>(
                    new ChatLoginRequest(message.AccountId, message.SessionId));

                if (!response.OK)
                {
                    _logger.LogInformation("Wrong login");
                    await session.SendAsync(new SLoginAckMessage(2));
                    return true;
                }

                if (!response.Account.Nickname.Equals(message.Nickname))
                {
                    _logger.LogInformation("Wrong login");
                    await session.SendAsync(new SLoginAckMessage(3));
                    return true;
                }

                using (var db = _databaseProvider.Open<GameContext>())
                {
                    var accountId = (int)message.AccountId;
                    var playerEntity = await db.Players
                        .LoadWith(x => x.Ignores)
                        .LoadWith(x => x.Inbox)
                        .LoadWith(x => x.Settings)
                        .FirstOrDefaultAsync(x => x.Id == accountId);

                    if (playerEntity == null)
                    {
                        _logger.LogWarning("Could not load player from database");
                        await session.SendAsync(new SLoginAckMessage(4));
                        return true;
                    }

                    session.Player = _serviceProvider.GetRequiredService<Player>();
                    await session.Player.Initialize(session, response.Account, playerEntity);
                    _playerManager.Add(session.Player);
                }

                await session.SendAsync(new SLoginAckMessage(0));
                await session.SendAsync(
                    new SDenyChatListAckMessage(session.Player.Ignore.Select(x => x.Map<Deny, DenyDto>()).ToArray()));
            }

            return true;
        }
    }
}
