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
            using (var db = AuthDatabase.Open())
            {
                var result = await db.FindAsync<AccountDto>(statement => statement
                        .Where($"{nameof(AccountDto.Username):C} = @{nameof(message.Username)}")
                        .Include<BanDto>(join => join.LeftOuterJoin())
                        .WithParameters(new { message.Username }));
                account = result.FirstOrDefault();

                if (account == null)
                {
                    if (Config.Instance.NoobMode || Config.Instance.AutoRegister)
                    {
                        // NoobMode/AutoRegister: Create a new account if non exists
                        account = new AccountDto { Username = message.Username };

                        var newSalt = new byte[24];
                        using (var csprng = new RNGCryptoServiceProvider())
                            csprng.GetBytes(newSalt);
                        
                        var hash = new byte[24];
                        using (var pbkdf2 = new Rfc2898DeriveBytes(message.Password, newSalt, 24000))
                            hash = pbkdf2.GetBytes(24);

                        account.Password = Convert.ToBase64String(hash);
                        account.Salt = Convert.ToBase64String(newSalt);

                        await db.InsertAsync(account);
                    }
                    else
                    {
                        Logger.Error($"Wrong login for {message.Username}");
                        session.SendAsync(new SAuthInEuAckMessage(AuthLoginResult.WrongIdorPw));
                        return;
                    }
                }

                var salt = Convert.FromBase64String(account.Salt);
                
                var passwordGuess = new byte[24];
                using (var pbkdf2 = new Rfc2898DeriveBytes(message.Password, salt, 24000))
                    passwordGuess = pbkdf2.GetBytes(24);
                
                var actualPassword = Convert.FromBase64String(account.Password);
                
                uint difference = (uint)passwordGuess.Length ^ (uint)actualPassword.Length;
                for (var i = 0; i < passwordGuess.Length && i < actualPassword.Length; i++)
                {
                    difference |= (uint)(passwordGuess[i] ^ actualPassword[i]);
                }
                
                if (difference != 0 || string.IsNullOrWhiteSpace(account.Password))
                {
                    if (Config.Instance.NoobMode)
                    {
                        // Noob Mode: Save new password
                        var newSalt = new byte[24];
                        using (var csprng = new RNGCryptoServiceProvider())
                            csprng.GetBytes(newSalt);

                        var hash = new byte[24];
                        using (var pbkdf2 = new Rfc2898DeriveBytes(message.Password, newSalt, 24000))
                            hash = pbkdf2.GetBytes(24);

                        account.Password = Convert.ToBase64String(hash);
                        account.Salt = Convert.ToBase64String(newSalt);

                        await db.UpdateAsync(account);
                    }
                    else
                    {
                        Logger.Error($"Wrong login for {message.Username}");
                        session.SendAsync(new SAuthInEuAckMessage(AuthLoginResult.WrongIdorPw));
                        return;
                    }
                }

                var now = DateTimeOffset.Now.ToUnixTimeSeconds();
                var ban = account.Bans.FirstOrDefault(b => b.Date + (b.Duration ?? 0) > now);
                if (ban != null)
                {
                    var unbanDate = DateTimeOffset.FromUnixTimeSeconds(ban.Date + (ban.Duration ?? 0));
                    Logger.Error($"{message.Username} is banned until {unbanDate}");
                    session.SendAsync(new SAuthInEuAckMessage(unbanDate));
                    return;
                }

                Logger.Info($"Login success for {message.Username}");

                var entry = new LoginHistoryDto
                {
                    AccountId = account.Id,
                    Date = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    IP = ip
                };
                await db.InsertAsync(entry);
            }

            // ToDo proper session generation
            var sessionId = Hash.GetUInt32<CRC32>($"<{account.Username}+{account.Password}>");
            session.SendAsync(new SAuthInEuAckMessage(AuthLoginResult.OK, (ulong)account.Id, sessionId));
        }

        [MessageHandler(typeof(CServerListReqMessage))]
        public void ServerListHandler(AuthServer server, ProudSession session)
        {
            session.SendAsync(new SServerListAckMessage(server.ServerManager.ToArray()), SendOptions.Reliable);
        }
    }
}
