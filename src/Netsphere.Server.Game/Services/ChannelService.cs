using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Netsphere.Server.Game.Services
{
    public class ChannelService : IHostedService, IReadOnlyCollection<Channel>
    {
        private readonly ILogger _logger;
        private readonly GameDataService _gameDataService;
        private readonly IServiceProvider _serviceProvider;
        private ImmutableDictionary<uint, Channel> _channels;

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

        public ChannelService(ILogger<ChannelService> logger, GameDataService gameDataService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _gameDataService = gameDataService;
            _serviceProvider = serviceProvider;
        }

        public Channel GetChannel(uint id)
        {
            _channels.TryGetValue(id, out var channel);
            return channel;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating channels...");

            _channels = _gameDataService.Channels.Select(x =>
            {
                var channel = new Channel(x.Id, x.Category, x.Name, x.PlayerLimit, x.Type,
                    _serviceProvider.GetRequiredService<RoomManager>());
                channel.PlayerJoined += (s, e) => OnPlayerJoined(e.Channel, e.Player);
                channel.PlayerLeft += (s, e) => OnPlayerLeft(e.Channel, e.Player);
                return channel;
            }).ToImmutableDictionary(x => x.Id, x => x);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
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
