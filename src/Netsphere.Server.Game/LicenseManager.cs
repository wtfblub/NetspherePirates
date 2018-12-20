using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.Collections.Concurrent;
using LinqToDB;
using Logging;
using Netsphere.Common;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game
{
    public class LicenseManager : ISaveable, IReadOnlyCollection<License>
    {
        private ILogger _logger;
        private readonly IdGeneratorService _idGeneratorService;
        private readonly GameDataService _gameDataService;
        private readonly ConcurrentDictionary<ItemLicense, License> _licenses;
        private readonly ConcurrentStack<License> _licensesToRemove;
        // ReSharper disable once NotAccessedField.Local
        private Player _player;

        public int Count => _licenses.Count;

        /// <summary>
        /// Returns the license or null if the license does not exist
        /// </summary>
        public License this[ItemLicense license] => GetLicense(license);

        public LicenseManager(ILogger<LicenseManager> logger, IdGeneratorService idGeneratorService,
            GameDataService gameDataService)
        {
            _logger = logger;
            _idGeneratorService = idGeneratorService;
            _gameDataService = gameDataService;
            _licenses = new ConcurrentDictionary<ItemLicense, License>();
            _licensesToRemove = new ConcurrentStack<License>();
        }

        internal void Initialize(Player plr, IEnumerable<PlayerLicenseEntity> entities)
        {
            _logger = plr.AddContextToLogger(_logger);
            _player = plr;
            foreach (var license in entities.Select(x => new License(x)))
                _licenses.TryAdd(license.ItemLicense, license);
        }

        /// <summary>
        /// Returns the license or null if the license does not exist
        /// </summary>
        public License GetLicense(ItemLicense license)
        {
            return CollectionExtensions.GetValueOrDefault(_licenses, license);
        }

        public License Acquire(ItemLicense itemLicense)
        {
            _logger.Information("Acquiring {License}", itemLicense);

            var licenseReward = _gameDataService.LicenseRewards.GetValueOrDefault(itemLicense);

            // TODO Should we require license rewards or no?
            // If no we need some other way to determine if a license is available to be acquired or no

            //if (!shop.Licenses.TryGetValue(license, out licenseInfo))
            //throw new LicenseNotFoundException($"License {license} does not exist");

            var license = this[itemLicense];

            // If this is the first time completing this license
            // give the player the item reward
            if (license == null && licenseReward != null)
            {
                _player.Inventory.Create(licenseReward.ShopItemInfo, licenseReward.ShopPrice, licenseReward.Color, 0, 0);
                _player.Session.Send(new SLicensedAckMessage(itemLicense, licenseReward.ItemNumber));
            }

            if (license != null)
            {
                ++license.TimesCompleted;
            }
            else
            {
                license = new License(_idGeneratorService.GetNextId(IdKind.License), itemLicense, DateTimeOffset.Now, 1);
                _licenses.TryAdd(itemLicense, license);
                // _player.Session.SendAsync(new SLicensedAckMessage(itemLicense, licenseReward?.ItemNumber ?? 0));

                _logger.Information("Acquired {License}", itemLicense);
            }

            return license;
        }

        /// <summary>
        /// Removes the license
        /// </summary>
        /// <returns>true if the license was removed and false if the license does not exist</returns>
        public bool Remove(License license)
        {
            return Remove(license.ItemLicense);
        }

        /// <summary>
        /// Removes the license
        /// </summary>
        /// <returns>true if the license was removed and false if the license does not exist</returns>
        public bool Remove(ItemLicense itemLicense)
        {
            var license = this[itemLicense];
            if (license == null)
                return false;

            _licenses.Remove(itemLicense);
            if (license.Exists)
                _licensesToRemove.Push(license);

            return true;
        }

        public async Task Save(GameContext db)
        {
            if (!_licensesToRemove.IsEmpty)
            {
                var idsToRemove = new List<long>();
                while (_licensesToRemove.TryPop(out var licenseToRemove))
                    idsToRemove.Add(licenseToRemove.Id);

                await db.LicenseRewards.Where(x => idsToRemove.Contains(x.Id)).DeleteAsync();
            }

            foreach (var license in this)
            {
                if (!license.Exists)
                {
                    await db.InsertAsync(new PlayerLicenseEntity
                    {
                        Id = license.Id,
                        PlayerId = (int)_player.Account.Id,
                        License = (byte)license.ItemLicense,
                        FirstCompletedDate = license.FirstCompletedDate.ToUnixTimeSeconds(),
                        CompletedCount = license.TimesCompleted
                    });

                    license.SetExistsState(true);
                }
                else
                {
                    if (!license.IsDirty)
                        continue;

                    await db.PlayerLicenses
                        .Where(x => x.Id == license.Id)
                        .Set(x => x.CompletedCount, license.TimesCompleted)
                        .UpdateAsync();

                    license.SetDirtyState(false);
                }
            }
        }

        public bool Contains(ItemLicense license)
        {
            return _licenses.ContainsKey(license);
        }

        public IEnumerator<License> GetEnumerator()
        {
            return _licenses.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
