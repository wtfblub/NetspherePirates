using Shaolinq;

namespace Netsphere.Database.Game
{
    [DataAccessModel]
    public abstract class GameDatabase : DataAccessModel
    {
        [DataAccessObjects]
        public abstract DataAccessObjects<LicenseRewardDto> LicenseRewards { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<PlayerDto> Players { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<PlayerCharacterDto> PlayerCharacters { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<PlayerDenyDto> PlayerDeny { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<PlayerItemDto> PlayerItems { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<PlayerLicenseDto> PlayerLicenses { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<PlayerMailDto> PlayerMails { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<PlayerSettingDto> PlayerSettings { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<ShopEffectGroupDto> ShopEffectGroups { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<ShopEffectDto> ShopEffects { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<ShopPriceGroupDto> ShopPriceGroups { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<ShopPriceDto> ShopPrices { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<ShopItemDto> ShopItems { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<ShopItemInfoDto> ShopItemInfos { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<ShopVersionDto> ShopVersion { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<StartItemDto> StartItems { get; }
    }
}