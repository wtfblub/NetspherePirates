using LinqToDB;
using LinqToDB.Data;
using Netsphere.Database.Game;

namespace Netsphere.Database
{
    public class GameContext : DataConnection
    {
        public ITable<PlayerEntity> Players => GetTable<PlayerEntity>();
        public ITable<PlayerCharacterEntity> PlayerCharacters => GetTable<PlayerCharacterEntity>();
        public ITable<PlayerDenyEntity> PlayerIgnores => GetTable<PlayerDenyEntity>();
        public ITable<PlayerItemEntity> PlayerItems => GetTable<PlayerItemEntity>();
        public ITable<PlayerLicenseEntity> PlayerLicenses => GetTable<PlayerLicenseEntity>();
        public ITable<PlayerMailEntity> PlayerMails => GetTable<PlayerMailEntity>();
        public ITable<PlayerSettingEntity> PlayerSettings => GetTable<PlayerSettingEntity>();
        public ITable<ShopEffectGroupEntity> EffectGroups => GetTable<ShopEffectGroupEntity>();
        public ITable<ShopEffectEntity> Effects => GetTable<ShopEffectEntity>();
        public ITable<ShopPriceGroupEntity> PriceGroups => GetTable<ShopPriceGroupEntity>();
        public ITable<ShopPriceEntity> Prices => GetTable<ShopPriceEntity>();
        public ITable<ShopItemEntity> Items => GetTable<ShopItemEntity>();
        public ITable<ShopItemInfoEntity> ItemInfos => GetTable<ShopItemInfoEntity>();
        public ITable<ShopVersionEntity> ShopVersion => GetTable<ShopVersionEntity>();
        public ITable<StartItemEntity> StartItems => GetTable<StartItemEntity>();
        public ITable<LicenseRewardEntity> LicenseRewards => GetTable<LicenseRewardEntity>();

        public GameContext(string provider, string connection)
            : base(provider, connection)
        {
        }
    }
}
