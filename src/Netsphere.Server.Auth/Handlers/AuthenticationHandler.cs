using System;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.Security.Cryptography;
using LinqToDB;
using Microsoft.Extensions.Logging;
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

        public AuthenticationHandler(ILogger<AuthenticationHandler> logger, IDatabaseProvider databaseProvider)
        {
            _logger = logger;
            _databaseProvider = databaseProvider;
        }

        [Firewall(typeof(MustBeLoggedIn), Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, CAuthInEUReqMessage message)
        {
            var session = context.Session;
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
                        .Where(x => x.Username == username)
                        .FirstOrDefaultAsync();

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
                    var ban = account.Bans.First(x => x.Duration == null || x.Date + x.Duration > now);

                    if (ban != null)
                    {
                        var unbanDate = DateTimeOffset.MinValue;
                        if (ban.Duration != null)
                            unbanDate = DateTimeOffset.FromUnixTimeSeconds(ban.Date + (ban.Duration ?? 0));

                        _logger.LogInformation("Account is banned until {UnbanDate}", username, unbanDate);
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

                // TODO proper session ids
                var sessionId = Hash.GetUInt32<CRC32>($"<{account.Username}+{account.Password}>");
                session.Send(new SAuthInEuAckMessage(AuthLoginResult.OK, (ulong)account.Id, sessionId.ToString()));
            }

            return true;
        }
    }
}
