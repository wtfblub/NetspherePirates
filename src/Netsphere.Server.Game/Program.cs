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
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;
using Netsphere.Network.Serializers;
using Netsphere.Server.Game.Commands;
using Netsphere.Server.Game.Handlers;
using Netsphere.Server.Game.Serializers;
using Netsphere.Server.Game.Services;
using Newtonsoft.Json;
using ProudNet;
using ProudNet.Hosting;
using Serilog;
using StackExchange.Redis;

namespace Netsphere.Server.Game
{
    internal static class Program
    {
        private static void Main()
        {
            var baseDirectory = Environment.GetEnvironmentVariable("NETSPHEREPIRATES_BASEDIR_GAME");
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
                        new[] { typeof(AuthenticationHandler).Assembly }, typeof(IGameMessage), typeof(IGameRuleMessage));

                    builder
                        .UseHostIdFactory<HostIdFactory>()
                        .UseSessionFactory<SessionFactory>()
                        .AddMessageFactory<GameMessageFactory>()
                        .AddMessageFactory<GameRuleMessageFactory>()
                        .UseMessageHandlerResolver(messageHandlerResolver)
                        .UseNetworkConfiguration((context, options) =>
                        {
                            options.Version = new Guid("{beb92241-8333-4117-ab92-9b4af78c688f}");
                            options.TcpListener = appOptions.Network.Listener;
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
                            serializer.AddSerializer(new CharacterStyleSerializer());
                            serializer.AddSerializer(new ItemNumberSerializer());
                            serializer.AddSerializer(new MatchKeySerializer());
                            serializer.AddSerializer(new VersionSerializer());
                            serializer.AddSerializer(new ShopPriceSerializer());
                            serializer.AddSerializer(new ShopEffectSerializer());
                            serializer.AddSerializer(new ShopItemSerializer());
                        });
                })
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddSingleton<IHostLifetime, ConsoleApplicationLifetime>()
                        .Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromMinutes(1))
                        .Configure<AppOptions>(context.Configuration)
                        .Configure<NetworkOptions>(context.Configuration.GetSection(nameof(AppOptions.Network)))
                        .Configure<ServerListOptions>(context.Configuration.GetSection(nameof(AppOptions.ServerList)))
                        .Configure<DatabaseOptions>(context.Configuration.GetSection(nameof(AppOptions.Database)))
                        .Configure<GameOptions>(context.Configuration.GetSection(nameof(AppOptions.Game)))
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
                        .AddTransient<Player>()
                        .AddTransient<LicenseManager>()
                        .AddTransient<CharacterManager>()
                        .AddTransient<PlayerInventory>()
                        .AddSingleton<PlayerManager>()
                        .AddCommands(typeof(Program).Assembly)
                        .AddService<IdGeneratorService>()
                        .AddHostedServiceEx<ServerlistService>()
                        .AddHostedServiceEx<GameDataService>()
                        .AddHostedServiceEx<ChannelService>()
                        .AddHostedServiceEx<IpcService>()
                        .AddHostedServiceEx<PlayerSaveService>()
                        .AddHostedServiceEx<CommandService>();
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
            Mapper.Register<Channel, ChannelInfoDto>()
                .Member(dest => dest.ChannelId, src => src.Id)
                .Member(dest => dest.PlayerCount, src => src.Players.Count);

            Mapper.Register<PlayerItem, ItemDto>()
                .Member(dest => dest.Refund, src => src.CalculateRefund())
                .Member(dest => dest.PurchaseTime, src => src.PurchaseDate.ToUnixTimeSeconds())
                .Member(dest => dest.ExpireTime,
                    src => src.ExpireDate == DateTimeOffset.MinValue ? -1 : src.ExpireDate.ToUnixTimeSeconds())

                // ToDo
                .Value(dest => dest.TimeLeft, 0)
                .Value(dest => dest.Unk1, (uint)0)
                .Value(dest => dest.Unk2, 0)
                .Value(dest => dest.Unk3, 0)
                .Value(dest => dest.Unk4, 0)
                .Value(dest => dest.Unk5, (uint)0)
                .Value(dest => dest.Unk6, (uint)0);

            Mapper.Register<PlayerItem, ItemDurabilityInfoDto>()
                .Member(dest => dest.ItemId, src => src.Id);

            Mapper.Compile(CompilationTypes.Source);
        }
    }
}
