using System;
using System.Threading.Tasks;
using Netsphere.Network;
using NLog;
using NLog.Fluent;
using Shaolinq;
using Shaolinq.MySql;
using Shaolinq.Sqlite;

namespace Netsphere
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static void Main()
        {
            Shaolinq.Logging.LogProvider.IsDisabled = true;

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            Logger.Info()
                .Message("Checking database...")
                .Write();

            AuthDatabase.Instance.CreateIfNotExist();

            Logger.Info()
                .Message("Starting server...")
                .Write();

            var server = new AuthServer();
            server.Start(Config.Instance.Listener);

            Logger.Info()
                .Message("Ready for connections!")
                .Write();

            if (Config.Instance.NoobMode)
            {
                Logger.Warn()
                    .Message("!!! NOOB MODE IS ENABLED! EVERY LOGIN SUCCEEDS AND OVERRIDES ACCOUNT LOGIN DETAILS !!!")
                    .Write();
            }

            Console.CancelKeyPress += OnCancelKeyPress;
            while (true)
            {
                var input = Console.ReadLine();
                if (input.Equals("exit", StringComparison.InvariantCultureIgnoreCase) ||
                    input.Equals("quit", StringComparison.InvariantCultureIgnoreCase) ||
                    input.Equals("stop", StringComparison.InvariantCultureIgnoreCase))
                    break;
            }

            server.Dispose(); // ToDo Make AuthServer a singleton
            Exit();
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Exit();
        }

        private static void Exit()
        {
            Logger.Info()
                .Message("Closing...")
                .Write();

            LogManager.Shutdown();
        }

        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger.Error()
                .Exception(e.Exception)
                .Message("UnobservedTaskException")
                .Write();
        }

        private static void OnUnhandledException(object s, UnhandledExceptionEventArgs e)
        {
            Logger.Error()
                .Exception((Exception)e.ExceptionObject)
                .Message("UnhandledException")
                .Write();
        }
    }

    internal static class AuthDatabase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static Database.Auth.AuthDatabase Instance { get; }

        static AuthDatabase()
        {
            var config = Config.Instance.AuthDatabase;

            DataAccessModelConfiguration dbConfig;
            switch (Config.Instance.AuthDatabase.Engine)
            {
                case DatabaseEngine.MySQL:
                    dbConfig = MySqlConfiguration.Create(config.Database, config.Host, config.Username, config.Password, true);
                    break;

                case DatabaseEngine.SQLite:
                    dbConfig = SqliteConfiguration.Create(config.Filename, null);
                    break;

                default:
                    Logger.Error()
                        .Message("Invalid database engine {0}", Config.Instance.AuthDatabase.Engine)
                        .Write();
                    Environment.Exit(0);
                    return;

            }

            Instance = DataAccessModel.BuildDataAccessModel<Netsphere.Database.Auth.AuthDatabase>(dbConfig);
        }
    }
}
