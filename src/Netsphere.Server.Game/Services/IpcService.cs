using System.Threading;
using System.Threading.Tasks;
using Foundatio.Messaging;
using Microsoft.Extensions.Hosting;
using Netsphere.Common;
using Netsphere.Common.Messaging;

namespace Netsphere.Server.Game.Services
{
    public class IpcService : IHostedService
    {
        private readonly IMessageBus _messageBus;
        private readonly PlayerManager _playerManager;
        private readonly CancellationTokenSource _cts;

        public IpcService(IMessageBus messageBus, PlayerManager playerManager)
        {
            _messageBus = messageBus;
            _playerManager = playerManager;
            _cts = new CancellationTokenSource();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _messageBus.SubscribeToRequestAsync<ChatLoginRequest, ChatLoginResponse>(OnChatLogin, _cts.Token);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }

        private Task<ChatLoginResponse> OnChatLogin(ChatLoginRequest request)
        {
            var plr = _playerManager[request.AccountId];
            if (plr == null || plr.Session.SessionId != request.SessionId)
                return Task.FromResult(new ChatLoginResponse(false));

            return Task.FromResult(new ChatLoginResponse(true));
        }
    }
}
