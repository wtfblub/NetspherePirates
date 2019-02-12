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
using Netsphere.Common.Plugins;
using Netsphere.Database;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;
using Netsphere.Network.Serializers;
using Netsphere.Server.Game.GameRules;
using Netsphere.Server.Game.Serializers;
using Netsphere.Server.Game.Services;
using Newtonsoft.Json;
using ProudNet;
using ProudNet.Hosting;
using ProudNet.Hosting.Services;
using Serilog;
using StackExchange.Redis;

namespace Netsphere.Server.Game
{
    internal static class Program
    {
        public static string BaseDirectory { get; private set; }

        private static void Main()
        {
            BaseDirectory = Environment.GetEnvironmentVariable("NETSPHEREPIRATES_BASEDIR_GAME");
            if (string.IsNullOrWhiteSpace(BaseDirectory))
                BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var configuration = Startup.Initialize(BaseDirectory, "config.hjson",
                x => x.GetSection(nameof(AppOptions.Logging)).Get<LoggerOptions>());

            Log.Information("Starting...");

            var appOptions = configuration.Get<AppOptions>();
            var hostBuilder = new HostBuilder();
            var redisConnectionMultiplexer = ConnectionMultiplexer.Connect(appOptions.Database.ConnectionStrings.Redis);

            IPluginHost pluginHost = new MefPluginHost();
            pluginHost.Initialize(configuration, Path.Combine(BaseDirectory, "plugins"));

            ConfigureMapper();

            hostBuilder
                .ConfigureHostConfiguration(builder => builder.AddConfiguration(configuration))
                .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
                .UseConsoleLifetime()
                .UseProudNetServer(builder =>
                {
                    var messageHandlerResolver = new DefaultMessageHandlerResolver(
                        AppDomain.CurrentDomain.GetAssemblies(), typeof(IGameMessage), typeof(IGameRuleMessage));

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
                            serializer.AddSerializer(new PeerIdSerializer());
                            serializer.AddSerializer(new LongPeerIdSerializer());
                        });
                })
                .ConfigureServices((context, services) =>
                {
                    services
                        .Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true)
                        .Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromMinutes(1))
                        .Configure<AppOptions>(context.Configuration)
                        .Configure<NetworkOptions>(context.Configuration.GetSection(nameof(AppOptions.Network)))
                        .Configure<ServerListOptions>(context.Configuration.GetSection(nameof(AppOptions.ServerList)))
                        .Configure<DatabaseOptions>(context.Configuration.GetSection(nameof(AppOptions.Database)))
                        .Configure<GameOptions>(context.Configuration.GetSection(nameof(AppOptions.Game)))
                        .Configure<DeathmatchOptions>(context.Configuration
                            .GetSection(nameof(AppOptions.Game))
                            .GetSection(nameof(AppOptions.Game.Deathmatch)))
                        .Configure<TouchdownOptions>(context.Configuration
                            .GetSection(nameof(AppOptions.Game))
                            .GetSection(nameof(AppOptions.Game.Touchdown)))
                        .Configure<BattleRoyalOptions>(context.Configuration
                            .GetSection(nameof(AppOptions.Game))
                            .GetSection(nameof(AppOptions.Game.BattleRoyal)))
                        .Configure<IdGeneratorOptions>(x => x.Id = 0)
                        .AddSingleton<DatabaseService>()
                        .AddDbContext<AuthContext>(x => x.UseMySql(appOptions.Database.ConnectionStrings.Auth))
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
                        .AddTransient<Player>()
                        .AddTransient<LicenseManager>()
                        .AddTransient<CharacterManager>()
                        .AddTransient<PlayerInventory>()
                        .AddSingleton<PlayerManager>()
                        .AddTransient<RoomManager>()
                        .AddTransient<Room>()
                        .AddSingleton<GameRuleResolver>()
                        .AddTransient<GameRuleStateMachine>()
                        .AddTransient<Deathmatch>()
                        .AddTransient<Touchdown>()
                        .AddTransient<BattleRoyal>()
                        .AddCommands(typeof(Program).Assembly)
                        .AddService<IdGeneratorService>()
                        .AddHostedServiceEx<ServerlistService>()
                        .AddHostedServiceEx<GameDataService>()
                        .AddHostedServiceEx<ChannelService>()
                        .AddHostedServiceEx<IpcService>()
                        .AddHostedServiceEx<PlayerSaveService>()
                        .AddHostedServiceEx<CommandService>();

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

