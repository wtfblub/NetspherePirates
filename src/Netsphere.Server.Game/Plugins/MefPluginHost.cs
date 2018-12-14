using System.Composition.Convention;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Netsphere.Server.Game.Plugins
{
    public class MefPluginHost : IPluginHost
    {
        private CompositionHost _container;
        private IPlugin[] _plugins;

        public void Initialize(IConfiguration configuration, string directory)
        {
            var logger = Log.ForContext<MefPluginHost>();
            logger.Information("Loading plugins...");

            var conventions = new ConventionBuilder();
            conventions
                .ForTypesDerivedFrom<IPlugin>()
                .Export<IPlugin>()
                .Shared();

            var assemblies = Enumerable.Empty<Assembly>();
            if (Directory.Exists(directory))
            {
                assemblies = Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories)
                    .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);
            }

            _container = new ContainerConfiguration().WithAssemblies(assemblies, conventions).CreateContainer();
            _plugins = _container.GetExports<IPlugin>().ToArray();

            foreach (var plugin in _plugins)
                plugin.OnInitialize();

            logger.Information("Loaded {Count} plugins", _plugins.Length);
        }

        public void OnConfigure(IServiceCollection services)
        {
            foreach (var plugin in _plugins)
                plugin.OnConfigure(services);
        }

        public void Dispose()
        {
            foreach (var plugin in _plugins)
                plugin.OnShutdown();

            _container.Dispose();
        }
    }

    public interface IPlugin
    {
        void OnInitialize();

        void OnConfigure(IServiceCollection services);

        void OnShutdown();
    }
}
