using System;
using System.Linq;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Foundatio.Messaging;
using Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
    internal class AuthenticationHandler : IHandle<LoginReqMessage>
    {
        private readonly ILogger _logger;
        private readonly NetworkOptions _networkOptions;
        private readonly IMessageBus _messageBus;
        private readonly ISessionManager _sessionManager;
        private readonly DatabaseService _databaseService;
        private readonly IServiceProvider _serviceProvider;
        private readonly PlayerManager _playerManager;

        public AuthenticationHandler(ILogger<AuthenticationHandler> logger, IOptions<NetworkOptions> networkOptions,
            IMessageBus messageBus, ISessionManager sessionManager, DatabaseService databaseService,
            IServiceProvider serviceProvider, PlayerManager playerManager)
        {
            _logger = logger;
            _networkOptions = networkOptions.Value;
            _messageBus = messageBus;
            _sessionManager = sessionManager;
            _databaseService = databaseService;
            _serviceProvider = serviceProvider;
            _playerManager = playerManager;
        }

        public async Task<bool> OnHandle(MessageContext context, LoginReqMessage message)
        {
            var session = context.GetSession<Session>();
            var logger = _logger.ForContext(
                ("RemoteEndPoint", session.RemoteEndPoint.ToString()),
                ("Nickname", message.Nickname),
                ("AccountId", message.AccountId),
                ("SessionId", message.SessionId)
            );

            logger.Debug("Login from {RemoteEndPoint}");

            if (_sessionManager.Sessions.Count >= _networkOptions.MaxSessions)
            {
                session.Send(new LoginAckMessage(1));
                return true;
            }

            var response = await _messageBus.PublishRequestAsync<ChatLoginRequest, ChatLoginResponse>(
                new ChatLoginRequest(message.AccountId, message.SessionId));

            if (!response.OK)
            {
                logger.Information("Wrong login");
                session.Send(new LoginAckMessage(2));
                return true;
            }

            if (!response.Account.Nickname.Equals(message.Nickname))
            {
                logger.Information("Wrong login");
                session.Send(new LoginAckMessage(3));
                return true;
            }

            if (_playerManager.Contains(message.AccountId))
            {
                logger.Information("Already logged in");
                session.Send(new LoginAckMessage(4));
                return true;
            }

            using (var db = _databaseService.Open<GameContext>())
            {
                var accountId = (long)message.AccountId;
                var playerEntity = await db.Players
                    .Include(x => x.Ignores)
                    .Include(x => x.Inbox)
                    .Include(x => x.Settings)
                    .FirstOrDefaultAsync(x => x.Id == accountId);

                if (playerEntity == null)
                {
                    logger.Warning("Could not load player from database");
                    session.Send(new LoginAckMessage(5));
                    return true;
                }

                session.Player = _serviceProvider.GetRequiredService<Player>();
                await session.Player.Initialize(session, response.Account, playerEntity);
                _playerManager.Add(session.Player);
            }

            session.Send(new LoginAckMessage(0));
            session.Send(
                new DenyListAckMessage(session.Player.Ignore.Select(x => x.Map<Deny, DenyDto>()).ToArray()));

            return true;
        }
    }
}
