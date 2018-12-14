using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Netsphere.Common;
using Netsphere.Common.Configuration.Hjson;
using Netsphere.Server.Game.Plugins;

namespace WebApi
{
    public class WebApiPlugin : IPlugin
    {
        private IConfiguration _configuration;

        public void OnInitialize()
        {
            var path = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            path = Path.GetDirectoryName(path);
            path = Path.Combine(path, "webapi.hjson");

            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddHjsonFile(path, false, true)
                .Build();
        }

        public void OnConfigure(IServiceCollection services)
        {
            services
                .Configure<WebApiOptions>(_configuration)
                .AddHostedServiceEx<WebApiService>();
        }

        public void OnShutdown()
        {
        }
    }
}
