using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlubLib;
using Nancy.Hosting.Self;
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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static NancyHost _nancyHost;

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
            _nancyHost = new NancyHost(new Uri(Config.Instance.WebAPI.Listener));
            _nancyHost.Start();

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

            _nancyHost.Dispose();
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
