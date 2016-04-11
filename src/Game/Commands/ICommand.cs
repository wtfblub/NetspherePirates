using System.Collections.Generic;
using Netsphere.Network;

namespace Netsphere.Commands
{
    internal interface ICommand
    {
        string Name { get; }
        bool AllowConsole { get; }
        SecurityLevel Permission { get; }
        IReadOnlyList<ICommand> SubCommands { get; }

        bool Execute(GameServer server, Player plr, string[] args);
        string Help();
    }
}
