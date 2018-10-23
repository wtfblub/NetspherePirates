using System;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netsphere.Common;
using Netsphere.Common.Configuration;
using Netsphere.Common.Hosting;
using Netsphere.Database;
using Netsphere.Network.Message.Auth;
using Netsphere.Server.Auth.Handlers;
using ProudNet;
using ProudNet.Hosting;
using Serilog;

namespace Netsphere.Server.Auth
{
    internal static class Program
    {
        private static void Main()
        {
            var baseDirectory = Environment.GetEnvironmentVariable("NETSPHEREPIRATES_BASEDIR_AUTH");
            if (string.IsNullOrWhiteSpace(baseDirectory))
                baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var configuration = Startup.Initialize(baseDirectory, "config.hjson",
                x => x.GetSection(nameof(AppOptions.Logging)).Get<LoggerOptions>());

            Log.Information("Starting...");

            var appOptions = configuration.Get<AppOptions>();
            var hostBuilder = new HostBuilder();

            hostBuilder
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddSingleton<IHostLifetime, ConsoleApplicationLifetime>()
                        .Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromMinutes(1))
                        .Configure<AppOptions>(context.Configuration)
                        .Configure<DatabasesOptions>(context.Configuration.GetSection(nameof(AppOptions.Database)))
                        .AddSingleton<IDatabaseProvider, DatabaseProvider>()
                        .AddSingleton<IDatabaseMigrator, FluentDatabaseMigrator>();
                })
                .ConfigureLogging(builder => builder.AddSerilog())
                .ConfigureHostConfiguration(builder => builder.AddConfiguration(configuration))
                .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
                .UseProudNetServer(builder =>
                {
                    var messageHandlerResolver = new DefaultMessageHandlerResolver(
                        new[] { typeof(AuthenticationHandler).Assembly }, typeof(IAuthMessage));

                    builder
                        .UseHostIdFactory<HostIdFactory>()
                        .UseSessionFactory<SessionFactory>()
                        .AddMessageFactory<AuthMessageFactory>()
                        .UseMessageHandlerResolver(messageHandlerResolver)
                        .UseNetworkConfiguration((context, options) =>
                        {
                            options.Version = new Guid("{9be73c0b-3b10-403e-be7d-9f222702a38c}");
                            options.TcpListener = appOptions.Listener;
                        })
                        .UseThreadingConfiguration((context, options) =>
                        {
                            options.SocketListenerThreadsFactory = () => new MultithreadEventLoopGroup(1);
                            options.SocketWorkerThreadsFactory = () => appOptions.WorkerThreads < 1
                                ? new MultithreadEventLoopGroup()
                                : new MultithreadEventLoopGroup(appOptions.WorkerThreads);
                            options.WorkerThreadFactory = () => new SingleThreadEventLoop();
                        });
                });

            var host = hostBuilder.Build();

            Log.Information("Initializing database provider...");
            var databaseProvider = host.Services.GetRequiredService<IDatabaseProvider>();
            databaseProvider.Initialize();

            Log.Information("Running database migrations...");
            var migrator = host.Services.GetRequiredService<IDatabaseMigrator>();
            if (appOptions.Database.RunMigration)
                migrator.MigrateTo();
            else if (migrator.HasMigrationsToApply())
                throw new DatabaseVersionMismatchException();

            Log.Information("Press Ctrl + C to shutdown");
            host.Run();
        }
    }
}
