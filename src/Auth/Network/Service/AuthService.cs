using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BlubLib.Network;
using BlubLib.Network.Pipes;
using BlubLib.Network.Transport.Sockets;
using BlubLib.Security.Cryptography;
using Netsphere.Network;
using Netsphere.Network.Message.Auth;
using Newtonsoft.Json;
using NLog;
using NLog.Fluent;
using Shaolinq;

namespace Auth.Network.Service
{
    internal class AuthService : BlubLib.Network.Pipes.Service
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [MessageHandler(typeof(CAuthInEUReqMessage))]
        public async Task LoginHandler(ISession session, CAuthInEUReqMessage message)
        {
            _logger.Debug()
                    .Message("Login from {0} - {1}{2}", message.Username, Environment.NewLine, JsonConvert.SerializeObject(message, Formatting.Indented))
                    .Write();

            var db = AuthDatabase.Instance;
            var account = await db.Accounts.FirstOrDefaultAsync(acc => acc.Username == message.Username)
                .ConfigureAwait(false);

            if (account == null)
            {
                _logger.Error()
                    .Message("Wrong login for {0}", message.Username)
                    .Write();
                await session.SendAsync(new SAuthInEuAckMessage(AuthLoginResult.WrongIdorPw))
                    .ConfigureAwait(false);
                return;
            }

            var password = Hash.GetString<SHA1CryptoServiceProvider>(message.Password + "+" + account.Salt);
            if (!account.Password.Equals(password, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.Error()
                    .Message("Wrong login for {0}", message.Username)
                    .Write();
                await session.SendAsync(new SAuthInEuAckMessage(AuthLoginResult.WrongIdorPw))
                    .ConfigureAwait(false);
                return;
            }

            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            var ban = account.Bans.FirstOrDefault(b => b.Date + b.Duration > now);
            if (ban != null)
            {
                var unbanDate = DateTimeOffset.FromUnixTimeSeconds(ban.Date + ban.Duration);
                _logger.Error()
                    .Message("{0} is banned until {1}", message.Username, unbanDate)
                    .Write();
                await session.SendAsync(new SAuthInEuAckMessage(unbanDate))
                    .ConfigureAwait(false);
                return;
            }

            _logger.Info()
                .Message("Login success for {0}", message.Username)
                .Write();

            using (var scope = new DataAccessScope())
            {
                var entry = account.LoginHistory.Create();
                entry.Date = DateTimeOffset.Now.ToUnixTimeSeconds();
                entry.IP = ((IPEndPoint) ((TcpProcessor) session.Processor).Socket.RemoteEndPoint).Address.ToString();

                await scope.CompleteAsync()
                    .ConfigureAwait(false);
            }

            // ToDo proper session generation
            var sessionId = Hash.GetUInt32<CRC32>($"<{account.Username}+{password}>");
            await session.SendAsync(new SAuthInEuAckMessage(AuthLoginResult.OK, (ulong)account.Id, sessionId))
                    .ConfigureAwait(false);
        }

        [MessageHandler(typeof(CServerListReqMessage))]
        public Task ServerListHandler(AuthServer server, ISession session)
        {
            return session.SendAsync(new SServerListAckMessage());
        }
    }
}
