using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using BlubLib.Security.Cryptography;
using Dapper.FastCrud;
using Netsphere.Database.Auth;
using Netsphere.Network.Message.Auth;
using NLog;
using ProudNet;
using ProudNet.Handlers;

namespace Netsphere.Network.Service
{
    internal class AuthService : ProudMessageHandler
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [MessageHandler(typeof(CAuthInEUReqMessage))]
        public async Task LoginHandler(ProudSession session, CAuthInEUReqMessage message)
        {
            var ip = session.RemoteEndPoint.Address.ToString();
            Logger.Debug($"Login from {ip} with username {message.Username}");

            AccountDto account;
            string password;
            using (var db = AuthDatabase.Open())
            {
                var result = await db.FindAsync<AccountDto>(statement => statement
                        .Where($"{nameof(AccountDto.Username):C} = @{nameof(message.Username)}")
                        .Include<BanDto>(join => join.LeftOuterJoin())
                        .WithParameters(new { message.Username }))
                    .ConfigureAwait(false);
                account = result.FirstOrDefault();

                if (account == null)
                {
                    if (Config.Instance.NoobMode)
                    {
                        // NoobMode: Create a new account if non exists
                        account = new AccountDto { Username = message.Username };

                        var bytes = new byte[16];
                        using (var rng = new RNGCryptoServiceProvider())
                            rng.GetBytes(bytes);

                        account.Salt = Hash.GetString<SHA1CryptoServiceProvider>(bytes);
                        account.Password = Hash.GetString<SHA1CryptoServiceProvider>(message.Password + "+" + account.Salt);

                        await db.InsertAsync(account)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        Logger.Error($"Wrong login for {message.Username}");
                        await session.SendAsync(new SAuthInEuAckMessage(AuthLoginResult.WrongIdorPw))
                            .ConfigureAwait(false);
                        return;
                    }
                }

                password = Hash.GetString<SHA1CryptoServiceProvider>(message.Password + "+" + account.Salt);
                if (string.IsNullOrWhiteSpace(account.Password) || !account.Password.Equals(password, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (Config.Instance.NoobMode)
                    {
                        // Noob Mode: Save new password
                        var bytes = new byte[16];
                        using (var rng = new RNGCryptoServiceProvider())
                            rng.GetBytes(bytes);

                        var salt = Hash.GetString<SHA1CryptoServiceProvider>(bytes);
                        password = Hash.GetString<SHA1CryptoServiceProvider>(message.Password + "+" + salt);
                        account.Password = password;
                        account.Salt = salt;

                        await db.UpdateAsync(account)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        Logger.Error($"Wrong login for {message.Username}");
                        await session.SendAsync(new SAuthInEuAckMessage(AuthLoginResult.WrongIdorPw))
                            .ConfigureAwait(false);
                        return;
                    }
                }

                var now = DateTimeOffset.Now.ToUnixTimeSeconds();
                var ban = account.Bans.FirstOrDefault(b => b.Date + (b.Duration ?? 0) > now);
                if (ban != null)
                {
                    var unbanDate = DateTimeOffset.FromUnixTimeSeconds(ban.Date + (ban.Duration ?? 0));
                    Logger.Error($"{message.Username} is banned until {unbanDate}");
                    await session.SendAsync(new SAuthInEuAckMessage(unbanDate))
                        .ConfigureAwait(false);
                    return;
                }

                Logger.Info($"Login success for {message.Username}");

                var entry = new LoginHistoryDto
                {
                    AccountId = account.Id,
                    Date = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    IP = ip
                };
                await db.InsertAsync(entry)
                    .ConfigureAwait(false);
            }

            // ToDo proper session generation
            var sessionId = Hash.GetUInt32<CRC32>($"<{account.Username}+{password}>");
            await session.SendAsync(new SAuthInEuAckMessage(AuthLoginResult.OK, (ulong)account.Id, sessionId))
                    .ConfigureAwait(false);
        }

        [MessageHandler(typeof(CServerListReqMessage))]
        public Task ServerListHandler(AuthServer server, ProudSession session)
        {
            return session.SendAsync(new SServerListAckMessage(server.ServerManager.ToArray()));
        }
    }
}
