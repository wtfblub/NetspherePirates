using System.Threading.Tasks;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game.Commands
{
    public class GameMasterCommands : ICommandHandler
    {
        private readonly PlayerManager _playerManager;

        public GameMasterCommands(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        [Command(
            CommandUsage.Player,
            SecurityLevel.GameMaster,
            "Usage: gm"
        )]
        public async Task<bool> GM(Player plr, string[] args)
        {
            plr.IsInGMMode = !plr.IsInGMMode;
            plr.SendConsoleMessage(plr.IsInGMMode ? "You are now in GM mode" : "You are no longer in GM mode");
            return true;
        }

        [Command(
            CommandUsage.Player,
            SecurityLevel.GameMaster,
            "Usage: announce"
        )]
        public async Task<bool> Announce(Player plr, string[] args)
        {
            if (args.Length == 0)
                return false;

            foreach (var p in _playerManager)
                p.SendNotice(string.Join("", args));

            return true;
        }
    }
}
