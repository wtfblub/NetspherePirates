using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BlubLib;
using Netsphere.Network;
using Newtonsoft.Json;
using NLog;
using NLog.Fluent;
using Shaolinq;
using Shaolinq.MySql;
using Shaolinq.Sqlite;

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

            Shaolinq.Logging.LogProvider.IsDisabled = true;

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            Logger.Info("Checking database...");

            AuthDatabase.Instance.CreateIfNotExist();

            using (var scope = new DataAccessScope(DataAccessIsolationLevel.Unspecified, TimeSpan.FromMinutes(5)))
                GameDatabase.Instance.CreateIfNotExist();

            FillShop();

            ItemIdGenerator.Initialize();
            CharacterIdGenerator.Initialize();
            LicenseIdGenerator.Initialize();
            DenyIdGenerator.Initialize();

            Logger.Info("Starting server...");

            GameServer.Instance.Start(Config.Instance.Listener);

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
            var db = GameDatabase.Instance;
            if (!db.ShopVersion.Any())
            {
                using (var scope = new DataAccessScope())
                {
                    var version = db.ShopVersion.Create(1);
                    version.Version = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");
                    scope.Complete();
                }
            }

            if (!Config.Instance.NoobMode ||
                db.ShopEffectGroups.Any() || db.ShopEffects.Any() ||
                db.ShopPriceGroups.Any() || db.ShopPrices.Any() ||
                db.ShopItemInfos.Any() || db.ShopItems.Any())
                return;

            Logger.Info("NoobMode: Filling the shop with items");


            using (var scope = new DataAccessScope())
            {
                #region Effects

                var noneEffectGroup = db.ShopEffectGroups.Create();
                noneEffectGroup.Name = "None";

                var hp30EffectGroup = db.ShopEffectGroups.Create();
                hp30EffectGroup.Name = "HP+30";
                hp30EffectGroup.ShopEffects.Create().Effect = 1030;

                var hp15EffectGroup = db.ShopEffectGroups.Create();
                hp15EffectGroup.Name = "HP+15";
                hp15EffectGroup.ShopEffects.Create().Effect = 1015;

                var sp40EffectGroup = db.ShopEffectGroups.Create();
                sp40EffectGroup.Name = "SP+40";
                sp40EffectGroup.ShopEffects.Create().Effect = 2040;

                var dualMasteryEffectGroup = db.ShopEffectGroups.Create();
                dualMasteryEffectGroup.Name = "HP+20 & SP+20";
                dualMasteryEffectGroup.ShopEffects.Create().Effect = 4001;

                var sp3EffectGroup = db.ShopEffectGroups.Create();
                sp3EffectGroup.Name = "SP+3";
                sp3EffectGroup.ShopEffects.Create().Effect = 2003;

                var defense7EffectGroup = db.ShopEffectGroups.Create();
                defense7EffectGroup.Name = "Defense+7";
                defense7EffectGroup.ShopEffects.Create().Effect = 19907;

                var hp3EffectGroup = db.ShopEffectGroups.Create();
                hp3EffectGroup.Name = "HP+3";
                hp3EffectGroup.ShopEffects.Create().Effect = 1003;

                #endregion

                #region Price

                var priceGroup = db.ShopPriceGroups.Create();
                priceGroup.Name = "PEN";
                priceGroup.PriceType = (byte)ItemPriceType.PEN;

                var price = priceGroup.ShopPrices.Create();
                price.PeriodType = (byte)ItemPeriodType.None;
                price.IsRefundable = false;
                price.Durability = 1000000;
                price.IsEnabled = true;
                price.Price = 1;

                #endregion

                #region Items

                var items = GameServer.Instance.ResourceCache.GetItems().Values.ToArray();
                for (var i = 0; i < items.Length; ++i)
                {
                    var item = items[i];
                    var effectToUse = noneEffectGroup;

                    switch (item.ItemNumber.Category)
                    {
                        case ItemCategory.Weapon:
                            break;

                        case ItemCategory.Skill:
                            if (item.ItemNumber.SubCategory == 0 && item.ItemNumber.Number == 0) // half hp mastery
                                effectToUse = hp15EffectGroup;

                            if (item.ItemNumber.SubCategory == 0 && item.ItemNumber.Number == 1) // hp mastery
                                effectToUse = hp30EffectGroup;

                            if (item.ItemNumber.SubCategory == 0 && item.ItemNumber.Number == 2) // sp mastery
                                effectToUse = sp40EffectGroup;

                            if (item.ItemNumber.SubCategory == 0 && item.ItemNumber.Number == 3) // dual mastery
                                effectToUse = dualMasteryEffectGroup;

                            break;

                        case ItemCategory.Costume:
                            if (item.ItemNumber.SubCategory == (int)CostumeSlot.Hair)
                                effectToUse = defense7EffectGroup;

                            if (item.ItemNumber.SubCategory == (int)CostumeSlot.Face)
                                effectToUse = sp3EffectGroup;

                            if (item.ItemNumber.SubCategory == (int)CostumeSlot.Pants)
                                effectToUse = defense7EffectGroup;

                            if (item.ItemNumber.SubCategory == (int)CostumeSlot.Gloves)
                                effectToUse = hp3EffectGroup;

                            if (item.ItemNumber.SubCategory == (int)CostumeSlot.Shoes)
                                effectToUse = hp3EffectGroup;

                            if (item.ItemNumber.SubCategory == (int)CostumeSlot.Accessory)
                                effectToUse = sp3EffectGroup;
                            break;

                        default:
                            continue;
                    }

                    var shopItem = db.ShopItems.Create((uint)item.ItemNumber);
                    shopItem.RequiredGender = (byte)item.Gender;
                    shopItem.RequiredLicense = (byte)item.License;
                    shopItem.IsDestroyable = true;

                    var shopItemInfo = shopItem.ItemInfos.Create();
                    shopItemInfo.PriceGroup = priceGroup;
                    shopItemInfo.EffectGroup = effectToUse;
                    shopItemInfo.IsEnabled = true;

                    Logger.Info($"[{i}/{items.Length}] {item.ItemNumber}: {item.Name}");
                }

                #endregion

                scope.Complete();
            }
        }
    }

    internal static class AuthDatabase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static Database.Auth.AuthDatabase Instance { get; }

        static AuthDatabase()
        {
            var config = Config.Instance.AuthDatabase;

            DataAccessModelConfiguration dbConfig;
            switch (Config.Instance.AuthDatabase.Engine)
            {
                case DatabaseEngine.MySQL:
                    dbConfig = MySqlConfiguration.Create(config.Database, config.Host, config.Username, config.Password, true);
                    break;

                case DatabaseEngine.SQLite:
                    if (Utilities.IsMono)
                        throw new NotSupportedException("SQLite is not supported on mono. Please switch to MySQL");
                    dbConfig = SqliteConfiguration.Create(config.Filename, null, Utilities.IsMono);
                    break;

                default:
                    Logger.Error()
                        .Message("Invalid database engine {0}", Config.Instance.AuthDatabase.Engine)
                        .Write();
                    Environment.Exit(0);
                    return;

            }

            Instance = DataAccessModel.BuildDataAccessModel<Database.Auth.AuthDatabase>(dbConfig);
        }
    }

    internal static class GameDatabase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static Database.Game.GameDatabase Instance { get; }

        static GameDatabase()
        {
            var config = Config.Instance.GameDatabase;

            DataAccessModelConfiguration dbConfig;
            switch (Config.Instance.GameDatabase.Engine)
            {
                case DatabaseEngine.MySQL:
                    dbConfig = MySqlConfiguration.Create(config.Database, config.Host, config.Username, config.Password, true);
                    break;

                case DatabaseEngine.SQLite:
                    if (Utilities.IsMono)
                        throw new NotSupportedException("SQLite is not supported on mono. Please switch to MySQL");
                    dbConfig = SqliteConfiguration.Create(config.Filename, null, Utilities.IsMono);
                    break;

                default:
                    Logger.Error()
                        .Message("Invalid database engine {0}", Config.Instance.AuthDatabase.Engine)
                        .Write();
                    Environment.Exit(0);
                    return;

            }

            Instance = DataAccessModel.BuildDataAccessModel<Database.Game.GameDatabase>(dbConfig);
        }
    }
}
