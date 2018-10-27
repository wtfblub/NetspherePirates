using System;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netsphere.Common.Messaging;
using ProudNet;
using ProudNet.Hosting.Services;

namespace Netsphere.Server.Chat.Services
{
    internal class ServerlistService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly ISessionManager _sessionManager;
        private readonly ISchedulerService _scheduler;
        private readonly IMessageBus _messageBus;
        private readonly ServerOptions _options;
        private bool _isStopped;

        public ServerlistService(ILogger<ServerlistService> logger, ISessionManager sessionManager, ISchedulerService scheduler,
            IMessageBus messageBus, IOptions<ServerOptions> options)
        {
            _logger = logger;
            _sessionManager = sessionManager;
            _scheduler = scheduler;
            _messageBus = messageBus;
            _options = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting...");
            _scheduler.ScheduleAsync(Update, _options.ServerUpdateInterval);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping...");
            _isStopped = true;
            return Task.CompletedTask;
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
                    Id = _options.Id,
                    ServerType = ServerType.Chat,
                    Name = _options.Name,
                    Online = (ushort)_sessionManager.Sessions.Count,
                    Limit = (ushort)_options.PlayerLimit,
                    EndPoint = _options.Listener
                });
                await _scheduler.ScheduleAsync(Update, _options.ServerUpdateInterval);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update serverlist");
            }
        }
    }
}
