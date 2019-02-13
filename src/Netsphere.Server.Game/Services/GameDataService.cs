using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Logging;
using Microsoft.Extensions.Hosting;
using Netsphere.Database;
using Netsphere.Server.Game.Data;

namespace Netsphere.Server.Game.Services
{
    public partial class GameDataService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly DatabaseService _databaseService;
        private readonly string _resourcePath;

        public ImmutableDictionary<int, LevelInfo> Levels { get; private set; }
        public ImmutableArray<ChannelInfo> Channels { get; private set; }
        public ImmutableArray<MapInfo> Maps { get; private set; }
        public ImmutableDictionary<uint, ItemEffect> Effects { get; private set; }
        public ImmutableDictionary<ItemNumber, ItemInfo> Items { get; private set; }
        public ImmutableArray<DefaultItem> DefaultItems { get; private set; }
        public ImmutableDictionary<string, GameTempo> GameTempos { get; private set; }

        // Shop data
        public ImmutableDictionary<ItemNumber, ShopItem> ShopItems { get; private set; }
        public ImmutableDictionary<int, ShopEffectGroup> ShopEffects { get; private set; }
        public ImmutableDictionary<int, ShopPriceGroup> ShopPrices { get; private set; }
        public ImmutableDictionary<ItemLicense, LicenseReward> LicenseRewards { get; private set; }
        public ImmutableDictionary<int, LevelReward> LevelRewards { get; private set; }
        public string ShopVersion { get; private set; }

        public GameDataService(ILogger<GameDataService> logger, DatabaseService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;
            _resourcePath = Path.Combine(Program.BaseDirectory, "data");
        }

        public DefaultItem GetDefaultItem(CharacterGender gender, CostumeSlot slot, byte variation)
        {
            return DefaultItems.FirstOrDefault(item =>
                item.Gender == gender && item.Variation == variation &&
                item.ItemNumber.SubCategory == (byte)slot);
        }

        public ShopItem GetShopItem(ItemNumber itemNumber)
        {
            ShopItems.TryGetValue(itemNumber, out var item);
            return item;
        }

        public ShopItemInfo GetShopItemInfo(ItemNumber itemNumber, ItemPriceType priceType)
        {
            var item = GetShopItem(itemNumber);
            return item?.GetItemInfo(priceType);
        }

        public LevelInfo GetLevelFromExperience(uint experience)
        {
            return Levels.Values
                       .OrderBy(levelInfo => levelInfo.Level)
                       .FirstOrDefault(levelInfo => experience >= levelInfo.TotalExperience &&
                                                    experience < levelInfo.TotalExperience + levelInfo.ExperienceToNextLevel)
                   ?? Levels.Last().Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            LoadLevels();
            LoadChannels();
            LoadMaps();
            LoadEffects();
            LoadItems();
            LoadDefaultItems();
            LoadGameTempos();
            await LoadShop();
            await LoadLevelRewards();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private T Deserialize<T>(string fileName)
        {
            var serializer = new XmlSerializer(typeof(T));

            var path = Path.Combine(_resourcePath, fileName.Replace('/', Path.DirectorySeparatorChar));
            using (var r = new StreamReader(path))
                return (T)serializer.Deserialize(r);
        }

        private byte[] GetBytes(string fileName)
        {
            var path = Path.Combine(_resourcePath, fileName.Replace('/', Path.DirectorySeparatorChar));
            return File.Exists(path) ? File.ReadAllBytes(path) : null;
        }
    }
}
