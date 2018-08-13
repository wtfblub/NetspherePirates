using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netsphere.Database;
using Netsphere.Network;
using Netsphere.Network.Message.Auth;
using ProudNet;
using ProudNet.Handlers;

namespace Netsphere.Server.Auth.Services
{
    internal class AuthService : ProudMessageHandler
    {
        private readonly ILogger _log;
        private readonly AppOptions _options;
        private readonly IDatabaseProvider _databaseProvider;

        public AuthService(ILogger<AuthService> logger, IOptions<AppOptions> options, IDatabaseProvider databaseProvider)
        {
            _log = logger;
            _options = options.Value;
            _databaseProvider = databaseProvider;
        }

        [MessageHandler(typeof(CAuthInEUReqMessage))]
        public async Task CAuthInEUReq(ProudSession session, CAuthInEUReqMessage message)
        {
            using (_log.BeginScope("RemoteEndPoint={RemoteEndPoint} Username={Username}",
                session.RemoteEndPoint.ToString(), message.Username))
            {
                _log.LogDebug("Login from {RemoteEndPoint} with username {Username}");

                session.Send(new SAuthInEuAckMessage(AuthLoginResult.Failed));

                // TODO
            }
        }

        [MessageHandler(typeof(CServerListReqMessage))]
        public async Task CServerListReq(ProudSession session)
        {
            // TODO
            session.Send(new SServerListAckMessage());
        }
    }
}
