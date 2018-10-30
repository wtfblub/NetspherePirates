using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netsphere.Common;
using Netsphere.Common.Messaging;
using Netsphere.Network.Data.Auth;

namespace Netsphere.Server.Auth.Services
{
    internal class ServerlistService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IMessageBus _messageBus;
        private readonly AppOptions _options;
        private readonly CancellationTokenSource _shutdown;
        private readonly ReaderWriterLockSlim _mutex;
        private readonly IDictionary<uint, ServerInfo> _servers;
        private DateTimeOffset _lastTimeoutCheck;

        public ServerlistService(ILogger<ServerlistService> logger, IMessageBus messageBus, IOptions<AppOptions> options)
        {
            _logger = logger;
            _messageBus = messageBus;
            _options = options.Value;
            _shutdown = new CancellationTokenSource();
            _mutex = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            _servers = new Dictionary<uint, ServerInfo>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting...");
            await _messageBus.SubscribeAsync((Action<ServerUpdateMessage>)OnServerUpdate, _shutdown.Token);
            await _messageBus.SubscribeAsync((Action<ServerShutdownMessage>)OnServerShutdown, _shutdown.Token);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping...");
            _shutdown.Cancel();
            return Task.CompletedTask;
        }

        public ServerInfoDto[] GetServerList()
        {
            CheckServerTimeoutIfNeeded();

            _mutex.EnterReadLock();
            try
            {
                return _servers.Values.Select(x => x.Server).ToArray();
            }
            finally
            {
                _mutex.ExitReadLock();
            }
        }

        private void OnServerUpdate(ServerUpdateMessage message)
        {
            using (_logger.BeginScope("Message={Message}", message.ToJson()))
            {
                _mutex.EnterWriteLock();

                try
                {
                    _logger.LogDebug("Updating...");
                    var id = (uint)(message.Id << 8 | (byte)message.ServerType);
                    _servers[id] = new ServerInfo(new ServerInfoDto
                    {
                        Id = id,
                        GroupId = message.Id,
                        Type = message.ServerType,
                        Name = message.Name,
                        PlayerLimit = message.Limit,
                        PlayerOnline = message.Online,
                        EndPoint = message.EndPoint,
                        IsEnabled = true
                    }, DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to update server");
                }
                finally
                {
                    _mutex.ExitWriteLock();
                }
            }
        }

        private void OnServerShutdown(ServerShutdownMessage message)
        {
            using (_logger.BeginScope("Message={Message}", message.ToJson()))
            {
                _mutex.EnterWriteLock();

                try
                {
                    _logger.LogDebug("Removign...");
                    var id = (uint)(message.Id << 8 | (byte)message.ServerType);
                    _servers.Remove(id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to remove server");
                }
                finally
                {
                    _mutex.ExitWriteLock();
                }
            }
        }

        private void CheckServerTimeoutIfNeeded()
        {
            if (!IsNeeded())
                return;

            _mutex.EnterWriteLock();

            try
            {
                if (!IsNeeded())
                    return;

                _logger.LogInformation("Checking for server timeout...");
                var now = DateTimeOffset.Now;
                foreach (var pair in _servers.Reverse())
                {
                    if (now - pair.Value.LastUpdate > _options.ServerlistTimeout)
                    {
                        _logger.LogInformation("Server timeout {Name}({Id}-{Type})",
                            pair.Value.Server.Name, pair.Value.Server.Id, pair.Value.Server.Type);
                        _servers.Remove(pair.Key);
                    }
                }

                _lastTimeoutCheck = now;
            }
            finally
            {
                _mutex.ExitWriteLock();
            }

            bool IsNeeded()
            {
                return DateTimeOffset.Now - _lastTimeoutCheck > _options.ServerlistTimeout;
            }
        }

        private struct ServerInfo
        {
            public readonly ServerInfoDto Server;
            public readonly DateTimeOffset LastUpdate;

            public ServerInfo(ServerInfoDto server, DateTimeOffset lastUpdate)
            {
                Server = server;
                LastUpdate = lastUpdate;
            }
        }
    }
}
