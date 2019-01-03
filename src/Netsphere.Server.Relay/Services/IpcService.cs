using System.Threading;
using System.Threading.Tasks;
using Foundatio.Messaging;
using Microsoft.Extensions.Hosting;
using Netsphere.Common.Messaging;

namespace Netsphere.Server.Relay.Services
{
    public class IpcService : IHostedService
    {
        private readonly IMessageBus _messageBus;
        private readonly PlayerManager _playerManager;
        private readonly CancellationTokenSource _shutdown;

        public IpcService(IMessageBus messageBus, PlayerManager playerManager)
        {
            _messageBus = messageBus;
            _playerManager = playerManager;
            _shutdown = new CancellationTokenSource();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _messageBus.SubscribeAsync<PlayerDisconnectedMessage>(OnPlayerDisconnected, _shutdown.Token);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _shutdown.Cancel();
            return Task.CompletedTask;
        }

        private Task OnPlayerDisconnected(PlayerDisconnectedMessage message)
        {
            var plr = _playerManager[message.AccountId];
            plr?.Disconnect();
            return Task.CompletedTask;
        }
    }
}
