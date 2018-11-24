using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Netsphere.Common.Messaging;

namespace Netsphere.Server.Chat.Services
{
    public class ChannelService : IHostedService, IReadOnlyCollection<Channel>
    {
        private readonly ILogger _logger;
        private readonly IMessageBus _messageBus;
        private readonly PlayerManager _playerManager;
        private readonly IDictionary<uint, Channel> _channels;
        private readonly CancellationTokenSource _shutdown;

        public Channel this[uint id] => GetChannel(id);

        public event EventHandler<ChannelEventArgs> PlayerJoined;
        public event EventHandler<ChannelEventArgs> PlayerLeft;

        protected virtual void OnPlayerJoined(Channel channel, Player plr)
        {
            PlayerJoined?.Invoke(this, new ChannelEventArgs(channel, plr));
        }

        protected virtual void OnPlayerLeft(Channel channel, Player plr)
        {
            PlayerLeft?.Invoke(this, new ChannelEventArgs(channel, plr));
        }

        public ChannelService(ILogger<ChannelService> logger, IMessageBus messageBus, PlayerManager playerManager)
        {
            _logger = logger;
            _messageBus = messageBus;
            _playerManager = playerManager;
            _channels = new ConcurrentDictionary<uint, Channel>();
            _shutdown = new CancellationTokenSource();
        }

        public Channel GetChannel(uint id)
        {
            _channels.TryGetValue(id, out var channel);
            return channel;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _messageBus.SubscribeAsync<ChannelPlayerJoinedMessage>(ChannelPlayerJoined, _shutdown.Token);
            await _messageBus.SubscribeAsync<ChannelPlayerLeftMessage>(ChannelPlayerLeft, _shutdown.Token);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _shutdown.Cancel();
            return Task.CompletedTask;
        }

        private Channel GetOrCreateChannel(uint id)
        {
            var channel = GetChannel(id);
            if (channel != null)
                return channel;

            channel = new Channel(id);
            if (_channels.TryAdd(id, channel))
            {
                channel.PlayerJoined += (_, e) => OnPlayerJoined(e.Channel, e.Player);
                channel.PlayerLeft += (_, e) => OnPlayerLeft(e.Channel, e.Player);
            }
            else
            {
                return GetChannel(id);
            }

            return channel;
        }

        private Task ChannelPlayerJoined(ChannelPlayerJoinedMessage message)
        {
            var plr = _playerManager[message.AccountId];
            if (plr == null)
            {
                _logger.LogWarning("<ChannelPlayerJoined> Cant find player={Id}", message.AccountId);
                return Task.CompletedTask;
            }

            var channel = GetOrCreateChannel(message.ChannelId);
            channel.Join(plr);
            return Task.CompletedTask;
        }

        private Task ChannelPlayerLeft(ChannelPlayerLeftMessage message)
        {
            var plr = _playerManager[message.AccountId];
            if (plr == null)
            {
                _logger.LogWarning("<ChannelPlayerLeft> Cant find player={Id}", message.AccountId);
                return Task.CompletedTask;
            }

            var channel = GetChannel(message.ChannelId);
            if (channel == null)
            {
                _logger.LogWarning("<ChannelPlayerLeft> Cant find channel={Id}", message.ChannelId);
                return Task.CompletedTask;
            }

            channel.Leave(plr);
            return Task.CompletedTask;
        }

        #region IReadOnlyCollection
        public int Count => _channels.Count;

        public IEnumerator<Channel> GetEnumerator()
        {
            return _channels.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
