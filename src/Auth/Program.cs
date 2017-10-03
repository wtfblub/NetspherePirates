using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using BlubLib.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Netsphere.API;
using Netsphere.Configuration;
using Netsphere.Database;
using Netsphere.Network;
using Newtonsoft.Json;
using Serilog;
using Serilog.Formatting.Json;

namespace Netsphere
{
    internal class Program
    {
        private static IEventLoopGroup s_apiEventLoopGroup;
        private static IChannel s_apiHost;
        private static readonly object s_exitMutex = new object();
        private static bool s_isExiting;

        private static void Main()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new IPEndPointConverter() }
            };

            Config<Config>.Initialize("auth.hjson", "NETSPHEREPIRATES_AUTHCONF");

            var jsonlog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "auth.json");
            var logfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "auth.log");
            Log.Logger = new LoggerConfiguration()
                .Destructure.ByTransforming<IPEndPoint>(endPoint => endPoint.ToString())
                .Destructure.ByTransforming<EndPoint>(endPoint => endPoint.ToString())
                .WriteTo.File(new JsonFormatter(), jsonlog)
                .WriteTo.File(logfile)
                .WriteTo.Console(outputTemplate: "[{Level} {SourceContext}] {Message}{NewLine}{Exception}")
                .MinimumLevel.Debug()
                .CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            try
            {
                AuthDatabase.Initialize(Config.Instance.Database.Auth);
            }
            catch (DatabaseNotFoundException ex)
            {
                Log.Error("Database {Name} not found", ex.Name);
                Environment.Exit(1);
            }
            
            catch (DatabaseVersionMismatchException ex)
            {
                Log.Error("Invalid version. Database={CurrentVersion} Required={RequiredVersion}. Run the DatabaseMigrator to update your database.",
                    ex.CurrentVersion, ex.RequiredVersion);
                Environment.Exit(1);
            }

            Log.Information("Starting server...");

            AuthServer.Initialize(new ProudNet.Configuration
            {
                SocketListenerThreads = new MultithreadEventLoopGroup(1),
                SocketWorkerThreads = new MultithreadEventLoopGroup(1),
                WorkerThread = new SingleThreadEventLoop()
            });
            AuthServer.Instance.Listen(Config.Instance.Listener);

            s_apiEventLoopGroup = new MultithreadEventLoopGroup(1);
            s_apiHost = new ServerBootstrap()
                .Group(s_apiEventLoopGroup)
                .Channel<TcpServerSocketChannel>()
                .Handler(new ActionChannelInitializer<IChannel>(ch => { }))
                .ChildHandler(new ActionChannelInitializer<IChannel>(ch =>
                {
                    ch.Pipeline.AddLast(new APIServerHandler());
                }))
                .BindAsync(Config.Instance.API.Listener).WaitEx();

            Log.Information("Ready for connections!");

            if (Config.Instance.NoobMode)
                Log.Warning("!!! NOOB MODE IS ENABLED! EVERY LOGIN SUCCEEDS AND OVERRIDES ACCOUNT LOGIN DETAILS !!!");

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
            lock (s_exitMutex)
            {
                if (s_isExiting)
                    return;

                s_isExiting = true;
            }

            Log.Information("Closing...");
            s_apiHost.CloseAsync().WaitEx();
            s_apiEventLoopGroup.ShutdownGracefullyAsync().WaitEx();
            AuthServer.Instance.Dispose();
        }

        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Log.Error(e.Exception, "UnobservedTaskException");
        }

        private static void OnUnhandledException(object s, UnhandledExceptionEventArgs e)
        {
            Log.Error((Exception)e.ExceptionObject, "UnhandledException");
        }
    }
}
