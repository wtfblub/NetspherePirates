using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Foundatio.Caching;
using LinqToDB;
using Microsoft.Extensions.Logging;
using Netsphere.Common;
using Netsphere.Common.Cryptography;
using Netsphere.Database;
using Netsphere.Database.Auth;
using Netsphere.Network;
using Netsphere.Network.Message.Auth;
using Netsphere.Server.Auth.Rules;
using ProudNet;

namespace Netsphere.Server.Auth.Handlers
{
    internal class AuthenticationHandler : IHandle<CAuthInEUReqMessage>
    {
        private readonly ILogger _logger;
        private readonly IDatabaseProvider _databaseProvider;
        private readonly ICacheClient _cacheClient;
        private readonly RandomNumberGenerator _randomNumberGenerator;

        public AuthenticationHandler(ILogger<AuthenticationHandler> logger, IDatabaseProvider databaseProvider,
            ICacheClient cacheClient)
        {
            _logger = logger;
            _databaseProvider = databaseProvider;
            _cacheClient = cacheClient;
            _randomNumberGenerator = RandomNumberGenerator.Create();
        }

        [Firewall(typeof(MustBeLoggedIn), Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, CAuthInEUReqMessage message)
        {
            var session = context.GetSession<Session>();
            var remoteAddress = session.RemoteEndPoint.Address.ToString();

            using (_logger.BeginScope("RemoteEndPoint={RemoteEndPoint} Username={Username}",
                session.RemoteEndPoint.ToString(), message.Username))
            {
                _logger.LogDebug("Login from {RemoteEndPoint} with username {Username}");

                AccountEntity account;
                using (var db = _databaseProvider.Open<AuthContext>())
                {
                    var username = message.Username.ToLower();
                    account = await db.Accounts
                        .LoadWith(x => x.Bans)
                        .FirstOrDefaultAsync(x => x.Username == username);

                    if (account == null)
                    {
                        _logger.LogInformation("Wrong login");
                        await session.SendAsync(new SAuthInEuAckMessage(AuthLoginResult.WrongLogin));
                        return true;
                    }

                    if (!PasswordHasher.IsPasswordValid(message.Password, account.Password, account.Salt))
                    {
                        _logger.LogInformation("Wrong login");
                        await session.SendAsync(new SAuthInEuAckMessage(AuthLoginResult.WrongLogin));
                        return true;
                    }

                    var now = DateTimeOffset.Now.ToUnixTimeSeconds();
                    var ban = account.Bans.FirstOrDefault(x => x.Duration == null || x.Date + x.Duration > now);

                    if (ban != null)
                    {
                        var unbanDate = DateTimeOffset.MinValue;
                        if (ban.Duration != null)
                            unbanDate = DateTimeOffset.FromUnixTimeSeconds(ban.Date + (ban.Duration ?? 0));

                        _logger.LogInformation("Account is banned until {UnbanDate}", unbanDate);
                        await session.SendAsync(new SAuthInEuAckMessage(unbanDate));
                        return true;
                    }

                    await db.InsertAsync(new LoginHistoryEntity
                    {
                        AccountId = account.Id,
                        Date = now,
                        IP = remoteAddress
                    });
                }

                _logger.LogInformation("Login success");

                var sessionId = NewSessionId();
                await _cacheClient.SetAsync(Constants.Cache.SessionKey(account.Id), sessionId, TimeSpan.FromMinutes(30));
                session.Authenticated = true;
                session.Send(new SAuthInEuAckMessage(AuthLoginResult.OK, (ulong)account.Id, sessionId));
            }

            return true;
        }

        private string NewSessionId()
        {
            Span<byte> bytes = stackalloc byte[16];
            _randomNumberGenerator.GetBytes(bytes);
            return new Guid(bytes).ToString("N");
        }
    }
}
