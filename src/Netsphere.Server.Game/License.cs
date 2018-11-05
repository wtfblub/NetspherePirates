using System;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;

namespace Netsphere.Server.Game
{
    public class License : DatabaseObject
    {
        private int _timesCompleted;

        public long Id { get; }
        public ItemLicense ItemLicense { get; }
        public DateTimeOffset FirstCompletedDate { get; }
        public int TimesCompleted
        {
            get => _timesCompleted;
            set => SetIfChanged(ref _timesCompleted, value);
        }

        internal License(PlayerLicenseEntity entity)
        {
            Id = entity.Id;
            ItemLicense = (ItemLicense)entity.License;
            FirstCompletedDate = DateTimeOffset.FromUnixTimeSeconds(entity.FirstCompletedDate);
            TimesCompleted = entity.CompletedCount;
            SetExistsState(true);
        }

        internal License(long id, ItemLicense license, DateTimeOffset firstCompletedDate, int timesCompleted)
        {
            Id = id;
            ItemLicense = license;
            FirstCompletedDate = firstCompletedDate;
            _timesCompleted = timesCompleted;
        }
    }
}
