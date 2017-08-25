using System;
using Netsphere.Database.Game;

namespace Netsphere
{
    internal class License
    {
        private int _timesCompleted;
        internal bool ExistsInDatabase { get; set; }
        internal bool NeedsToSave { get; set; }

        public int Id { get; }
        public ItemLicense ItemLicense { get; }
        public DateTimeOffset FirstCompletedDate { get; }
        public int TimesCompleted
        {
            get => _timesCompleted;
            set
            {
                if (_timesCompleted == value)
                    return;
                _timesCompleted = value;
                NeedsToSave = true;
            }
        }

        internal License(PlayerLicenseDto dto)
        {
            ExistsInDatabase = true;
            Id = dto.Id;
            ItemLicense = (ItemLicense)dto.License;
            FirstCompletedDate = DateTimeOffset.FromUnixTimeSeconds(dto.FirstCompletedDate);
            _timesCompleted = dto.CompletedCount;
        }

        internal License(ItemLicense license, DateTimeOffset firstCompletedDate, int timesCompleted)
        {
            Id = LicenseIdGenerator.GetNextId();
            ItemLicense = license;
            FirstCompletedDate = firstCompletedDate;
            _timesCompleted = timesCompleted;
        }
    }
}
