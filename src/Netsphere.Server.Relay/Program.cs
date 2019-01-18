using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetty.Transport.Channels;
using ExpressMapper;
using Foundatio.Caching;
using Foundatio.Messaging;
using Foundatio.Serializer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netsphere.Common;
using Netsphere.Common.Configuration;
using Netsphere.Common.Hosting;
using Netsphere.Common.Plugins;
using Netsphere.Database;
using Netsphere.Network.Message.Event;
using Netsphere.Network.Message.P2P;
using Netsphere.Network.Message.Relay;
using Netsphere.Network.Serializers;
using Netsphere.Server.Relay.Services;
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

            var appOptions = configuration.Get<AppOptions>();
            var hostBuilder = new HostBuilder();
            var redisConnectionMultiplexer = ConnectionMultiplexer.Connect(appOptions.Database.ConnectionStrings.Redis);

            IPluginHost pluginHost = new MefPluginHost();
            pluginHost.Initialize(configuration, Path.Combine(baseDirectory, "plugins"));

            ConfigureMapper();

            hostBuilder
                .ConfigureHostConfiguration(builder => builder.AddConfiguration(configuration))
                .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
                .UseProudNetServer(builder =>
                {
                    var messageHandlerResolver = new DefaultMessageHandlerResolver(
                        AppDomain.CurrentDomain.GetAssemblies(),
                        typeof(IRelayMessage), typeof(IEventMessage), typeof(IP2PMessage));

                    builder
                        .UseHostIdFactory<HostIdFactory>()
                        .UseSessionFactory<SessionFactory>()
                        .AddMessageFactory<RelayMessageFactory>()
                        .AddMessageFactory<EventMessageFactory>()
                        .UseMessageHandlerResolver(messageHandlerResolver)
                        .UseNetworkConfiguration((context, options) =>
                        {
                            options.Version = new Guid("{a43a97d1-9ec7-495e-ad5f-8fe45fde1151}");
                            options.TcpListener = appOptions.Network.Listener;
                            options.UdpAddress = appOptions.Network.Address;
                            options.UdpListenerPorts = appOptions.Network.UdpPorts;
                            options.ServerAsP2PGroupMemberHack = true;
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
                        .AddSingleton<DatabaseService>()
                        .AddDbContext<GameContext>(x => x.UseMySql(appOptions.Database.ConnectionStrings.Game))
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
                        .AddHostedServiceEx<IpcService>()
                        .AddSingleton<PlayerManager>()
                        .AddTransient<Player>()
                        .AddSingleton<RoomManager>();

                    pluginHost.OnConfigure(services);
                });

            var host = hostBuilder.Build();

            var contexts = host.Services.GetRequiredService<IEnumerable<DbContext>>();
            foreach (var db in contexts)
            {
                Log.Information("Checking database={Context}...", db.GetType().Name);

                using (db)
                {
                    if (db.Database.GetPendingMigrations().Any())
                    {
                        if (appOptions.Database.RunMigration)
                        {
                            Log.Information("Applying database={Context} migrations...", db.GetType().Name);
                            db.Database.Migrate();
                        }
                        else
                        {
                            Log.Error("Database={Context} does not have all migrations applied", db.GetType().Name);
                            return;
                        }
                    }
                }
            }

            host.Services.GetRequiredService<IApplicationLifetime>().ApplicationStarted.Register(() =>
                Log.Information("Press Ctrl + C to shutdown"));
            host.Run();
            pluginHost.Dispose();
        }

        private static void ConfigureMapper()
        {
            Mapper.Compile(CompilationTypes.Source);
        }
    }
}
