using System.Threading;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Foundatio.Messaging;
using Logging;
using Microsoft.Extensions.Hosting;
using Netsphere.Common.Messaging;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;

namespace Netsphere.Server.Chat.Services
{
    public class IpcService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IMessageBus _messageBus;
        private readonly PlayerManager _playerManager;
        private readonly ChannelManager _channelManager;
        private readonly CancellationTokenSource _shutdown;

        public IpcService(ILogger<IpcService> logger, IMessageBus messageBus, PlayerManager playerManager,
            ChannelManager channelManager)
        {
            _logger = logger;
            _messageBus = messageBus;
            _playerManager = playerManager;
            _channelManager = channelManager;
            _shutdown = new CancellationTokenSource();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _messageBus.SubscribeAsync<ChannelPlayerJoinedMessage>(OnPlayerJoinedChannel, _shutdown.Token);
            await _messageBus.SubscribeAsync<ChannelPlayerLeftMessage>(OnPlayerLeftChannel, _shutdown.Token);
            await _messageBus.SubscribeAsync<PlayerUpdateMessage>(OnPlayerUpdate, _shutdown.Token);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _shutdown.Cancel();
            return Task.CompletedTask;
        }

        private Task OnPlayerJoinedChannel(ChannelPlayerJoinedMessage message)
        {
            var plr = _playerManager[message.AccountId];
            if (plr == null)
            {
                _logger.Warning("<OnPlayerJoinedChannel> Cant find player={Id}", message.AccountId);
                return Task.CompletedTask;
            }

            var channel = _channelManager.GetOrCreateChannel(message.ChannelId);
            channel.Join(plr);
            return Task.CompletedTask;
        }

        private Task OnPlayerLeftChannel(ChannelPlayerLeftMessage message)
        {
            var plr = _playerManager[message.AccountId];
            if (plr == null)
            {
                _logger.Warning("<OnPlayerLeftChannel> Cant find player={Id}", message.AccountId);
                return Task.CompletedTask;
            }

            var channel = _channelManager.GetChannel(message.ChannelId);
            if (channel == null)
            {
                _logger.Warning("<OnPlayerLeftChannel> Cant find channel={Id}", message.ChannelId);
                return Task.CompletedTask;
            }

            channel.Leave(plr);
            return Task.CompletedTask;
        }

        private Task OnPlayerUpdate(PlayerUpdateMessage message)
        {
            var plr = _playerManager[message.AccountId];
            if (plr == null)
            {
                _logger.Warning("<OnPlayerUpdate> Cant find player={Id}", message.AccountId);
                return Task.CompletedTask;
            }

            plr.TotalExperience = message.TotalExperience;
            plr.RoomId = message.RoomId;
            plr.TeamId = message.TeamId;

            var channel = plr.Channel;
            channel.Broadcast(new SUserDataAckMessage(plr.Map<Player, UserDataDto>()));
            return Task.CompletedTask;
        }
    }
}
