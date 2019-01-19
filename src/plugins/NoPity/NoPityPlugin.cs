using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Netsphere.Common;
using Netsphere.Common.Configuration.Hjson;
using Netsphere.Common.Plugins;
using Netsphere.Server.Game;

namespace NoPity
{
    public class NoPityPlugin : IPlugin
    {
        private IConfiguration _configuration;

        public void OnInitialize(IConfiguration appConfiguration)
        {
            var path = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            path = Path.GetDirectoryName(path);
            path = Path.Combine(path, "nopity.hjson");

            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddHjsonFile(path, false, true)
                .Build();
        }

        public void OnConfigure(IServiceCollection services)
        {
            services
                .Configure<NoPityOptions>(_configuration)
                .AddHostedServiceEx<NoPityService>();
        }

        public void OnShutdown()
        {
        }
    }

    public class NoPityService : IHostedService
    {
        private readonly IOptionsMonitor<NoPityOptions> _noPityOptions;

        public NoPityService(IOptionsMonitor<NoPityOptions> noPityOptions)
        {
            _noPityOptions = noPityOptions;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            RoomManager.RoomCreateHook += OnRoomCreateHook;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private bool OnRoomCreateHook(RoomCreateHookEventArgs e)
        {
            if (_noPityOptions.CurrentValue.Enabled)
                e.Options.IsBalanced = false;

            return true;
        }
    }
}
