using System;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.Collections.Generic;
using Netsphere.Network.Message.GameRule;
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
                this.ReplyError(plr, "You need to be a in a room");
                return false;
            }

            if (!byte.TryParse(args[0], out var teamId))
                return false;

            if (!uint.TryParse(args[1], out var score))
                return false;

            var team = plr.Room.TeamManager[(TeamId)teamId];
            if (team == null)
            {
                this.ReplyError(plr,
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
                this.ReplyError(plr, "You need to be a in a room");
                return false;
            }

            var room = plr.Room;
            var gameRule = room.GameRule;
            var stateMachine = gameRule.StateMachine;

            switch (args[0].ToLower())
            {
                case "start":
                    if (stateMachine.GameState != Netsphere.GameState.Waiting)
                        this.ReplyError(plr, "This state is currently not possible");

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
                        this.ReplyError(plr, "This state is currently not possible");

                    break;

                case "result":
                    if (!plr.Room.GameRule.StateMachine.StartResult())
                        this.ReplyError(plr, "This state is currently not possible");

                    break;

                default:
                    this.ReplyError(
                        plr,
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
                this.ReplyError(plr, "You need to be a in a room");
                return false;
            }

            plr.Room.BroadcastBriefing();
            return true;
        }

        [Command(
            CommandUsage.Player,
            SecurityLevel.Developer,
            "Usage: changehp <value> [target nickname or id]"
        )]
        public async Task<bool> ChangeHP(Player plr, string[] args)
        {
            if (args.Length < 1)
                return false;

            if (plr.Room == null)
            {
                this.ReplyError(plr, "You need to be in a room");
                return true;
            }

            if (!float.TryParse(args[0], out var value))
                return false;

            var target = plr;
            if (args.Length > 1)
            {
                if (ulong.TryParse(args[1], out var id))
                {
                    target = plr.Room.Players.GetValueOrDefault(id);
                    if (target == null)
                    {
                        this.ReplyError(plr, "Target not found in current room");
                        return true;
                    }
                }
                else
                {
                    target = plr.Room.Players.Values.FirstOrDefault(x =>
                        x.Account.Nickname.Equals(args[1], StringComparison.OrdinalIgnoreCase));
                    if (target == null)
                    {
                        this.ReplyError(plr, "Target not found in current room");
                        return true;
                    }
                }
            }

            target.Session.Send(new SChangeHPAckMessage(value));
            return true;
        }

        [Command(
            CommandUsage.Player,
            SecurityLevel.Developer,
            "Usage: changesp <value> [target nickname or id]"
        )]
        public async Task<bool> ChangeSP(Player plr, string[] args)
        {
            if (args.Length < 1)
                return false;

            if (plr.Room == null)
            {
                this.ReplyError(plr, "You need to be in a room");
                return true;
            }

            if (!float.TryParse(args[0], out var value))
                return false;

            var target = plr;
            if (args.Length > 1)
            {
                if (ulong.TryParse(args[1], out var id))
                {
                    target = plr.Room.Players.GetValueOrDefault(id);
                    if (target == null)
                    {
                        this.ReplyError(plr, "Target not found in current room");
                        return true;
                    }
                }
                else
                {
                    target = plr.Room.Players.Values.FirstOrDefault(x =>
                        x.Account.Nickname.Equals(args[1], StringComparison.OrdinalIgnoreCase));
                    if (target == null)
                    {
                        this.ReplyError(plr, "Target not found in current room");
                        return true;
                    }
                }
            }

            target.Session.Send(new SChangeMPAckMessage(value));
            return true;
        }
    }
}
