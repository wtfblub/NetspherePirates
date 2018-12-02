using System;
using DotNetty.Transport.Channels;
using ExpressMapper;
using Foundatio.Caching;
using Foundatio.Messaging;
using Foundatio.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netsphere.Common;
using Netsphere.Common.Configuration;
using Netsphere.Common.Hosting;
using Netsphere.Database;
using Netsphere.Network.Message.Relay;
using Netsphere.Network.Serializers;
using Newtonsoft.Json;
using ProudNet;
using ProudNet.Hosting;
using Serilog;
using StackExchange.Redis;

namespace Netsphere.Server.Relay
{
    internal static class Program
    {
        private static void Main()
        {
            var baseDirectory = Environment.GetEnvironmentVariable("NETSPHEREPIRATES_BASEDIR_RELAY");
            if (string.IsNullOrWhiteSpace(baseDirectory))
                baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var configuration = Startup.Initialize(baseDirectory, "config.hjson",
                x => x.GetSection(nameof(AppOptions.Logging)).Get<LoggerOptions>());

            Log.Information("Starting...");

            ConfigureMapper();
            var appOptions = configuration.Get<AppOptions>();
            var hostBuilder = new HostBuilder();
            var redisConnectionMultiplexer = ConnectionMultiplexer.Connect(appOptions.Database.ConnectionStrings.Redis);

            hostBuilder
                .ConfigureLogging(builder => builder.AddSerilog())
                .ConfigureHostConfiguration(builder => builder.AddConfiguration(configuration))
                .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
                .UseProudNetServer(builder =>
                {
                    var messageHandlerResolver = new DefaultMessageHandlerResolver(
                        new[] { typeof(Program).Assembly }, typeof(IRelayMessage));

                    builder
                        .UseHostIdFactory<HostIdFactory>()
                        .UseSessionFactory<SessionFactory>()
                        .AddMessageFactory<RelayMessageFactory>()
                        .UseMessageHandlerResolver(messageHandlerResolver)
                        .UseNetworkConfiguration((context, options) =>
                        {
                            options.Version = new Guid("{a43a97d1-9ec7-495e-ad5f-8fe45fde1151}");
                            options.TcpListener = appOptions.Network.Listener;
                            options.UdpAddress = appOptions.Network.Address;
                            options.UdpListenerPorts = appOptions.Network.UdpPorts;
                        })
                        .UseThreadingConfiguration((context, options) =>
                        {
                            options.SocketListenerThreadsFactory = () => new MultithreadEventLoopGroup(1);
                            options.SocketWorkerThreadsFactory = () => appOptions.Network.WorkerThreads < 1
                                ? new MultithreadEventLoopGroup()
                                : new MultithreadEventLoopGroup(appOptions.Network.WorkerThreads);
                            options.WorkerThreadFactory = () => new SingleThreadEventLoop();
                        })
                        .ConfigureSerializer(serializer =>
                        {
                            serializer.AddSerializer(new ItemNumberSerializer());
                            serializer.AddSerializer(new CompressedFloatSerializer());
                            serializer.AddSerializer(new CompressedVector3Serializer());
                            serializer.AddSerializer(new LongPeerIdSerializer());
                            serializer.AddSerializer(new PeerIdSerializer());
                            serializer.AddSerializer(new RoomLocationSerializer());
                            serializer.AddSerializer(new RotationVectorSerializer());
                        });
                })
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddSingleton<IHostLifetime, ConsoleApplicationLifetime>()
                        .Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromMinutes(1))
                        .Configure<AppOptions>(context.Configuration)
                        .Configure<NetworkOptions>(context.Configuration.GetSection(nameof(AppOptions.Network)))
                        .Configure<DatabaseOptions>(context.Configuration.GetSection(nameof(AppOptions.Database)))
                        .Configure<IdGeneratorOptions>(x => x.Id = 0)
                        .AddSingleton<IDatabaseProvider, DatabaseProvider>()
                        .AddSingleton<IDatabaseMigrator, FluentDatabaseMigrator>()
                        .AddSingleton(redisConnectionMultiplexer)
                        .AddTransient<ISerializer>(x => new JsonNetSerializer(JsonConvert.DefaultSettings()))
                        .AddSingleton<ICacheClient, RedisCacheClient>()
                        .AddSingleton<IMessageBus, RedisMessageBus>()
                        .AddSingleton(x => new RedisCacheClientOptions
                        {
                            ConnectionMultiplexer = x.GetRequiredService<ConnectionMultiplexer>(),
                            Serializer = x.GetRequiredService<ISerializer>()
                        })
                        .AddSingleton(x => new RedisMessageBusOptions
                        {
                            Subscriber = x.GetRequiredService<ConnectionMultiplexer>().GetSubscriber(),
                            Serializer = x.GetRequiredService<ISerializer>()
                        })
                        .AddService<IdGeneratorService>()
                        .AddSingleton<PlayerManager>()
                        .AddTransient<Player>()
                        .AddSingleton<RoomManager>();
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

            host.Services.GetRequiredService<IApplicationLifetime>().ApplicationStarted.Register(() =>
                Log.Information("Press Ctrl + C to shutdown"));
            host.Run();
        }

        private static void ConfigureMapper()
        {
            Mapper.Compile(CompilationTypes.Source);
        }
    }
}
