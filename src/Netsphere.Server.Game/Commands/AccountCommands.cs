using System;
using System.Threading.Tasks;
using LinqToDB;
using Netsphere.Common.Cryptography;
using Netsphere.Database;
using Netsphere.Database.Auth;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game.Commands
{
    internal class AccountCommands : ICommandHandler
    {
        private readonly IDatabaseProvider _databaseProvider;

        public AccountCommands(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
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

            using (var db = _databaseProvider.Open<AuthContext>())
            {
                var id = await db.InsertWithInt32IdentityAsync(new AccountEntity
                {
                    Username = username,
                    Password = hash,
                    Salt = salt,
                    SecurityLevel = (byte)SecurityLevel.User
                });

                var msg = $"Created account with username={username} id={id}";
                if (plr != null)
                    await plr.SendConsoleMessage(S4Color.Green + msg);
                else
                    Console.WriteLine(msg);
            }

            return true;
        }
    }
}
