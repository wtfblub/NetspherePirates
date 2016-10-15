using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BlubLib;
using Dapper.FastCrud;
using Netsphere.API;
using Netsphere.Network;
using Newtonsoft.Json;
using NLog;

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
        private static readonly string s_connectionString;

        static AuthDatabase()
        {
            var config = Config.Instance.AuthDatabase;

            switch (config.Engine)
            {
                //case DatabaseEngine.MySQL:
                //    break;

                case DatabaseEngine.SQLite:
                    if (Utilities.IsMono)
                        throw new NotSupportedException("SQLite is not supported on mono. Please switch to MySQL");
                    s_connectionString = $"Data Source={config.Filename};Pooling=true;";
                    OrmConfiguration.DefaultDialect = SqlDialect.SqLite;
                    break;

                default:
                    Logger.Error($"Invalid database engine {config.Engine}");
                    Environment.Exit(0);
                    return;

            }
        }

        public static IDbConnection Open()
        {
            var engine = Config.Instance.AuthDatabase.Engine;
            switch (engine)
            {
                case DatabaseEngine.SQLite:
                    return new System.Data.SQLite.SQLiteConnection(s_connectionString);

                default:
                    Logger.Error($"Invalid database engine {engine}");
                    Environment.Exit(0);
                    return null;
            }
        }
    }
}
