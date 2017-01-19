using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Netsphere.Network;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using Newtonsoft.Json;

namespace Netsphere.Commands
{
    internal class InventoryCommands : ICommand
    {
        public string Name { get; }
        public bool AllowConsole { get; }
        public SecurityLevel Permission { get; }
        public IReadOnlyList<ICommand> SubCommands { get; }

        public InventoryCommands()
        {
            Name = "inventory";
            AllowConsole = false;
            Permission = SecurityLevel.Developer;
            SubCommands = new ICommand[] { new ListCommand(), new ShowItemCommand(), new SetCommand() };
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

        private class ListCommand : ICommand
        {
            public string Name { get; }
            public bool AllowConsole { get; }
            public SecurityLevel Permission { get; }
            public IReadOnlyList<ICommand> SubCommands { get; }

            public ListCommand()
            {
                Name = "list";
                AllowConsole = false;
                Permission = SecurityLevel.Developer;
                SubCommands = new ICommand[0];
            }

            public bool Execute(GameServer server, Player plr, string[] args)
            {
                var res = GameServer.Instance.ResourceCache.GetItems();
                var sb = new StringBuilder();
                foreach (var item in plr.Inventory)
                {
                    var itemInfo = res.GetValueOrDefault(item.ItemNumber);
                    sb.AppendLine($"#{item.Id}: {item.ItemNumber} {itemInfo?.Name}");
                }
                plr.SendConsoleMessage(sb.ToString());
                return true;
            }

            public string Help()
            {
                return Name;
            }
        }

        private class ShowItemCommand : ICommand
        {
            public string Name { get; }
            public bool AllowConsole { get; }
            public SecurityLevel Permission { get; }
            public IReadOnlyList<ICommand> SubCommands { get; }

            public ShowItemCommand()
            {
                Name = "showitem";
                AllowConsole = false;
                Permission = SecurityLevel.Developer;
                SubCommands = new ICommand[0];
            }

            public bool Execute(GameServer server, Player plr, string[] args)
            {
                if (args.Length < 1)
                    return false;

                ulong itemId;
                if (!ulong.TryParse(args[0], out itemId))
                    return false;

                var item = plr.Inventory[itemId];
                if (item == null)
                {
                    plr.SendConsoleMessage(S4Color.Red + "Item not found");
                    return true;
                }

                plr.SendConsoleMessage(JsonConvert.SerializeObject(item, Formatting.Indented));
                return true;
            }

            public string Help()
            {
                return Name + " <id>";
            }
        }

        private class SetCommand : ICommand
        {
            public string Name { get; }
            public bool AllowConsole { get; }
            public SecurityLevel Permission { get; }
            public IReadOnlyList<ICommand> SubCommands { get; }

            public SetCommand()
            {
                Name = "set";
                AllowConsole = false;
                Permission = SecurityLevel.Developer;
                SubCommands = new ICommand[0];
            }

            public bool Execute(GameServer server, Player plr, string[] args)
            {
                if (args.Length < 2)
                    return false;

                ulong itemId;
                if (!ulong.TryParse(args[0], out itemId))
                    return false;

                var item = plr.Inventory[itemId];
                if (item == null)
                {
                    plr.SendConsoleMessage(S4Color.Red + "Item not found");
                    return true;
                }

                switch (args[1].ToLower())
                {
                    case "durability":
                        int newDurability;
                        if (!int.TryParse(args[2], out newDurability))
                            return false;

                        plr.Session.SendAsync(new SItemDurabilityInfoAckMessage(new[]{new ItemDurabilityInfoDto
                        {
                            ItemId = item.Id,
                            Durability = newDurability,
                            Unk2 = 0,
                            Unk3 = 0
                        } })).WaitEx();

                        break;

                    default:
                        plr.SendConsoleMessage(S4Color.Red + "Invalid field");
                        break;
                }
                return true;
            }

            public string Help()
            {
                return Name + " <id> <field> <value>";
            }
        }
    }
}
