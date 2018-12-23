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

namespace SoloMode
{
    public class SoloMode : IPlugin
    {
        private IConfiguration _configuration;

        public void OnInitialize()
        {
            var path = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            path = Path.GetDirectoryName(path);
            path = Path.Combine(path, "solomode.hjson");

            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddHjsonFile(path, false, true)
                .Build();
        }

        public void OnConfigure(IServiceCollection services)
        {
            services
                .Configure<SoloModeOptions>(_configuration)
                .AddHostedServiceEx<SoloModeService>();
        }

        public void OnShutdown()
        {
        }
    }

    public class SoloModeService : IHostedService
    {
        private readonly IOptionsMonitor<SoloModeOptions> _soloModeOptions;

        public SoloModeService(IOptionsMonitor<SoloModeOptions> soloModeOptions)
        {
            _soloModeOptions = soloModeOptions;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            GameRuleBase.CanStartGameHook += OnCanStartGameHook;
            GameRuleBase.HasEnoughPlayersHook += OnHasEnoughPlayersHook;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private bool OnCanStartGameHook(CanStartGameHookEventArgs e)
        {
            var options = _soloModeOptions.CurrentValue;
            if (options.Enabled &&
                e.GameRule.Room.Options.Name.Contains(options.Trigger, StringComparison.OrdinalIgnoreCase))
            {
                e.Result = true;
                return false;
            }

            return true;
        }

        private bool OnHasEnoughPlayersHook(HasEnoughPlayersHookEventArgs e)
        {
            var options = _soloModeOptions.CurrentValue;
            if (options.Enabled &&
                e.GameRule.Room.Options.Name.Contains(_soloModeOptions.CurrentValue.Trigger, StringComparison.OrdinalIgnoreCase))
            {
                e.Result = true;
                return false;
            }

            return true;
        }
    }
}
