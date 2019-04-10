using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundatio.Caching;
using Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Netsphere.Common;
using Netsphere.Common.Configuration;
using Netsphere.Database;
using Netsphere.Database.Auth;
using Netsphere.Database.Game;
using Netsphere.Network;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Rules;
using Netsphere.Server.Game.Services;
using ProudNet;
using Z.EntityFramework.Plus;
using Constants = Netsphere.Common.Constants;

namespace Netsphere.Server.Game.Handlers
{
    internal class AuthenticationHandler
        : IHandle<LoginRequestReqMessage>,
          IHandle<CCheckNickReqMessage>,
          IHandle<NickCheckReqMessage>
    {
        private readonly ILogger _logger;
        private readonly NetworkOptions _networkOptions;
        private readonly IOptionsMonitor<AppOptions> _appOptions;
        private readonly GameOptions _gameOptions;
        private readonly ICacheClient _cacheClient;
        private readonly ISessionManager _sessionManager;
        private readonly DatabaseService _databaseService;
        private readonly IServiceProvider _serviceProvider;
        private readonly PlayerManager _playerManager;
        private readonly GameDataService _gameDataService;

        public AuthenticationHandler(ILogger<AuthenticationHandler> logger,
            IOptions<NetworkOptions> networkOptions, IOptionsMonitor<AppOptions> appOptions, IOptions<GameOptions> gameOptions,
            ICacheClient cacheClient, ISessionManager sessionManager, DatabaseService databaseService,
            IServiceProvider serviceProvider, PlayerManager playerManager, GameDataService gameDataService)
        {
            _logger = logger;
            _networkOptions = networkOptions.Value;
            _appOptions = appOptions;
            _gameOptions = gameOptions.Value;
            _cacheClient = cacheClient;
            _sessionManager = sessionManager;
            _databaseService = databaseService;
            _serviceProvider = serviceProvider;
            _playerManager = playerManager;
            _gameDataService = gameDataService;
        }

        [Firewall(typeof(MustBeLoggedIn), Invert = true)]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, LoginRequestReqMessage message)
        {
            var session = context.GetSession<Session>();
            var logger = _logger.ForContext(
                ("RemoteEndPoint", session.RemoteEndPoint.ToString()),
                ("Message", message.ToJson()));

            logger.Debug("Login");

            var allowedVersions = _appOptions.CurrentValue.ClientVersions;
            if (allowedVersions.All(x => message.Version != x))
            {
                logger.Information("Invalid client version={Version} supported versions are {SupportedVersions}",
                    message.Version.ToString(), string.Join(",", allowedVersions.Select(x => x.ToString())));
                session.Send(new SLoginAckMessage(GameLoginResult.WrongVersion));
                await session.CloseAsync();
                return true;
            }

            if (_sessionManager.Sessions.Count >= _networkOptions.MaxSessions)
            {
                session.Send(new SLoginAckMessage(GameLoginResult.ServerFull));
                return true;
            }

            // Validate session
            var sessionId = await _cacheClient.GetAsync<string>(Constants.Cache.SessionKey(message.AccountId));
            if (!sessionId.HasValue || !sessionId.Value.Equals(message.SessionId))
            {
                logger.Information("Invalid session id");
                session.Send(new SLoginAckMessage(GameLoginResult.SessionTimeout));
                return true;
            }

            AccountEntity accountEntity;
            using (var db = _databaseService.Open<AuthContext>())
            {
                var accountId = (long)message.AccountId;
                accountEntity = await db.Accounts
                    .Include(x => x.Bans)
                    .FirstOrDefaultAsync(x => x.Id == accountId);
            }

            if (accountEntity == null)
            {
                logger.Information("Wrong login");
                session.Send(new SLoginAckMessage(GameLoginResult.SessionTimeout));
                return true;
            }

            // Check ban status
            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            var ban = accountEntity.Bans.FirstOrDefault(x => x.Duration == null || x.Date + x.Duration > now);
            if (ban != null)
            {
                var unbanDate = DateTimeOffset.MinValue;
                if (ban.Duration != null)
                    unbanDate = DateTimeOffset.FromUnixTimeSeconds(ban.Date + (ban.Duration ?? 0));

                logger.Information("Account is banned until {UnbanDate}", unbanDate);
                session.Send(new SLoginAckMessage(GameLoginResult.SessionTimeout));
                return true;
            }

            var account = new Account((ulong)accountEntity.Id, accountEntity.Username, accountEntity.Nickname,
                (SecurityLevel)accountEntity.SecurityLevel);

            if (message.KickConnection)
            {
                var oldPlr = _playerManager[account.Id];
                if (oldPlr != null)
                {
                    logger.Information("Kicking old connection hostId={HostId}", oldPlr.Session.HostId);
                    await oldPlr.DisconnectAsync();
                }
            }

            if (_playerManager.Contains(account.Id))
            {
                // TODO Check if logged in on another server

                logger.Information("Account is already logged in");
                session.Send(new SLoginAckMessage(GameLoginResult.TerminateOtherConnection));
                return true;
            }

            using (var db = _databaseService.Open<GameContext>())
            {
                var plr = await db.Players
                    .Include(x => x.Characters)
                    .Include(x => x.Items)
                    .Include(x => x.Licenses)
                    .FirstOrDefaultAsync(x => x.Id == accountEntity.Id);

                if (plr == null)
                {
                    var levelInfo = _gameDataService.Levels.GetValueOrDefault(_gameOptions.StartLevel);
                    if (levelInfo == null)
                        logger.Warning("Invalid StartLevel={StartLevel} in config", _gameOptions.StartLevel);

                    plr = new PlayerEntity
                    {
                        Id = (int)account.Id,
                        AP = _gameOptions.StartAP,
                        PEN = _gameOptions.StartPEN,
                        Coins1 = _gameOptions.StartCoins1,
                        Coins2 = _gameOptions.StartCoins2,
                        TotalExperience = (int)(levelInfo?.TotalExperience ?? 0)
                    };

                    db.Players.Add(plr);
                    await db.SaveChangesAsync();
                }

                session.Player = _serviceProvider.GetRequiredService<Player>();
                session.Player.Initialize(session, account, plr);
                session.SessionId = message.SessionId;
            }

            _playerManager.Add(session.Player);
            logger.Information("Login success");

            var result = string.IsNullOrWhiteSpace(account.Nickname)
                ? GameLoginResult.ChooseNickname
                : GameLoginResult.OK;
            session.Send(new SLoginAckMessage(result, account.Id));

            if (!string.IsNullOrWhiteSpace(account.Nickname))
                await session.Player.SendAccountInformation();
            return true;
        }

