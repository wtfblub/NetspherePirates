using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlubLib.Caching;
using NLog;
using NLog.Fluent;

namespace Netsphere.Resource
{
    internal class ResourceCache
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ResourceLoader _loader;
        private readonly ICache _cache = new MemoryCache();

        public ResourceCache()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path = Path.Combine(path, "data");
            _loader = new ResourceLoader(path);
        }

        public void PreCache()
        {
            _logger.Info()
                .Message("Caching: Channels")
                .Write();
            GetChannels();

            _logger.Info()
                .Message("Caching: Effects")
                .Write();
            GetEffects();

            _logger.Info()
                .Message("Caching: Items")
                .Write();
            GetItems();

            _logger.Info()
                .Message("Caching: DefaultItems")
                .Write();
            GetDefaultItems();

            _logger.Info()
                .Message("Caching: Shop")
                .Write();
            GetShop();

            _logger.Info()
                .Message("Caching: Experience")
                .Write();
            GetExperience();

            _logger.Info()
                .Message("Caching: Maps")
                .Write();
            GetMaps();

            _logger.Info()
                .Message("Caching: GameTempos")
                .Write();
            GetGameTempos();
        }

        public IReadOnlyList<ChannelInfo> GetChannels()
        {
            var value = _cache.Get<IReadOnlyList<ChannelInfo>>(ResourceCacheType.Channels);
            if (value == null)
            {
                _logger.Debug()
                    .Message("Caching...")
                    .Write();

                value = _loader.LoadChannels().ToList();
                _cache.AddOrUpdate(ResourceCacheType.Channels, value);
            }
            return value;
        }

        public IReadOnlyDictionary<uint, ItemEffect> GetEffects()
        {
            var value = _cache.Get<IReadOnlyDictionary<uint, ItemEffect>>(ResourceCacheType.Effects);
            if (value == null)
            {
                _logger.Debug()
                    .Message("Caching...")
                    .Write();

                value = _loader.LoadEffects().ToDictionary(effect => effect.Id);
                _cache.AddOrUpdate(ResourceCacheType.Effects, value);
            }

            return value;
        }

        public IReadOnlyDictionary<ItemNumber, ItemInfo> GetItems()
        {
            var value = _cache.Get<IReadOnlyDictionary<ItemNumber, ItemInfo>>(ResourceCacheType.Items);
            if (value == null)
            {
                _logger.Debug()
                    .Message("Caching...")
                    .Write();

                value = _loader.LoadItems().ToDictionary(item => item.ItemNumber);
                _cache.AddOrUpdate(ResourceCacheType.Items, value);
            }

            return value;
        }

        public IReadOnlyList<DefaultItem> GetDefaultItems()
        {
            var value = _cache.Get<IReadOnlyList<DefaultItem>>(ResourceCacheType.DefaultItems);
            if (value == null)
            {
                _logger.Debug()
                    .Message("Caching...")
                    .Write();

                value = _loader.LoadDefaultItems().ToList();
                _cache.AddOrUpdate(ResourceCacheType.DefaultItems, value);
            }

            return value;
        }

        public ShopResources GetShop()
        {
            var value = _cache.Get<ShopResources>(ResourceCacheType.Shop);
            if (value == null)
            {
                _logger.Debug()
                    .Message("Caching...")
                    .Write();

                value = new ShopResources();
                _cache.AddOrUpdate(ResourceCacheType.Shop, value);
            }
            if (string.IsNullOrWhiteSpace(value.Version))
                value.Load();

            return value;
        }

        public IReadOnlyDictionary<int, Experience> GetExperience()
        {
            var value = _cache.Get<IReadOnlyDictionary<int, Experience>>(ResourceCacheType.Exp);
            if (value == null)
            {
                _logger.Debug()
                    .Message("Caching...")
                    .Write();

                value = _loader.LoadExperience().ToDictionary(e => e.Level);
                _cache.AddOrUpdate(ResourceCacheType.Exp, value);
            }

            return value;
        }

        public IReadOnlyDictionary<byte, MapInfo> GetMaps()
        {
            var value = _cache.Get<IReadOnlyDictionary<byte, MapInfo>>(ResourceCacheType.Maps);
            if (value == null)
            {
                _logger.Debug()
                    .Message("Caching...")
                    .Write();

                value = _loader.LoadMaps().ToDictionary(maps => maps.Id);
                _cache.AddOrUpdate(ResourceCacheType.Maps, value);
            }

            return value;
        }

        public IReadOnlyDictionary<string, GameTempo> GetGameTempos()
        {
            var value = _cache.Get<IReadOnlyDictionary<string, GameTempo>>(ResourceCacheType.GameTempo);
            if (value == null)
            {
                _logger.Debug()
                    .Message("Caching...")
                    .Write();

                value = _loader.LoadGameTempos().ToDictionary(t => t.Name);
                _cache.AddOrUpdate(ResourceCacheType.GameTempo, value);
            }

            return value;
        }

        public void Clear()
        {
            _logger.Debug()
                .Message("Clearing cache")
                .Write();
            _cache.Clear();
        }

        public void Clear(ResourceCacheType type)
        {
            _logger.Debug()
                .Message("Clearing cache for {0}", type)
                .Write();

            if (type == ResourceCacheType.Shop)
            {
                GetShop().Clear();
                return;
            }
            _cache.Remove(type.ToString());
        }
    }

    internal static class ResourceCacheExtensions
    {
        public static T Get<T>(this ICache cache, ResourceCacheType type)
            where T : class
        {
            return cache.Get<T>(type.ToString());
        }

        public static void AddOrUpdate(this ICache cache, ResourceCacheType type, object value)
        {
            cache.AddOrUpdate(type.ToString(), value);
        }

        public static void AddOrUpdate(this ICache cache, ResourceCacheType type, object value, TimeSpan ts)
        {
            cache.AddOrUpdate(type.ToString(), value, ts);
        }

        public static void AddOrUpdate(this ICache cache, ResourceCacheType type, object value, long ttl)
        {
            cache.AddOrUpdate(type.ToString(), value, ttl);
        }
    }
}