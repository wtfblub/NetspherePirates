using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BlubLib;
using Dapper.FastCrud;
using DotNetty.Transport.Channels;
using Netsphere.Configuration;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Network;
using Newtonsoft.Json;
using Serilog;
using Serilog.Formatting.Json;

namespace Netsphere
{
    internal class Program
    {
        private static readonly object s_exitMutex = new object();
        private static bool s_hasExiting;

        public static Stopwatch AppTime { get; } = Stopwatch.StartNew();

        private static void Main()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new IPEndPointConverter() }
            };
            
            Config<Config>.Initialize("game.hjson", "NETSPHEREPIRATES_GAMECONF");

            var jsonlog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "game.json");
            var logfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "game.log");
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

            Log.Information("Initializing...");

            try
            {
                AuthDatabase.Initialize(Config.Instance.Database.Auth);
                GameDatabase.Initialize(Config.Instance.Database.Game);
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

            ItemIdGenerator.Initialize();
            CharacterIdGenerator.Initialize();
            LicenseIdGenerator.Initialize();
            DenyIdGenerator.Initialize();

            var listenerThreads = new MultithreadEventLoopGroup(Config.Instance.ListenerThreads);
            var workerThreads = new MultithreadEventLoopGroup(Config.Instance.WorkerThreads);
            var workerThread = new SingleThreadEventLoop();
            ChatServer.Initialize(new ProudNet.Configuration
            {
                SocketListenerThreads = listenerThreads,
                SocketWorkerThreads = workerThreads,
                WorkerThread = workerThread
            });
            RelayServer.Initialize(new ProudNet.Configuration
            {
                SocketListenerThreads = listenerThreads,
                SocketWorkerThreads = workerThreads,
                WorkerThread = workerThread
            });
            GameServer.Initialize(new ProudNet.Configuration
            {
                SocketListenerThreads = listenerThreads,
                SocketWorkerThreads = workerThreads,
                WorkerThread = workerThread
            });

            FillShop();

            Log.Information("Starting server...");

            ChatServer.Instance.Listen(Config.Instance.ChatListener);
            RelayServer.Instance.Listen(Config.Instance.RelayListener, IPAddress.Parse(Config.Instance.IP), Config.Instance.RelayUdpPorts);
            GameServer.Instance.Listen(Config.Instance.Listener);

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

                var args = input.GetArgs();
                if (args.Length == 0)
                    continue;

                if (!GameServer.Instance.CommandManager.Execute(null, args))
                    Console.WriteLine("Unknown command");
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
                if (s_hasExiting)
                    return;
                
                Log.Information("Closing...");
                var chat = Task.Run(() => ChatServer.Instance.Dispose());
                var relay = Task.Run(() => RelayServer.Instance.Dispose());
                var game = Task.Run(() => GameServer.Instance.Dispose());
                Task.WaitAll(chat, relay, game);
                s_hasExiting = true;
            }
        }

        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Log.Error(e.Exception, "UnobservedTaskException");
        }

        private static void OnUnhandledException(object s, UnhandledExceptionEventArgs e)
        {
            Log.Error((Exception)e.ExceptionObject, "UnhandledException");
        }

        private static void FillShop()
        {
            if (!Config.Instance.NoobMode)
                return;

            using (var db = GameDatabase.Open())
            {
                if (!db.Find<ShopVersionDto>().Any())
                {
                    var version = new ShopVersionDto
                    {
                        Version = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss")
                    };
                    db.Insert(version);
                }

                if (db.Find<ShopEffectGroupDto>().Any() || db.Find<ShopEffectDto>().Any() ||
                    db.Find<ShopPriceGroupDto>().Any() || db.Find<ShopPriceDto>().Any() ||
                    db.Find<ShopItemDto>().Any() || db.Find<ShopItemInfoDto>().Any())
                    return;

                Log.Information("NoobMode: Filling the shop with items");

                using (var transaction = db.BeginTransaction())
                {
                    var effects = new Dictionary<string, Tuple<uint[], int>>
                    {
                        {"None", Tuple.Create(Array.Empty<uint>(), 0)},
                        {"HP+30", Tuple.Create(new uint[] {1030}, 0)},
                        {"HP+15", Tuple.Create(new uint[] {1015}, 0)},
                        {"SP+40", Tuple.Create(new uint[] {2040}, 0)},
                        {"HP+20 & SP+20", Tuple.Create(new uint[] {4001}, 0)},
                        {"SP+3", Tuple.Create(new uint[] {2003}, 0)},
                        {"Defense+7", Tuple.Create(new uint[] {19907}, 0)},
                        {"HP+3", Tuple.Create(new uint[] {1003}, 0)}
                    };

                    #region Effects

                    foreach (var pair in effects.ToArray())
                    {
                        var effectGroup = new ShopEffectGroupDto { Name = pair.Key };
                        db.Insert(effectGroup, statement => statement.AttachToTransaction(transaction));
                        effects[pair.Key] = Tuple.Create(pair.Value.Item1, effectGroup.Id);

                        foreach (var effect in pair.Value.Item1)
                        {
                            db.Insert(new ShopEffectDto { EffectGroupId = effectGroup.Id, Effect = effect },
                                statement => statement.AttachToTransaction(transaction));
                        }
                    }

                    #endregion

                    #region Price

                    var priceGroup = new ShopPriceGroupDto
                    {
                        Name = "PEN",
                        PriceType = (byte)ItemPriceType.PEN
                    };
                    db.Insert(priceGroup, statement => statement.AttachToTransaction(transaction));

                    var price = new ShopPriceDto
                    {
                        PriceGroupId = priceGroup.Id,
                        PeriodType = (byte)ItemPeriodType.None,
                        IsRefundable = false,
                        Durability = 1000000,
                        IsEnabled = true,
                        Price = 1
                    };
                    db.Insert(price, statement => statement.AttachToTransaction(transaction));

                    #endregion

                    #region Items

                    var items = GameServer.Instance.ResourceCache.GetItems().Values.ToArray();
                    for (var i = 0; i < items.Length; ++i)
                    {
                        var item = items[i];
                        var effectToUse = effects["None"];

                        switch (item.ItemNumber.Category)
                        {
                            case ItemCategory.Weapon:
                                break;

                            case ItemCategory.Skill:
                                if (item.ItemNumber.SubCategory == 0 && item.ItemNumber.Number == 0) // half hp mastery
                                    effectToUse = effects["HP+15"];

                                if (item.ItemNumber.SubCategory == 0 && item.ItemNumber.Number == 1) // hp mastery
                                    effectToUse = effects["HP+30"];

                                if (item.ItemNumber.SubCategory == 0 && item.ItemNumber.Number == 2) // sp mastery
                                    effectToUse = effects["SP+40"];

                                if (item.ItemNumber.SubCategory == 0 && item.ItemNumber.Number == 3) // dual mastery
                                    effectToUse = effects["HP+20 & SP+20"];

                                break;

                            case ItemCategory.Costume:
                                if (item.ItemNumber.SubCategory == (int)CostumeSlot.Hair)
                                    effectToUse = effects["Defense+7"];

                                if (item.ItemNumber.SubCategory == (int)CostumeSlot.Face)
                                    effectToUse = effects["SP+3"];

                                if (item.ItemNumber.SubCategory == (int)CostumeSlot.Pants)
                                    effectToUse = effects["Defense+7"];

                                if (item.ItemNumber.SubCategory == (int)CostumeSlot.Gloves)
                                    effectToUse = effects["HP+3"];

                                if (item.ItemNumber.SubCategory == (int)CostumeSlot.Shoes)
                                    effectToUse = effects["HP+3"];

                                if (item.ItemNumber.SubCategory == (int)CostumeSlot.Accessory)
                                    effectToUse = effects["SP+3"];
                                break;

                            default:
                                continue;
                        }

                        var shopItem = new ShopItemDto
                        {
                            Id = item.ItemNumber,
                            RequiredGender = (byte)item.Gender,
                            RequiredLicense = (byte)item.License,
                            IsDestroyable = true
                        };
                        db.Insert(shopItem, statement => statement.AttachToTransaction(transaction));

                        var shopItemInfo = new ShopItemInfoDto
                        {
                            ShopItemId = shopItem.Id,
                            PriceGroupId = priceGroup.Id,
                            EffectGroupId = effectToUse.Item2,
                            IsEnabled = true
                        };
                        db.Insert(shopItemInfo, statement => statement.AttachToTransaction(transaction));

                        Log.Information($"[{i}/{items.Length}] {item.ItemNumber}: {item.Name}");
                    }

                    #endregion

                    try
                    {
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

    }
}