        [Firewall(typeof(MustNotHaveANickname))]
        public async Task<bool> OnHandle(MessageContext context, CCheckNickReqMessage message)
        {
            var session = context.Session;
            var logger = _logger.ForContext(
                ("RemoteEndPoint", session.RemoteEndPoint.ToString()),
                ("Nickname", message.Nickname));

            var available = await IsNickAvailableAsync(message.Nickname);
            if (!available)
                logger.Debug("Nickname not available");

            session.Send(new SCheckNickAckMessage(!available));
            return true;
        }

        [Firewall(typeof(MustNotHaveANickname))]
        public async Task<bool> OnHandle(MessageContext context, NickCheckReqMessage message)
        {
            var session = (Session)context.Session;
            var logger = _logger.ForContext(
                ("RemoteEndPoint", session.RemoteEndPoint.ToString()),
                ("Nickname", message.Nickname));

            var available = await IsNickAvailableAsync(message.Nickname);
            if (!available)
            {
                logger.Debug("Nickname not available");
                session.Send(new SCheckNickAckMessage(true));
            }

            session.Player.Account.Nickname = message.Nickname;
            using (var db = _databaseService.Open<AuthContext>())
            {
                var accountId = (long)session.Player.Account.Id;
                await db.Accounts
                    .Where(x => x.Id == accountId)
                    .UpdateAsync(x => new AccountEntity { Nickname = message.Nickname });
            }

            session.Send(new SServerResultInfoAckMessage(ServerResult.CreateNicknameSuccess));
            await session.Player.SendAccountInformation();
            session.Player.OnNicknameCreated(message.Nickname);
            return true;
        }

        private async Task<bool> IsNickAvailableAsync(string nickname)
        {
            var minLength = _gameOptions.NickRestrictions.MinLength;
            var maxLength = _gameOptions.NickRestrictions.MaxLength;
            var whitespace = _gameOptions.NickRestrictions.WhitespaceAllowed;
            var ascii = _gameOptions.NickRestrictions.AsciiOnly;
            if (string.IsNullOrWhiteSpace(nickname) ||
                !whitespace && nickname.Contains(" ") ||
                nickname.Length < minLength ||
                nickname.Length > maxLength ||
                ascii && Encoding.UTF8.GetByteCount(nickname) != nickname.Length)
            {
                return false;
            }

            // check for repeating chars example: (AAAHello, HeLLLLo)
            var maxRepeat = _gameOptions.NickRestrictions.MaxRepeat;
            if (maxRepeat > 0)
            {
                var counter = 1;
                var current = nickname[0];
                for (var i = 1; i < nickname.Length; i++)
                {
                    if (current != nickname[i])
                    {
                        if (counter > maxRepeat)
                            return false;

                        counter = 0;
                        current = nickname[i];
                    }

                    ++counter;
                }

                if (counter > maxRepeat)
                    return false;
            }

            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            using (var db = _databaseService.Open<AuthContext>())
            {
                var nickExists = await db.Accounts.AnyAsync(x => x.Nickname == nickname);
                var nickReserved = await db.Nicknames.AnyAsync(x =>
                    x.Nickname == nickname && (x.ExpireDate == -1 || x.ExpireDate > now));
                return !nickExists && !nickReserved;
            }
        }
    }
}
