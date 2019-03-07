using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Netsphere.Common;
using Netsphere.Database;
using Netsphere.Database.Auth;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game.Commands
{
    public class GameMasterCommands : ICommandHandler
    {
        private readonly PlayerManager _playerManager;
        private readonly DatabaseService _databaseService;

        public GameMasterCommands(PlayerManager playerManager, DatabaseService databaseService)
        {
            _playerManager = playerManager;
            _databaseService = databaseService;
        }

        [Command(
            CommandUsage.Player,
            SecurityLevel.GameMaster,
            "Usage: gm"
        )]
        public async Task<bool> GM(Player plr, string[] args)
        {
            plr.IsInGMMode = !plr.IsInGMMode;
            this.Reply(plr, plr.IsInGMMode ? "You are now in GM mode" : "You are no longer in GM mode");
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
                p.SendNotice(string.Join(" ", args));

            return true;
        }

        [Command(
            CommandUsage.Player | CommandUsage.Console,
            SecurityLevel.GameMaster,
            "Usage: kick <accountId or nickname>"
        )]
        public async Task<bool> Kick(Player plr, string[] args)
        {
            if (args.Length != 1)
                return false;

            var playerToKick = ulong.TryParse(args[0], out var id)
                ? _playerManager[id]
                : _playerManager.GetByNickname(args[0]);

            if (playerToKick == null)
            {
                this.ReplyError(plr, "Player not found");
                return true;
            }

            playerToKick.Disconnect();
            this.Reply(plr, $"Kicked player {playerToKick.Account.Nickname}(Id:{playerToKick.Account.Id})");
            return true;
        }

        [Command(
            CommandUsage.Player | CommandUsage.Console,
            SecurityLevel.GameMaster,
            "Usage: ban <accountId or nickname> <duration> <reason>"
        )]
        public async Task<bool> Ban(Player plr, string[] args)
        {
            if (args.Length < 3)
                return false;

            var bannedBy = plr == null ? "SYSTEM" : plr.Account.Nickname;
            var reason = $"{bannedBy}: {string.Join(" ", args.Skip(2))}";

            if (!TimeSpan.TryParse(args[1], out var duration))
            {
                this.ReplyError(plr, "Invalid duration. Format is days:hours:minutes:seconds");
                return true;
            }

            AccountEntity account = null;
            if (ulong.TryParse(args[0], out var id))
            {
                var accountId = (long)id;
                using (var db = _databaseService.Open<AuthContext>())
                    account = await db.Accounts.Include(x => x.Bans).FirstOrDefaultAsync(x => x.Id == accountId);
            }

            if (account == null)
            {
                var nickname = args[0];
                using (var db = _databaseService.Open<AuthContext>())
                    account = await db.Accounts.Include(x => x.Bans).FirstOrDefaultAsync(x => x.Nickname == nickname);
            }

            if (account == null)
            {
                this.ReplyError(plr, "Player not found");
                return true;
            }

            // Check ban status
            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            var ban = account.Bans.FirstOrDefault(x => x.Duration == null || x.Date + x.Duration > now);
            if (ban != null)
            {
                this.ReplyError(plr, "Player is already banned");
                return true;
            }

            using (var db = _databaseService.Open<AuthContext>())
            {
                db.Bans.Add(new BanEntity
                {
                    AccountId = account.Id,
                    Date = now,
                    Duration = (long?)duration.TotalSeconds,
                    Reason = reason
                });

                await db.SaveChangesAsync();
            }

            // Kick player if online
            _playerManager[(ulong)account.Id]?.Disconnect();

            this.Reply(plr, $"Banned player {account.Nickname}(Id:{account.Id}) for {duration.ToHumanReadable()}");
            return true;
        }

        [Command(
            CommandUsage.Player | CommandUsage.Console,
            SecurityLevel.GameMaster,
            "Usage: unban <accountId or nickname>"
        )]
        public async Task<bool> Unban(Player plr, string[] args)
        {
            if (args.Length < 1)
                return false;

            AccountEntity account = null;
            if (ulong.TryParse(args[0], out var id))
            {
                var accountId = (long)id;
                using (var db = _databaseService.Open<AuthContext>())
                    account = await db.Accounts.Include(x => x.Bans).FirstOrDefaultAsync(x => x.Id == accountId);
            }

            if (account == null)
            {
                var nickname = args[0];
                using (var db = _databaseService.Open<AuthContext>())
                    account = await db.Accounts.Include(x => x.Bans).FirstOrDefaultAsync(x => x.Nickname == nickname);
            }

            if (account == null)
            {
                this.ReplyError(plr, "Player not found");
                return true;
            }

            // Check ban status
            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            var ban = account.Bans.FirstOrDefault(x => x.Duration == null || x.Date + x.Duration > now);
            if (ban != null)
            {
                using (var db = _databaseService.Open<AuthContext>())
                {
                    db.Bans.Remove(ban);
                    await db.SaveChangesAsync();
                }

                this.Reply(plr, $"Unbanned player {account.Nickname}({account.Id})");
                return true;
            }

            this.ReplyError(plr, "Player is not banned");
            return true;
        }
    }
}
