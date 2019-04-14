using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netsphere.Database;

namespace Netsphere.Server.Game.Services
{
    public class ChannelService : IHostedService, IReadOnlyCollection<Channel>
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly DatabaseService _databaseService;
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

        public ChannelService(ILogger<ChannelService> logger, DatabaseService databaseService,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _databaseService = databaseService;
            _serviceProvider = serviceProvider;
        }

        public Channel GetChannel(uint id)
        {
            _channels.TryGetValue(id, out var channel);
            return channel;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Creating channels...");

            using (var db = _databaseService.Open<GameContext>())
            {
                var entities = await db.Channels.ToArrayAsync();
                _channels = entities
                    .Select(x =>
                    {
                        var channel = new Channel(x, _serviceProvider.GetRequiredService<RoomManager>());
                        channel.PlayerJoined += (s, e) => OnPlayerJoined(e.Channel, e.Player);
                        channel.PlayerLeft += (s, e) => OnPlayerLeft(e.Channel, e.Player);
                        return channel;
                    })
                    .ToImmutableDictionary(x => x.Id, x => x);
            }

            _logger.Information("Created {Count} channels", _channels.Count);
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
