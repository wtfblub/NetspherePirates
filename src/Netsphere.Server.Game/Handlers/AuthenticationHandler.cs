using System.Threading.Tasks;
using Foundatio.Caching;
using Microsoft.Extensions.Logging;
using Netsphere.Network;
using Netsphere.Network.Message.Game;
using ProudNet;

namespace Netsphere.Server.Game.Handlers
{
    internal class AuthenticationHandler : IHandle<CLoginReqMessage>
    {
        private readonly ILogger _logger;
        private readonly ICacheClient _cacheClient;

        public AuthenticationHandler(ILogger<AuthenticationHandler> logger, ICacheClient cacheClient)
        {
            _logger = logger;
            _cacheClient = cacheClient;
        }

        public async Task<bool> OnHandle(MessageContext context, CLoginReqMessage message)
        {
            var session = context.GetSession<Session>();

            using (_logger.BeginScope(
                "RemoteEndPoint={RemoteEndPoint} Message={@Message}",
                session.RemoteEndPoint.ToString(), message))
            {
                _logger.LogDebug("Login from {RemoteEndPoint}");
                await session.SendAsync(new SLoginAckMessage(GameLoginResult.ServerFull));
            }

            return true;
        }
    }
}
