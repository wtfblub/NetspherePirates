using System;
using System.Collections.Generic;
using System.Text;
using Netsphere.Network;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;

namespace Netsphere.Commands
{
    internal class GameCommands : ICommand
    {
        public string Name { get; }
        public bool AllowConsole { get; }
        public SecurityLevel Permission { get; }
        public IReadOnlyList<ICommand> SubCommands { get; }

        public GameCommands()
        {
            Name = "game";
            AllowConsole = false;
            Permission = SecurityLevel.Developer;
            SubCommands = new ICommand[] { new StateCommand(), new EventCommand() };
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

        private class StateCommand : ICommand
        {
            public string Name { get; }
            public bool AllowConsole { get; }
            public SecurityLevel Permission { get; }
            public IReadOnlyList<ICommand> SubCommands { get; }

            public StateCommand()
            {
                Name = "state";
                AllowConsole = false;
                Permission = SecurityLevel.Developer;
                SubCommands = new ICommand[0];
            }

            public bool Execute(GameServer server, Player plr, string[] args)
            {
                if (plr.Room == null)
                {
                    plr.SendConsoleMessage(S4Color.Red + "You're not inside a room");
                    return false;
                }

                var stateMachine = plr.Room.GameRuleManager.GameRule.StateMachine;
                if (args.Length == 0)
                {
                    plr.SendConsoleMessage($"Current state: {stateMachine.State}");
                }
                else
                {
                    if (!Enum.TryParse<GameRuleStateTrigger>(args[0], out var trigger))
                    {
                        plr.SendConsoleMessage($"{S4Color.Red}Invalid trigger! Available triggers: {string.Join(",", stateMachine.PermittedTriggers)}");
                    }
                    else
                    {
                        stateMachine.Fire(trigger);
                        plr.Room.Broadcast(new SNoticeMessageAckMessage($"Current game state has been changed by {plr.Account.Nickname}"));
                    }
                }

                return true;
            }

            public string Help()
            {
                return Name + " [trigger]";
            }
        }

        private class EventCommand : ICommand
        {
            public string Name { get; }
            public bool AllowConsole { get; }
            public SecurityLevel Permission { get; }
            public IReadOnlyList<ICommand> SubCommands { get; }

            public EventCommand()
            {
                Name = "event";
                AllowConsole = false;
                Permission = SecurityLevel.Developer;
                SubCommands = new ICommand[0];
            }

            public bool Execute(GameServer server, Player plr, string[] args)
            {
                if (plr.Room == null)
                {
                    plr.SendConsoleMessage(S4Color.Red + "You're not inside a room");
                    return false;
                }

                if (args.Length < 1)
                    return false;

                if (!Enum.TryParse<GameEventMessage>(args[0], out var eventMessage))
                    plr.SendConsoleMessage($"{S4Color.Red}Invalid event! Available events: {string.Join(", ", Enum.GetNames(typeof(GameEventMessage)))}");

                ulong p1 = 0;
                uint p2 = 0;
                ushort p3 = 0;
                var p4 = "";

                if (args.Length > 1)
                {
                    if (!ulong.TryParse(args[1], out p1))
                        return false;
                }

                if (args.Length > 2)
                {
                    if (!uint.TryParse(args[2], out p2))
                        return false;
                }

                if (args.Length > 3)
                {
                    if (!ushort.TryParse(args[3], out p3))
                        return false;
                }

                if (args.Length > 4)
                    p4 = args[4];

                plr.Room.Broadcast(new SEventMessageAckMessage(eventMessage, p1, p2, p3, p4));
                return true;
            }

            public string Help()
            {
                return Name + "<event> [param1] [param2] [param3] [param4]";
            }
        }
    }
}
