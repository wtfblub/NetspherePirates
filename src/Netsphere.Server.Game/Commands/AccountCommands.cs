using System;
using System.Threading.Tasks;
using Netsphere.Common.Cryptography;
using Netsphere.Database;
using Netsphere.Database.Auth;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game.Commands
{
    internal class AccountCommands : ICommandHandler
    {
        private readonly DatabaseService _databaseService;

        public AccountCommands(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [Command(
            CommandUsage.Player | CommandUsage.Console,
            SecurityLevel.Administrator,
            "Usage: createaccount <username> <password>"
        )]
        public async Task<bool> CreateAccount(Player plr, string[] args)
        {
            if (args.Length != 2)
                return false;

            var username = args[0];
            var password = args[1];
            var (hash, salt) = PasswordHasher.Hash(password);

            using (var db = _databaseService.Open<AuthContext>())
            {
                var accountEntity = new AccountEntity
                {
                    Username = username,
                    Password = hash,
                    Salt = salt,
                    SecurityLevel = (byte)SecurityLevel.User
                };
                db.Accounts.Add(accountEntity);
                await db.SaveChangesAsync();

                var msg = $"Created account with username={username} id={accountEntity.Id}";
                if (plr != null)
                    await plr.SendConsoleMessage(S4Color.Green + msg);
                else
                    Console.WriteLine(msg);
            }

            return true;
        }
    }
}
