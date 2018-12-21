using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Netsphere.Common.Plugins
{
    public interface IPluginHost : IDisposable
    {
        void Initialize(IConfiguration configuration, string directory);

        void OnConfigure(IServiceCollection services);
    }
}
