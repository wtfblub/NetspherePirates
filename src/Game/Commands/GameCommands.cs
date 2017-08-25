using System;
using System.Collections.Generic;
using System.Text;
using Netsphere.Network;
using Netsphere.Network.Message.Game;

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
            SubCommands = new ICommand[] { new StateCommand() };
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
                    GameRuleStateTrigger trigger;
                    if (!Enum.TryParse(args[0], out trigger))
                    {
                        plr.SendConsoleMessage($"{S4Color.Red}Invalid trigger! Available triggers: {string.Join(",", stateMachine.PermittedTriggers)}");
                    }
                    else
                    {
                        stateMachine.Fire(trigger);
                        plr.Room.Broadcast(new NoticeAdminMessageAckMessage($"Current game state has been changed by {plr.Account.Nickname}"));
                    }
                }

                return true;
            }

            public string Help()
            {
                return Name + " [trigger]";
            }
        }
    }
}
