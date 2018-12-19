using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netsphere.Common.Configuration;
using Netsphere.Common.Messaging;
using ProudNet;
using ProudNet.Hosting.Services;

namespace Netsphere.Server.Game.Services
{
    internal class ServerlistService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly ISessionManager _sessionManager;
        private readonly ISchedulerService _scheduler;
        private readonly IMessageBus _messageBus;
        private readonly NetworkOptions _networkOptions;
        private readonly ServerListOptions _serverOptions;
        private bool _isStopped;

        public ServerlistService(ILogger<ServerlistService> logger, ISessionManager sessionManager, ISchedulerService scheduler,
            IMessageBus messageBus, IOptions<NetworkOptions> networkOptions, IOptions<ServerListOptions> serverOptions)
        {
            _logger = logger;
            _sessionManager = sessionManager;
            _scheduler = scheduler;
            _messageBus = messageBus;
            _networkOptions = networkOptions.Value;
            _serverOptions = serverOptions.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting...");
            _scheduler.ScheduleAsync(Update, _serverOptions.UpdateInterval);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping...");
            await _messageBus.PublishAsync(new ServerShutdownMessage
            {
                Id = _serverOptions.Id,
                ServerType = ServerType.Game
            });
            _isStopped = true;
        }

        private async void Update()
        {
            if (_isStopped)
                return;

            try
            {
                _logger.LogDebug("Updating serverlist...");
                await _messageBus.PublishAsync(new ServerUpdateMessage
                {
                    Id = _serverOptions.Id,
                    ServerType = ServerType.Game,
                    Name = _serverOptions.Name,
                    Online = (ushort)_sessionManager.Sessions.Count,
                    Limit = (ushort)_networkOptions.MaxSessions,
                    EndPoint = new IPEndPoint(IPAddress.Parse(_serverOptions.Address), _networkOptions.Listener.Port)
                });
                await _scheduler.ScheduleAsync(Update, _serverOptions.UpdateInterval);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update serverlist");
            }
        }
    }
}
