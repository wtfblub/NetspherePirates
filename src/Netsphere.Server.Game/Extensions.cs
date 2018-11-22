using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommands(this IServiceCollection This, Assembly assembly)
        {
            foreach (var type in assembly.DefinedTypes)
            {
                if (!type.IsInterface && typeof(ICommandHandler).IsAssignableFrom(type))
                    This.AddSingleton(typeof(ICommandHandler), type);
            }

            return This;
        }
    }
}
