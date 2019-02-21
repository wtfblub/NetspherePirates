using System.Linq;
using System.Threading.Tasks;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game.Commands
{
    internal class GameRuleCommands : ICommandHandler
    {
        [Command(
            CommandUsage.Player,
            SecurityLevel.Developer,
            "Usage: teamscore <team> <score>"
        )]
        public async Task<bool> TeamScore(Player plr, string[] args)
        {
            if (args.Length != 2)
                return false;

            if (plr.Room == null)
            {
                plr.SendConsoleMessage(S4Color.Red + "You need to be a in a room");
                return false;
            }

            if (!byte.TryParse(args[0], out var teamId))
                return false;

            if (!uint.TryParse(args[1], out var score))
                return false;

            var team = plr.Room.TeamManager[(TeamId)teamId];
            if (team == null)
            {
                plr.SendConsoleMessage(
                    S4Color.Red +
                    "Invalid team. Valid teams are: " +
                    string.Join(", ", plr.Room.TeamManager.Select(x => (int)x.Key))
                );
                return false;
            }

            team.Score = score;
            plr.Room.BroadcastBriefing();
            return true;
        }

        [Command(
            CommandUsage.Player,
            SecurityLevel.Developer,
            "Usage: gamestate <gamestate>"
        )]
        public async Task<bool> GameState(Player plr, string[] args)
        {
            if (args.Length != 1)
                return false;

            if (plr.Room == null)
            {
                plr.SendConsoleMessage(S4Color.Red + "You need to be a in a room");
                return false;
            }

            var room = plr.Room;
            var gameRule = room.GameRule;
            var stateMachine = gameRule.StateMachine;

            switch (args[0].ToLower())
            {
                case "start":
                    if (stateMachine.GameState != Netsphere.GameState.Waiting)
                        plr.SendConsoleMessage(S4Color.Red + "This state is currently not possible");

                    if (!stateMachine.StartGame())
                    {
                        GameRuleBase.CanStartGameHook += CanStartGameHook;
                        stateMachine.StartGame();
                        GameRuleBase.CanStartGameHook -= CanStartGameHook;

                        bool CanStartGameHook(CanStartGameHookEventArgs e)
                        {
                            if (e.GameRule == room.GameRule)
                                e.Result = true;

                            return true;
                        }
                    }

                    break;

                case "halftime":
                    if (!plr.Room.GameRule.StateMachine.StartHalfTime())
                        plr.SendConsoleMessage(S4Color.Red + "This state is currently not possible");

                    break;

                case "result":
                    if (!plr.Room.GameRule.StateMachine.StartResult())
                        plr.SendConsoleMessage(S4Color.Red + "This state is currently not possible");

                    break;

                default:
                    plr.SendConsoleMessage(
                        S4Color.Red +
                        "Invalid state. Valid values are: start, halftime, result"
                    );
                    return false;
            }

            return true;
        }

        [Command(
            CommandUsage.Player,
            SecurityLevel.Developer,
            "Usage: briefing"
        )]
        public async Task<bool> Briefing(Player plr, string[] args)
        {
            if (plr.Room == null)
            {
                plr.SendConsoleMessage(S4Color.Red + "You need to be a in a room");
                return false;
            }

            plr.Room.BroadcastBriefing();
            return true;
        }
    }
}
