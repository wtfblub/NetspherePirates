using System;
using System.Threading.Tasks;
using Foundatio.Messaging;
using LinqToDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netsphere.Common;
using Netsphere.Common.Configuration;
using Netsphere.Common.Messaging;
using Netsphere.Database;
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
        private readonly IdGeneratorService _idGeneratorService;
        private readonly IServiceProvider _serviceProvider;

        public AuthenticationHandler(ILogger<AuthenticationHandler> logger, IOptions<NetworkOptions> networkOptions,
            IMessageBus messageBus, ISessionManager sessionManager, IDatabaseProvider databaseProvider,
            IdGeneratorService idGeneratorService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _networkOptions = networkOptions.Value;
            _messageBus = messageBus;
            _sessionManager = sessionManager;
            _databaseProvider = databaseProvider;
            _idGeneratorService = idGeneratorService;
            _serviceProvider = serviceProvider;
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
                        await session.SendAsync(new SLoginAckMessage(3));
                        return true;
                    }

                    session.AccountId = message.AccountId;
                    session.Mailbox = _serviceProvider.GetRequiredService<Mailbox>();
                    await session.Mailbox.Initialize();

                    session.Ignore = _serviceProvider.GetRequiredService<DenyManager>();
                    await session.Ignore.Initialize(session, playerEntity);

                    session.Settings = new PlayerSettingManager(session, playerEntity, _idGeneratorService);
                }

                await session.SendAsync(new SLoginAckMessage(0));
            }

            return true;
        }
    }
}
