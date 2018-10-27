using System.Threading.Tasks;
using Foundatio.Caching;
using Microsoft.Extensions.Logging;
using Netsphere.Network.Message.Chat;
using ProudNet;

namespace Netsphere.Server.Chat.Handlers
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
                "RemoteEndPoint={RemoteEndPoint} Nickname={Nickname} AccountId={AccountId} SessionId={SessionId}",
                session.RemoteEndPoint.ToString(), message.Nickname, message.AccountId, message.SessionId))
            {
                _logger.LogDebug("Login from {RemoteEndPoint}");
                await session.SendAsync(new SLoginAckMessage(1));
            }

            return true;
        }
    }
}
