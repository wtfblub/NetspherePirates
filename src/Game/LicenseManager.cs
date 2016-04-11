using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Netsphere.Database.Game;
using Netsphere.Network;
using Netsphere.Network.Message.Game;
using NLog;
using NLog.Fluent;

namespace Netsphere
{
    internal class LicenseManager : IReadOnlyCollection<License>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
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
            _logger.Debug()
                .Account(_player)
                .Message("Acquiring license {0}", itemLicense)
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
                license.TimesCompleted++;
            }
            else
            {
                license = new License(itemLicense, DateTimeOffset.Now, 1);
                _licenses.TryAdd(itemLicense, license);
                _player.Session.Send(new SLicensedAckMessage(itemLicense, licenseReward?.ItemNumber ?? 0));

                _logger.Info()
                    .Account(_player)
                    .Message("Acquired license {0}", itemLicense)
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

        internal void Save()
        {
            License licenseToRemove;
            while (_licensesToRemove.TryPop(out licenseToRemove))
                GameDatabase.Instance.PlayerLicenses.GetReference(licenseToRemove.Id).Delete();

            foreach (var license in this)
            {
                PlayerLicenseDto licenseDto;
                if (!license.ExistsInDatabase)
                {
                    licenseDto = GameDatabase.Instance.Players
                        .GetReference((int)_player.Account.Id)
                        .Licenses.Create(license.Id);
                    license.ExistsInDatabase = true;

                    licenseDto.License = (byte)license.ItemLicense;
                    licenseDto.FirstCompletedDate = license.FirstCompletedDate.ToUnixTimeSeconds();
                }
                else
                {
                    if(!license.NeedsToSave)
                        continue;

                    license.NeedsToSave = false;
                    licenseDto = GameDatabase.Instance.PlayerLicenses
                        .GetReference(license.Id);
                }

                licenseDto.CompletedCount = license.TimesCompleted;
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