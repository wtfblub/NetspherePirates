using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlubLib;
using Netsphere.API;
using Netsphere.Network;
using Newtonsoft.Json;
using NLog;
using NLog.Fluent;
using Shaolinq;
using Shaolinq.MySql;
using Shaolinq.Sqlite;

namespace Netsphere
{
    internal class Program
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static APIServer s_apiHost;

        private static void Main()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new IPEndPointConverter() }
            };

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

            AuthServer.Instance.Start(Config.Instance.Listener);
            s_apiHost = new APIServer();
            s_apiHost.Start(Config.Instance.API.Listener);

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
                if (input == null)
                    break;

                if (input.Equals("exit", StringComparison.InvariantCultureIgnoreCase) ||
                    input.Equals("quit", StringComparison.InvariantCultureIgnoreCase) ||
                    input.Equals("stop", StringComparison.InvariantCultureIgnoreCase))
                    break;
            }

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

            s_apiHost.Dispose();
            AuthServer.Instance.Dispose();
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
        // ReSharper disable once InconsistentNaming
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
                    if (Utilities.IsMono)
                        throw new NotSupportedException("SQLite is not supported on mono. Please switch to MySQL");
                    dbConfig = SqliteConfiguration.Create(config.Filename, null, Utilities.IsMono);
                    break;

                default:
                    Logger.Error()
                        .Message("Invalid database engine {0}", Config.Instance.AuthDatabase.Engine)
                        .Write();
                    Environment.Exit(0);
                    return;

            }

            Instance = DataAccessModel.BuildDataAccessModel<Database.Auth.AuthDatabase>(dbConfig);
        }
    }
}
