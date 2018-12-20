using System;
using DotNetty.Transport.Channels;
using ExpressMapper;
using ExpressMapper.Extensions;
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
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;
using Netsphere.Server.Chat.Handlers;
using Netsphere.Server.Chat.Services;
using Newtonsoft.Json;
using ProudNet;
using ProudNet.Hosting;
using ProudNet.Hosting.Services;
using Serilog;
using StackExchange.Redis;

namespace Netsphere.Server.Chat
{
    internal static class Program
    {
        private static void Main()
        {
            var baseDirectory = Environment.GetEnvironmentVariable("NETSPHEREPIRATES_BASEDIR_CHAT");
            if (string.IsNullOrWhiteSpace(baseDirectory))
                baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var configuration = Startup.Initialize(baseDirectory, "config.hjson",
                x => x.GetSection(nameof(AppOptions.Logging)).Get<LoggerOptions>());

            Log.Information("Starting...");

            var appOptions = configuration.Get<AppOptions>();
            var hostBuilder = new HostBuilder();
            var redisConnectionMultiplexer = ConnectionMultiplexer.Connect(appOptions.Database.ConnectionStrings.Redis);

            ConfigureMapper(appOptions);

            hostBuilder
                .ConfigureHostConfiguration(builder => builder.AddConfiguration(configuration))
                .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
                .UseProudNetServer(builder =>
                {
                    var messageHandlerResolver = new DefaultMessageHandlerResolver(
                        new[] { typeof(AuthenticationHandler).Assembly }, typeof(IChatMessage));

                    builder
                        .UseHostIdFactory<HostIdFactory>()
                        .UseSessionFactory<SessionFactory>()
                        .AddMessageFactory<ChatMessageFactory>()
                        .UseMessageHandlerResolver(messageHandlerResolver)
                        .UseNetworkConfiguration((context, options) =>
                        {
                            options.Version = new Guid("{97d36acf-8cc0-4dfb-bcc9-97cab255e2bc}");
                            options.TcpListener = appOptions.Network.Listener;
                        })
                        .UseThreadingConfiguration((context, options) =>
                        {
                            options.SocketListenerThreadsFactory = () => new MultithreadEventLoopGroup(1);
                            options.SocketWorkerThreadsFactory = () => appOptions.Network.WorkerThreads < 1
                                ? new MultithreadEventLoopGroup()
                                : new MultithreadEventLoopGroup(appOptions.Network.WorkerThreads);
                            options.WorkerThreadFactory = () => new SingleThreadEventLoop();
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
                        .Configure<IdGeneratorOptions>(x => x.Id = 1)
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
                        .AddTransient<Mailbox>()
                        .AddTransient<DenyManager>()
                        .AddTransient<PlayerSettingManager>()
                        .AddSingleton<PlayerManager>()
                        .AddSingleton<ChannelManager>()
                        .AddService<IdGeneratorService>()
                        .AddHostedServiceEx<ServerlistService>()
                        .AddHostedServiceEx<IpcService>();
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

            host.Services
                .GetRequiredService<IProudNetServerService>()
                .UnhandledRmi += (s, e) => Log.Debug("Unhandled Message={@Message} HostId={HostId}", e.Message, e.Session.HostId);

            host.Services.GetRequiredService<IApplicationLifetime>().ApplicationStarted.Register(() =>
                Log.Information("Press Ctrl + C to shutdown"));
            host.Run();
        }

        private static void ConfigureMapper(AppOptions appOptions)
        {
            Mapper.Register<Mail, NoteDto>()
                .Function(dest => dest.ReadCount, src => src.IsNew ? 0 : 1)
                .Function(dest => dest.DaysLeft,
                    src => DateTimeOffset.Now < src.Expires ? (src.Expires - DateTimeOffset.Now).TotalDays : 0);

            Mapper.Register<Mail, NoteContentDto>()
                .Member(dest => dest.Id, src => src.Id)
                .Member(dest => dest.Message, src => src.Message);

            Mapper.Register<Deny, DenyDto>()
                .Member(dest => dest.AccountId, src => src.DenyId)
                .Member(dest => dest.Nickname, src => src.Nickname);

            Mapper.Register<Player, UserDataDto>()
                .Member(dest => dest.AccountId, src => src.Account.Id)
                .Member(dest => dest.ServerId, src => appOptions.ServerList.Id)
                .Function(dest => dest.ChannelId, src => src.Channel != null ? (short)src.Channel.Id : (short)-1)
                .Function(dest => dest.RoomId, src => src.RoomId == 0 ? 0xFFFFFFFF : src.RoomId) // ToDo: Tutorial, License
                .Function(dest => dest.Team, src => src.TeamId)
                .Function(dest => dest.TotalExperience, src => src.TotalExperience);

            Mapper.Register<Player, UserDataWithNickDto>()
                .Member(dest => dest.AccountId, src => src.Account.Id)
                .Member(dest => dest.Nickname, src => src.Account.Nickname)
                .Function(dest => dest.Data, src => src.Map<Player, UserDataDto>());

            Mapper.Compile(CompilationTypes.Source);
        }
    }
}
