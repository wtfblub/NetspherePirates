using System;
using System.Linq;
using System.Threading.Tasks;
using Auth.Network;
using NLog;
using NLog.Fluent;
using Shaolinq;
using Shaolinq.MySql;
using Shaolinq.Sqlite;

namespace Auth
{
    internal class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static void Main()
        {
            Shaolinq.Logging.LogProvider.IsDisabled = true;

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            _logger.Info()
                .Message("Checking database...")
                .Write();

            AuthDatabase.Instance.CreateIfNotExist();

            _logger.Info()
                .Message("Starting server...")
                .Write();

            var server = new AuthServer();
            server.Start(Config.Instance.Listener);

            _logger.Info()
                .Message("Ready for connections!")
                .Write();

            while (true)
            {
                var input = Console.ReadLine();
                if (input == "exit")
                    break;
            }

            _logger.Info()
                .Message("Closing...")
                .Write();
            server.Dispose();
            LogManager.Shutdown();
        }

        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            _logger.Error()
                .Exception(e.Exception)
                .Message("UnobservedTaskException")
                .Write();
        }

        private static void OnUnhandledException(object s, UnhandledExceptionEventArgs e)
        {
            _logger.Error()
                .Exception((Exception)e.ExceptionObject)
                .Message("UnhandledException")
                .Write();
        }
    }

    internal static class AuthDatabase
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public static Netsphere.Database.Auth.AuthDatabase Instance { get; }

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
                    _logger.Error()
                        .Message("Invalid database engine {0}", Config.Instance.AuthDatabase.Engine)
                        .Write();
                    Environment.Exit(0);
                    return;

            }

            Instance = DataAccessModel.BuildDataAccessModel<Netsphere.Database.Auth.AuthDatabase>(dbConfig);
        }
    }
}
