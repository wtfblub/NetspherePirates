using System;
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

    public static class CommandHandlerExtensions
    {
        public static void Reply(this ICommandHandler This, Player sender, string message)
        {
            if (sender == null)
                Console.WriteLine(message);
            else
                sender.SendConsoleMessage(message);
        }

        public static void ReplyError(this ICommandHandler This, Player sender, string message)
        {
            if (sender == null)
                Console.WriteLine(message);
            else
                sender.SendConsoleMessage(S4Color.Red + message);
        }
    }
}
