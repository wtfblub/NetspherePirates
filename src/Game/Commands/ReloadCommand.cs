using System;
using System.Collections.Generic;
using System.Text;
using Netsphere.Network;
using Netsphere.Resource;

namespace Netsphere.Commands
{
    internal class ReloadCommand : ICommand
    {
        public string Name { get; }
        public bool AllowConsole { get; }
        public SecurityLevel Permission { get; }
        public IReadOnlyList<ICommand> SubCommands { get; }

        public ReloadCommand()
        {
            Name = "reload";
            AllowConsole = true;
            Permission = SecurityLevel.Developer;
            SubCommands = new ICommand[] {new ShopCommand()};
        }

        public bool Execute(GameServer server, Player plr, string[] args)
        {
            return true;
        }

        public string Help()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Name);
            foreach (var cmd in SubCommands)
            {
                sb.Append("\t");
                sb.AppendLine(cmd.Help());
            }
            return sb.ToString();
        }

        private class ShopCommand : ICommand
        {
            public string Name { get; }
            public bool AllowConsole { get; }
            public SecurityLevel Permission { get; }
            public IReadOnlyList<ICommand> SubCommands { get; }

            public ShopCommand()
            {
                Name = "shop";
                AllowConsole = true;
                Permission = SecurityLevel.Developer;
                SubCommands = new ICommand[] {};
            }

            public bool Execute(GameServer server, Player plr, string[] args)
            {
                var message = "Reloading shop, server may lag for a short period of time...";

                if (plr == null)
                    Console.WriteLine(message);
                else
                    plr.SendConsoleMessage(S4Color.Green + message);

                server.BroadcastNotice(message);

                server.ResourceCache.Clear(ResourceCacheType.Shop);
                server.ResourceCache.GetShop();

                message = "Reload completed";
                server.BroadcastNotice(message);
                if (plr == null)
                    Console.WriteLine(message);
                else
                    plr.SendConsoleMessage(S4Color.Green + message);

                return true;
            }

            public string Help()
            {
                return Name;
            }
        }
    }
}