            host.Services
                .GetRequiredService<IProudNetServerService>()
                .UnhandledRmi += (s, e) => Log.Debug("Unhandled Message={@Message} HostId={HostId}", e.Message, e.Session.HostId);

            host.Services.GetRequiredService<IApplicationLifetime>().ApplicationStarted.Register(() =>
                Log.Information("Press Ctrl + C to shutdown"));
            host.Run();
            pluginHost.Dispose();
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

            Mapper.Register<Room, RoomDto>()
                .Member(dest => dest.RoomId, src => src.Id)
                .Member(dest => dest.MatchKey, src => src.Options.MatchKey)
                .Member(dest => dest.Name, src => src.Options.Name)
                .Member(dest => dest.HasPassword, src => !string.IsNullOrWhiteSpace(src.Options.Password))
                .Member(dest => dest.TimeLimit, src => src.Options.TimeLimit.TotalMilliseconds)
                .Member(dest => dest.ScoreLimit, src => src.Options.ScoreLimit)
                .Member(dest => dest.IsFriendly, src => src.Options.IsFriendly)
                .Member(dest => dest.IsBalanced, src => src.Options.IsBalanced)
                .Member(dest => dest.MinLevel, src => src.Options.MinLevel)
                .Member(dest => dest.MaxLevel, src => src.Options.MaxLevel)
                .Member(dest => dest.EquipLimit, src => src.Options.ItemLimit)
                .Member(dest => dest.IsNoIntrusion, src => src.Options.IsNoIntrusion)
                .Member(dest => dest.ConnectingCount, src => src.TeamManager.Players.Count())
                .Member(dest => dest.PlayerCount, src => src.TeamManager.Players.Count())
                .Function(dest => dest.Latency, src =>
                {
                    const int good = 30;
                    const int bad = 190;

                    var averagePing = src.GetAveragePing();

                    if (averagePing <= good)
                        return 100;

                    if (averagePing >= bad)
                        return 0;

                    var result = (uint)(100f * averagePing / bad);
                    return (byte)(100 - result);
                })
                .Member(dest => dest.State, src => src.GameRule.StateMachine.GameState);

            Mapper.Register<Room, EnterRoomInfoDto>()
                .Member(dest => dest.RoomId, src => src.Id)
                .Member(dest => dest.MatchKey, src => src.Options.MatchKey)
                .Member(dest => dest.TimeLimit, src => src.Options.TimeLimit.TotalMilliseconds)
                .Member(dest => dest.TimeSync, src => src.GameRule.StateMachine.RoundTime.TotalMilliseconds)
                .Member(dest => dest.ScoreLimit, src => src.Options.ScoreLimit)
                .Member(dest => dest.IsFriendly, src => src.Options.IsFriendly)
                .Member(dest => dest.IsBalanced, src => src.Options.IsBalanced)
                .Member(dest => dest.MinLevel, src => src.Options.MinLevel)
                .Member(dest => dest.MaxLevel, src => src.Options.MaxLevel)
                .Member(dest => dest.ItemLimit, src => src.Options.ItemLimit)
                .Member(dest => dest.IsNoIntrusion, src => src.Options.IsNoIntrusion)
                .Member(dest => dest.RelayEndPoint, src => src.Options.RelayEndPoint)
                .Member(dest => dest.State, src => src.GameRule.StateMachine.GameState)
                .Function(dest => dest.TimeState, src => src.GameRule.StateMachine.TimeState);

            Mapper.Register<Player, RoomPlayerDto>()
                .Member(dest => dest.AccountId, src => src.Account.Id)
                .Member(dest => dest.Nickname, src => src.Account.Nickname)
                .Value(dest => dest.Unk1, (byte)144);

            Mapper.Register<RoomCreationOptions, ChangeRuleDto>()
                .Member(dest => dest.Name, src => src.Name)
                .Member(dest => dest.Password, src => src.Password)
                .Function(dest => dest.MatchKey, src => src.MatchKey)
                .Member(dest => dest.TimeLimit, src => src.TimeLimit)
                .Member(dest => dest.ScoreLimit, src => src.ScoreLimit)
                .Member(dest => dest.IsFriendly, src => src.IsFriendly)
                .Member(dest => dest.IsBalanced, src => src.IsBalanced)
                .Member(dest => dest.ItemLimit, src => src.ItemLimit)
                .Member(dest => dest.IsNoIntrusion, src => src.IsNoIntrusion);

            Mapper.Compile(CompilationTypes.Source);
        }
    }
}
