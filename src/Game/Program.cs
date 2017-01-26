using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlubLib;
using Dapper;
using Dapper.FastCrud;
using DotNetty.Transport.Channels;
using Netsphere.Database.Game;
using Netsphere.Network;
using Newtonsoft.Json;
using NLog;
using ProudNet;

namespace Netsphere
{
    internal class Program
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static Stopwatch AppTime { get; } = Stopwatch.StartNew();

        private static void Main()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new IPEndPointConverter() }
            };

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            Logger.Info("Initializing...");

            AuthDatabase.Initialize();
            GameDatabase.Initialize();

            ItemIdGenerator.Initialize();
            CharacterIdGenerator.Initialize();
            LicenseIdGenerator.Initialize();
            DenyIdGenerator.Initialize();

            ChatServer.Initialize(new Configuration());
            RelayServer.Initialize(new Configuration());
            GameServer.Initialize(new Configuration());

            FillShop();

            Logger.Info("Starting server...");

            var eventLoopGroup = new MultithreadEventLoopGroup();
            ChatServer.Instance.Listen(Config.Instance.ChatListener, eventLoopGroup: eventLoopGroup);
            RelayServer.Instance.Listen(Config.Instance.RelayListener, Config.Instance.RelayUdpPorts, eventLoopGroup);
            GameServer.Instance.Listen(Config.Instance.Listener, eventLoopGroup: eventLoopGroup);

            Logger.Info("Ready for connections!");

            if (Config.Instance.NoobMode)
                Logger.Warn("!!! NOOB MODE IS ENABLED! EVERY LOGIN SUCCEEDS AND OVERRIDES ACCOUNT LOGIN DETAILS !!!");

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
            Logger.Info("Closing...");

            ChatServer.Instance.Dispose();
            RelayServer.Instance.Dispose();
            GameServer.Instance.Dispose();
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

                Logger.Info("NoobMode: Filling the shop with items");

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
                            db.Insert(new ShopEffectDto { EffectGroupId = effectGroup.Id, Effect = effect },
                                statement => statement.AttachToTransaction(transaction));
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

                        Logger.Info($"[{i}/{items.Length}] {item.ItemNumber}: {item.Name}");
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

    internal static class AuthDatabase
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static string s_connectionString;

        public static void Initialize()
        {
            Logger.Info("Initializing...");

            var config = Config.Instance.Database;

            switch (config.Engine)
            {
                case DatabaseEngine.MySQL:
                    s_connectionString =
                        $"Server={config.Auth.Host};Port={config.Auth.Port};Database={config.Auth.Database};Uid={config.Auth.Username};Pwd={config.Auth.Password};Pooling=true;";
                    OrmConfiguration.DefaultDialect = SqlDialect.MySql;

                    using (var con = Open())
                    {
                        if (con.QueryFirstOrDefault($"SHOW DATABASES LIKE \"{config.Auth.Database}\"") == null)
                        {
                            Logger.Error($"Database '{config.Auth.Database}' not found");
                            Environment.Exit(0);
                        }
                    }
                    break;

                case DatabaseEngine.SQLite:
                    if (Utilities.IsMono)
                    {
                        Logger.Error("SQLite is not supported on mono");
                        Environment.Exit(0);
                    }
                    s_connectionString = $"Data Source={config.Auth.Filename};Pooling=true;";
                    OrmConfiguration.DefaultDialect = SqlDialect.SqLite;

                    if (!File.Exists(config.Auth.Filename))
                    {
                        Logger.Error($"Database '{config.Auth.Filename}' not found");
                        Environment.Exit(0);
                    }
                    break;

                default:
                    Logger.Error($"Invalid database engine {config.Engine}");
                    Environment.Exit(0);
                    return;
            }
        }

        public static IDbConnection Open()
        {
            var engine = Config.Instance.Database.Engine;
            IDbConnection connection;
            switch (engine)
            {
                case DatabaseEngine.MySQL:
                    connection = new MySql.Data.MySqlClient.MySqlConnection(s_connectionString);
                    break;

                case DatabaseEngine.SQLite:
                    connection = new System.Data.SQLite.SQLiteConnection(s_connectionString);
                    break;

                default:
                    Logger.Error($"Invalid database engine {engine}");
                    Environment.Exit(0);
                    return null;
            }
            connection.Open();
            return connection;
        }
    }

    internal static class GameDatabase
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static string s_connectionString;

        public static void Initialize()
        {
            Logger.Info("Initializing...");

            var config = Config.Instance.Database;

            switch (config.Engine)
            {
                case DatabaseEngine.MySQL:
                    s_connectionString =
                        $"Server={config.Game.Host};Port={config.Game.Port};Database={config.Game.Database};Uid={config.Game.Username};Pwd={config.Game.Password};Pooling=true;";
                    OrmConfiguration.DefaultDialect = SqlDialect.MySql;

                    using (var con = Open())
                    {
                        if (con.QueryFirstOrDefault($"SHOW DATABASES LIKE \"{config.Game.Database}\"") == null)
                        {
                            Logger.Error($"Database '{config.Game.Database}' not found");
                            Environment.Exit(0);
                        }
                    }
                    break;

                case DatabaseEngine.SQLite:
                    if (Utilities.IsMono)
                    {
                        Logger.Error("SQLite is not supported on mono");
                        Environment.Exit(0);
                    }
                    s_connectionString = $"Data Source={config.Game.Filename};Pooling=true;";
                    OrmConfiguration.DefaultDialect = SqlDialect.SqLite;

                    if (!File.Exists(config.Game.Filename))
                    {
                        Logger.Error($"Database '{config.Game.Filename}' not found");
                        Environment.Exit(0);
                    }
                    break;

                default:
                    Logger.Error($"Invalid database engine {config.Engine}");
                    Environment.Exit(0);
                    return;
            }
        }

        public static IDbConnection Open()
        {
            var engine = Config.Instance.Database.Engine;
            IDbConnection connection;
            switch (engine)
            {
                case DatabaseEngine.MySQL:
                    connection = new MySql.Data.MySqlClient.MySqlConnection(s_connectionString);
                    break;

                case DatabaseEngine.SQLite:
                    connection = new System.Data.SQLite.SQLiteConnection(s_connectionString);
                    break;

                default:
                    Logger.Error($"Invalid database engine {engine}");
                    Environment.Exit(0);
                    return null;
            }
            connection.Open();
            return connection;
        }
    }
}
