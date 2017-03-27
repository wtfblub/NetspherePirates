using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.FastCrud;
using Netsphere.Database.Game;
using Netsphere.Network;
using Netsphere.Network.Message.Game;
using NLog;
using NLog.Fluent;

namespace Netsphere
{
    internal class LicenseManager : IReadOnlyCollection<License>
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Player _player;
        private readonly ConcurrentDictionary<ItemLicense, License> _licenses = new ConcurrentDictionary<ItemLicense, License>();
        private readonly ConcurrentStack<License> _licensesToRemove = new ConcurrentStack<License>();

        public int Count => _licenses.Count;

        /// <summary>
        /// Returns the license or null if the license does not exist
        /// </summary>
        public License this[ItemLicense license] => GetLicense(license);

        internal LicenseManager(Player plr, PlayerDto dto)
        {
            _player = plr;
            foreach (var license in dto.Licenses.Select(l => new License(l)))
                _licenses.TryAdd(license.ItemLicense, license);
        }

        /// <summary>
        /// Returns the license or null if the license does not exist
        /// </summary>
        public License GetLicense(ItemLicense license)
        {
            return _licenses.GetValueOrDefault(license);
        }

        public License Acquire(ItemLicense itemLicense)
        {
            Logger.Debug()
                .Account(_player)
                .Message($"Acquiring license {itemLicense}")
                .Write();

            var shop = GameServer.Instance.ResourceCache.GetShop();
            var licenseReward = shop.Licenses.GetValueOrDefault(itemLicense);

            // ToDo Should we require license rewards or no?
            // If no we need some other way to determine if a license is available to be acquired or no
            //if (!shop.Licenses.TryGetValue(license, out licenseInfo))
            //throw new LicenseNotFoundException($"License {license} does not exist");

            var license = this[itemLicense];

            // If this is the first time completing this license
            // give the player the item reward [TEMP: if available]
            if (license == null && licenseReward != null)
                _player.Inventory.Create(licenseReward.ShopItemInfo, licenseReward.ShopPrice, licenseReward.Color, 0, 0);

            if (license != null)
            {
                ++license.TimesCompleted;
            }
            else
            {
                license = new License(itemLicense, DateTimeOffset.Now, 1);
                _licenses.TryAdd(itemLicense, license);
                _player.Session.SendAsync(new SLicensedAckMessage(itemLicense, licenseReward?.ItemNumber ?? 0));

                Logger.Info()
                    .Account(_player)
                    .Message($"Acquired license {itemLicense}")
                    .Write();
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
            if (license.ExistsInDatabase)
                _licensesToRemove.Push(license);
            return true;
        }

        internal void Save(IDbConnection db)
        {
            if (!_licensesToRemove.IsEmpty)
            {
                var idsToRemove = new StringBuilder();
                var firstRun = true;
                License licenseToRemove;
                while (_licensesToRemove.TryPop(out licenseToRemove))
                {
                    if (firstRun)
                        firstRun = false;
                    else
                        idsToRemove.Append(',');
                    idsToRemove.Append(licenseToRemove.Id);
                }

                db.BulkDelete<PlayerLicenseDto>(statement => statement
                    .Where($"{nameof(PlayerLicenseDto.Id):C} IN ({idsToRemove})"));
            }

            foreach (var license in this)
            {
                if (!license.ExistsInDatabase)
                {
                    db.Insert(new PlayerLicenseDto
                    {
                        Id = license.Id,
                        PlayerId = (int)_player.Account.Id,
                        License = (byte)license.ItemLicense,
                        FirstCompletedDate = license.FirstCompletedDate.ToUnixTimeSeconds(),
                        CompletedCount = license.TimesCompleted
                    });
                    license.ExistsInDatabase = true;
                }
                else
                {
                    if (!license.NeedsToSave)
                        continue;

                    var mapping = OrmConfiguration
                        .GetDefaultEntityMapping<PlayerLicenseDto>()
                        .Clone()
                        .UpdatePropertiesExcluding(prop => prop.IsExcludedFromUpdates = true,
                            nameof(PlayerLicenseDto.CompletedCount));
                    db.Update(new PlayerLicenseDto
                    {
                        CompletedCount = license.TimesCompleted
                    }, statement => statement.WithEntityMappingOverride(mapping));
                    license.NeedsToSave = false;
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