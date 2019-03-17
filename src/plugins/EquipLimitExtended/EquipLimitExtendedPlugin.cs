using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Netsphere.Common;
using Netsphere.Common.Configuration.Hjson;
using Netsphere.Common.Plugins;

namespace EquipLimitExtended
{
    public class EquipLimitExtendedPlugin : IPlugin
    {
        private IConfiguration _configuration;

        public void OnInitialize(IConfiguration appConfiguration)
        {
            var path = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            path = Path.GetDirectoryName(path);
            path = Path.Combine(path, "equiplimitextended.hjson");

            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddHjsonFile(path, false, true)
                .Build();
        }

        public void OnConfigure(IServiceCollection services)
        {
            services
                .Configure<EquipLimitExtendedOptions>(_configuration)
                .AddHostedServiceEx<EquipLimitExtendedService>();
        }

        public void OnShutdown()
        {
        }
    }
}
