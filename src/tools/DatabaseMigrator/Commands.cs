using System;
using EntryPoint;
using EntryPoint.Exceptions;
using MySql.Data.MySqlClient;
using SimpleMigrations;
using SimpleMigrations.Console;
using SimpleMigrations.DatabaseProvider;

namespace DatabaseMigrator
{
    internal class Commands : BaseCliCommands
    {
        private static readonly Type s_authType = typeof(Netsphere.Database.Migration.Auth.Base);
        private static readonly Type s_gameType = typeof(Netsphere.Database.Migration.Game.Base);

        [Command("migrate")]
        public void Migrate(string[] args)
        {
            var options = Cli.Parse<MigrateOptions>(args);
            var connectionString = $"SslMode=none;Server={options.Host};Port={options.Port};Database={options.Database};Uid={options.User};Pwd={options.Password};";
            using (var con = new MySqlConnection(connectionString))
            {
                con.Open();
                var databaseProvider = new MysqlDatabaseProvider(con) { TableName = "__version" };
                AssemblyMigrationProvider assemblyProvider = null;
                switch (options.Schema.ToLower())
                {
                    case "auth":
                        assemblyProvider = new AssemblyMigrationProvider(s_authType.Assembly, s_authType.Namespace);
                        break;

                    case "game":
                        assemblyProvider = new AssemblyMigrationProvider(s_gameType.Assembly, s_gameType.Namespace);
                        break;

                    default:
                        Error("Invalid schema");
                        break;
                }

                try
                {
                    var migrator = new SimpleMigrator(assemblyProvider, databaseProvider, new ConsoleLogger());
                    migrator.Load();
                    if (options.CurrentVersion > 0)
                        migrator.Baseline(options.CurrentVersion);

                    if (options.Version == 0)
                        migrator.MigrateToLatest();
                    else
                        migrator.MigrateTo(options.Version);
                }
                catch (MigrationNotFoundException ex)
                {
                    Error(ex.Message);
                }
            }
        }

        public override void OnUserFacingException(UserFacingException e, string message)
        {
            Error(message);
        }

        private static void Error(string message)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Console.ForegroundColor = color;
            Environment.Exit(1);
        }

        internal class MigrateOptions : BaseCliArguments
        {
            [OptionParameter("version", 'v')]
            [Help("The version to migrate to. Defaults to latest when not specified.")]
            public uint Version { get; set; }

            [OptionParameter("current-version", 'c')]
            [Help("The current version to pretend the database uses. Use at own risk!")]
            public uint CurrentVersion { get; set; }

            [OptionParameter("host")]
            [Help("Database host")]
            [Required]
            public string Host { get; set; }

            [OptionParameter("port")]
            [Help("Database port. Default is 3306")]
            public ushort Port { get; set; }

            [OptionParameter("user", 'u')]
            [Help("Database user")]
            [Required]
            public string User { get; set; }

            [OptionParameter("password", 'p')]
            [Help("Database user password")]
            [Required]
            public string Password { get; set; }

            [OptionParameter("database")]
            [Required]
            public string Database { get; set; }

            [Operand(1)]
            [Help("The schema to use. Values: auth, game")]
            [Required]
            public string Schema { get; set; }

            public MigrateOptions()
                : base("DatabaseMigrator Command Line")
            {
                Port = 3306;
            }

            public override void OnUserFacingException(UserFacingException e, string message)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(message);
                Console.ForegroundColor = color;
                Environment.Exit(1);
            }
        }
    }
}
