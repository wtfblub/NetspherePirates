using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlubLib;
using Netsphere.API;
using Netsphere.Network;
using Newtonsoft.Json;
using NLog;
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

            Logger.Info("Checking database...");

            AuthDatabase.Instance.CreateIfNotExist();

            Logger.Info("Starting server...");

            AuthServer.Instance.Start(Config.Instance.Listener);
            s_apiHost = new APIServer();
            s_apiHost.Start(Config.Instance.API.Listener);

            Logger.Info("Ready for connections!");

            if (Config.Instance.NoobMode)
            {
                Logger.Warn("!!! NOOB MODE IS ENABLED! EVERY LOGIN SUCCEEDS AND OVERRIDES ACCOUNT LOGIN DETAILS !!!");
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
            Logger.Info("Closing...");

            s_apiHost.Dispose();
            AuthServer.Instance.Dispose();
            LogManager.Shutdown();
        }

        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger.Error(e.Exception, "UnobservedTaskException");
        }

        private static void OnUnhandledException(object s, UnhandledExceptionEventArgs e)
        {
            Logger.Error((Exception)e.ExceptionObject, "UnhandledException");
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
                    Logger.Error("Invalid database engine {0}", Config.Instance.AuthDatabase.Engine);
                    Environment.Exit(0);
                    return;

            }

            Instance = DataAccessModel.BuildDataAccessModel<Database.Auth.AuthDatabase>(dbConfig);
        }
    }
}
