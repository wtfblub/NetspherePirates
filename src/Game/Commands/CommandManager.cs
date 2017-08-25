using System;
using System.Collections.Generic;
using System.Linq;
using Netsphere.Network;
using Serilog;
using Serilog.Core;

namespace Netsphere.Commands
{
    internal class CommandManager
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ILogger Logger = Log.ForContext(Constants.SourceContextPropertyName, nameof(CommandManager));
        private readonly IList<ICommand> _commands = new List<ICommand>();

        public GameServer Server { get; }

        public CommandManager(GameServer server)
        {
            Server = server;
        }

        public CommandManager Add(ICommand cmd)
        {
            if (_commands.Any(c => c.Name.Equals(cmd.Name, StringComparison.InvariantCultureIgnoreCase)))
                throw new Exception("Command " + cmd.Name + " already exists");

            _commands.Add(cmd);
            return this;
        }

        public bool Execute(Player plr, string[] args)
        {
            return ExecuteCommand(plr, _commands, args);
        }

        private bool ExecuteCommand(Player plr, IEnumerable<ICommand> cmds, string[] args)
        {
            if (args.Length == 0)
                return false;

            var isConsole = plr == null;
            var cmd = cmds.FirstOrDefault(c => c.Name.Equals(args[0], StringComparison.InvariantCultureIgnoreCase));
            if (cmd == null)
                return false;

            var tmp = new string[args.Length - 1];
            Array.Copy(args, 1, tmp, 0, tmp.Length);

            if (isConsole && !cmd.AllowConsole)
                return false;

            if (!isConsole && plr.Account.SecurityLevel < cmd.Permission)
            {
                Logger.ForAccount(plr)
                    .Error("Access denied for command {cmdName} - args: {args}", cmd.Name, string.Join(",", args));
                plr.SendConsoleMessage(S4Color.Red + "Unknown command");
                return true;
            }

            if (cmd.SubCommands.Count == 0)
            {
                if (!cmd.Execute(Server, plr, tmp))
                {
                    if(plr == null)
                        Console.WriteLine(cmd.Help());
                    else
                        plr.SendConsoleMessage(S4Color.Red + cmd.Help());
                }
                return true;
            }

            if (cmd.SubCommands.Count > 0 && args.Length < 2)
            {
                if (plr == null)
                    Console.WriteLine(cmd.Help());
                else
                    plr.SendConsoleMessage(S4Color.Red + cmd.Help());
                return true;
            }

            if (!ExecuteCommand(plr, cmd.SubCommands, tmp))
            {
                if (plr == null)
                    Console.WriteLine(cmd.Help());
                else
                    plr.SendConsoleMessage(S4Color.Red + cmd.Help());

                return true;
            }
            return true;
        }
    }
}
