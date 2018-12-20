using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BlubLib;
using Logging;
using Microsoft.Extensions.Hosting;

namespace Netsphere.Server.Game.Services
{
    public class CommandService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly ICommandHandler[] _commandHandlers;
        private readonly CancellationTokenSource _shutdown;
        private Command[] _commands;

        public CommandService(ILogger<CommandService> logger, IEnumerable<ICommandHandler> commandHandlers)
        {
            _logger = logger;
            _commandHandlers = commandHandlers.ToArray();
            _shutdown = new CancellationTokenSource();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            LoadCommands();
            var _ = Task.Run(ConsoleReader);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _shutdown.Cancel();
            return Task.CompletedTask;
        }

        public async Task<bool> Execute(Player plr, string[] args)
        {
            var command = FindCommand(args[0]);
            if (command == null)
                return false;

            // Check if console usage is allowed
            if (plr == null && !command.CommandUsage.HasFlag(CommandUsage.Console))
                return false;

            // Check if player usage is allowed and if the player has the permission
            if (plr != null)
            {
                if (command.CommandUsage.HasFlag(CommandUsage.Player))
                {
                    if (plr.Account.SecurityLevel < command.SecurityLevel)
                        return false;
                }
                else
                {
                    return false;
                }
            }

            _logger.Information("Executing command={CommandName} player={PlayerId}",
                command.Name, plr?.Account.Id.ToString() ?? "console");

            var result = await command.Execute(command.CommandHandler, plr, args.Skip(1).ToArray());
            if (!result)
            {
                if (plr != null)
                    await plr.SendConsoleMessage(command.Help);
                else
                    Console.WriteLine(command.Help);
            }

            return result;
        }

        private async Task ConsoleReader()
        {
            while (!_shutdown.IsCancellationRequested)
            {
                try
                {
                    var input = Console.ReadLine();
                    await Execute(null, input.GetArgs());
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Unable to execute command");
                }
            }
        }

        private Command FindCommand(string cmd)
        {
            return _commands.FirstOrDefault(x =>
                x.Triggers.Any(trigger => trigger.Equals(cmd, StringComparison.OrdinalIgnoreCase)));
        }

        private void LoadCommands()
        {
            _logger.Information("Loading commands...");

            var commands = new List<Command>();

            foreach (var commandHandler in _commandHandlers)
            {
                var type = commandHandler.GetType().GetTypeInfo();
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var method in methods)
                {
                    var attribute = method.GetCustomAttribute<CommandAttribute>();
                    if (attribute == null)
                        continue;

                    var command = new Command
                    {
                        Name = $"{type.FullName}.{method.Name}",
                        Triggers = attribute.Alias.Concat(new[] { method.Name }).ToArray(),
                        CommandHandler = commandHandler,
                        CommandUsage = attribute.CommandUsage,
                        SecurityLevel = attribute.SecurityLevel,
                        Help = attribute.Help
                    };

                    var thisParam = Expression.Parameter(typeof(ICommandHandler));
                    var plrParam = Expression.Parameter(typeof(Player));
                    var argsParam = Expression.Parameter(typeof(string[]));
                    var body = Expression.Call(Expression.Convert(thisParam, type), method, plrParam, argsParam);
                    command.Execute = Expression.Lambda<Func<ICommandHandler, Player, string[], Task<bool>>>(
                        body, thisParam, plrParam, argsParam).Compile();
                    commands.Add(command);
                }
            }

            _commands = commands.ToArray();
            _logger.Information("Loaded {Count} commands", commands.Count);
        }

        private class Command
        {
            public string Name { get; set; }
            public string[] Triggers { get; set; }
            public ICommandHandler CommandHandler { get; set; }
            public CommandUsage CommandUsage { get; set; }
            public SecurityLevel SecurityLevel { get; set; }
            public string Help { get; set; }
            public Func<ICommandHandler, Player, string[], Task<bool>> Execute { get; set; }
        }
    }

    public interface ICommandHandler
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : System.Attribute
    {
        public string[] Alias { get; set; }
        public CommandUsage CommandUsage { get; set; }
        public SecurityLevel SecurityLevel { get; set; }
        public string Help { get; set; }

        public CommandAttribute()
        {
            Alias = Array.Empty<string>();
            CommandUsage = CommandUsage.Player | CommandUsage.Console;
            SecurityLevel = SecurityLevel.GameMaster;
            Help = "";
        }

        public CommandAttribute(CommandUsage commandUsage, SecurityLevel securityLevel, string help, params string[] alias)
        {
            CommandUsage = commandUsage;
            SecurityLevel = securityLevel;
            Help = help;
            Alias = alias;
        }
    }

    [Flags]
    public enum CommandUsage
    {
        Player,
        Console
    }
}
