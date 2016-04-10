using Platform.Validation;
using Shaolinq;

namespace Netsphere.Database.Game
{
    [DataAccessObject("license_rewards")]
    public abstract class LicenseRewardDto : DataAccessObject
    {
        [PrimaryKey]
        [PersistedMember]
        public virtual byte Id { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract ShopItemInfoDto ShopItemInfo { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract ShopPriceDto ShopPrice { get; set; }

        [PersistedMember]
        public abstract byte Color { get; set; }
    }

    [DataAccessObject("players")]
    public abstract class PlayerDto : DataAccessObject
    {
        [PrimaryKey]
        [PersistedMember]
        public virtual int Id { get; set; }

        [PersistedMember]
        public abstract byte TutorialState { get; set; }

        [PersistedMember]
        public abstract byte Level { get; set; }

        [PersistedMember]
        public abstract int TotalExperience { get; set; }

        [PersistedMember]
        public abstract int PEN { get; set; }

        [PersistedMember]
        public abstract int AP { get; set; }

        [PersistedMember]
        public abstract int Coins1 { get; set; }

        [PersistedMember]
        public abstract int Coins2 { get; set; }

        [PersistedMember]
        public abstract byte CurrentCharacterSlot { get; set; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<PlayerCharacterDto> Characters { get; }

        [RelatedDataAccessObjects(BackReferenceName = "Player")]
        public abstract RelatedDataAccessObjects<PlayerDenyDto> Ignores { get; }

        [RelatedDataAccessObjects(BackReferenceName = "DenyPlayer")]
        public abstract RelatedDataAccessObjects<PlayerDenyDto> IgnoredBy { get; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<PlayerItemDto> Items { get; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<PlayerLicenseDto> Licenses { get; }

        [RelatedDataAccessObjects(BackReferenceName = "Player")]
        public abstract RelatedDataAccessObjects<PlayerMailDto> Inbox { get; }

        [RelatedDataAccessObjects(BackReferenceName = "SenderPlayer")]
        public abstract RelatedDataAccessObjects<PlayerMailDto> Outbox { get; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<PlayerSettingDto> Settings { get; }
    }

    [DataAccessObject("player_characters")]
    public abstract class PlayerCharacterDto : DataAccessObject
    {
        [PrimaryKey]
        [PersistedMember]
        public virtual int Id { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract PlayerDto Player { get; set; }

        [PersistedMember]
        public abstract byte Slot { get; set; }

        [PersistedMember]
        public abstract byte Gender { get; set; }

        [PersistedMember]
        public abstract byte BasicHair { get; set; }

        [PersistedMember]
        public abstract byte BasicFace { get; set; }

        [PersistedMember]
        public abstract byte BasicShirt { get; set; }

        [PersistedMember]
        public abstract byte BasicPants { get; set; }

        [BackReference]
        public abstract PlayerItemDto Weapon1 { get; set; }

        [BackReference]
        public abstract PlayerItemDto Weapon2 { get; set; }

        [BackReference]
        public abstract PlayerItemDto Weapon3 { get; set; }

        [BackReference]
        public abstract PlayerItemDto Skill { get; set; }

        [BackReference]
        public abstract PlayerItemDto Hair { get; set; }

        [BackReference]
        public abstract PlayerItemDto Face { get; set; }

        [BackReference]
        public abstract PlayerItemDto Shirt { get; set; }

        [BackReference]
        public abstract PlayerItemDto Pants { get; set; }

        [BackReference]
        public abstract PlayerItemDto Gloves { get; set; }

        [BackReference]
        public abstract PlayerItemDto Shoes { get; set; }

        [BackReference]
        public abstract PlayerItemDto Accessory { get; set; }
    }

    [DataAccessObject("player_deny")]
    public abstract class PlayerDenyDto : DataAccessObject
    {
        [PrimaryKey]
        [PersistedMember]
        public virtual int Id { get; set; }

        [ValueRequired]
        [BackReference(Name = "Player")]
        public abstract PlayerDto Player { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract PlayerDto DenyPlayer { get; set; }
    }

    [DataAccessObject("player_items")]
    public abstract class PlayerItemDto : DataAccessObject
    {
        [PrimaryKey]
        [PersistedMember]
        public virtual int Id { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract PlayerDto Player { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract ShopItemInfoDto ShopItemInfo { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract ShopPriceDto ShopPrice { get; set; }

        [PersistedMember]
        public abstract uint Effect { get; set; }

        [PersistedMember]
        public abstract byte Color { get; set; }

        [PersistedMember]
        public abstract long PurchaseDate { get; set; }

        [PersistedMember]
        public abstract int Durability { get; set; }

        [PersistedMember]
        public abstract int Count { get; set; }

        [RelatedDataAccessObjects(BackReferenceName = nameof(PlayerCharacterDto.Weapon1))]
        public abstract RelatedDataAccessObjects<PlayerCharacterDto> Weapon1 { get; }

        [RelatedDataAccessObjects(BackReferenceName = nameof(PlayerCharacterDto.Weapon2))]
        public abstract RelatedDataAccessObjects<PlayerCharacterDto> Weapon2 { get; }

        [RelatedDataAccessObjects(BackReferenceName = nameof(PlayerCharacterDto.Weapon3))]
        public abstract RelatedDataAccessObjects<PlayerCharacterDto> Weapon3 { get; }

        [RelatedDataAccessObjects(BackReferenceName = nameof(PlayerCharacterDto.Skill))]
        public abstract RelatedDataAccessObjects<PlayerCharacterDto> Skill { get; }

        [RelatedDataAccessObjects(BackReferenceName = nameof(PlayerCharacterDto.Hair))]
        public abstract RelatedDataAccessObjects<PlayerCharacterDto> Hair { get; }

        [RelatedDataAccessObjects(BackReferenceName = nameof(PlayerCharacterDto.Face))]
        public abstract RelatedDataAccessObjects<PlayerCharacterDto> Face { get; }

        [RelatedDataAccessObjects(BackReferenceName = nameof(PlayerCharacterDto.Shirt))]
        public abstract RelatedDataAccessObjects<PlayerCharacterDto> Shirt { get; }

        [RelatedDataAccessObjects(BackReferenceName = nameof(PlayerCharacterDto.Pants))]
        public abstract RelatedDataAccessObjects<PlayerCharacterDto> Pants { get; }

        [RelatedDataAccessObjects(BackReferenceName = nameof(PlayerCharacterDto.Gloves))]
        public abstract RelatedDataAccessObjects<PlayerCharacterDto> Gloves { get; }

        [RelatedDataAccessObjects(BackReferenceName = nameof(PlayerCharacterDto.Shoes))]
        public abstract RelatedDataAccessObjects<PlayerCharacterDto> Shoes { get; }

        [RelatedDataAccessObjects(BackReferenceName = nameof(PlayerCharacterDto.Accessory))]
        public abstract RelatedDataAccessObjects<PlayerCharacterDto> Accessory { get; }
    }

    [DataAccessObject("player_licenses")]
    public abstract class PlayerLicenseDto : DataAccessObject
    {
        [PrimaryKey]
        [PersistedMember]
        public virtual int Id { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract PlayerDto Player { get; set; }

        [PersistedMember]
        public abstract byte License { get; set; }

        [PersistedMember]
        public abstract long FirstCompletedDate { get; set; }

        [PersistedMember]
        public abstract int CompletedCount { get; set; }
    }

    [DataAccessObject("player_mails")]
    public abstract class PlayerMailDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract PlayerDto Player { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract PlayerDto SenderPlayer { get; set; }

        [PersistedMember]
        public abstract long SentDate { get; set; }

        [ValueRequired]
        [SizeConstraint(MaximumLength = 100, SizeFlexibility = SizeFlexibility.Variable)]
        [PersistedMember]
        public abstract string Title { get; set; }

        [ValueRequired]
        [PersistedMember]
        [SizeConstraint(MaximumLength = 500, SizeFlexibility = SizeFlexibility.Variable)]
        public abstract string Message { get; set; }

        [PersistedMember]
        public abstract bool IsMailNew { get; set; }

        [PersistedMember]
        public abstract bool IsMailDeleted { get; set; }
    }

    [DataAccessObject("player_settings")]
    public abstract class PlayerSettingDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract PlayerDto Player { get; set; }

        [ValueRequired]
        [SizeConstraint(MaximumLength = 512, SizeFlexibility = SizeFlexibility.Variable)]
        [PersistedMember]
        public abstract string Setting { get; set; }

        [ValueRequired]
        [SizeConstraint(MaximumLength = 512, SizeFlexibility = SizeFlexibility.Variable)]
        [PersistedMember]
        public abstract string Value { get; set; }
    }

    [DataAccessObject("shop_effect_groups")]
    public abstract class ShopEffectGroupDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [ValueRequired]
        [SizeConstraint(MaximumLength = 20, SizeFlexibility = SizeFlexibility.Variable)]
        [PersistedMember]
        public abstract string Name { get; set; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<ShopEffectDto> ShopEffects { get; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<ShopItemInfoDto> ShopItemInfos { get; }
    }

    [DataAccessObject("shop_effects")]
    public abstract class ShopEffectDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract ShopEffectGroupDto EffectGroup { get; set; }

        [PersistedMember]
        public abstract uint Effect { get; set; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<StartItemDto> StartItems { get; }
    }

    [DataAccessObject("shop_price_groups")]
    public abstract class ShopPriceGroupDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [ValueRequired]
        [SizeConstraint(MaximumLength = 20, SizeFlexibility = SizeFlexibility.Variable)]
        [PersistedMember]
        public abstract string Name { get; set; }

        [ValueRequired]
        [PersistedMember]
        public abstract byte PriceType { get; set; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<ShopPriceDto> ShopPrices { get; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<ShopItemInfoDto> ShopItemInfos { get; }
    }

    [DataAccessObject("shop_prices")]
    public abstract class ShopPriceDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract ShopPriceGroupDto PriceGroup { get; set; }

        [PersistedMember]
        public abstract byte PeriodType { get; set; }

        [PersistedMember]
        public abstract int Period { get; set; }

        [PersistedMember]
        public abstract int Price { get; set; }

        [PersistedMember]
        public abstract bool IsRefundable { get; set; }

        [PersistedMember]
        public abstract int Durability { get; set; }

        [PersistedMember]
        public abstract bool IsEnabled { get; set; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<LicenseRewardDto> LicenseRewards { get; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<PlayerItemDto> PlayerItems { get; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<StartItemDto> StartItems { get; }
    }

    [DataAccessObject("shop_items")]
    public abstract class ShopItemDto : DataAccessObject
    {
        [PrimaryKey]
        [PersistedMember]
        public virtual uint Id { get; set; }

        [PersistedMember]
        public abstract byte RequiredGender { get; set; }

        [PersistedMember]
        public abstract byte RequiredLicense { get; set; }

        [PersistedMember]
        public abstract byte Colors { get; set; }

        [PersistedMember]
        public abstract byte UniqueColors { get; set; }

        [PersistedMember]
        public abstract byte RequiredLevel { get; set; }

        [PersistedMember]
        public abstract byte LevelLimit { get; set; }

        [PersistedMember]
        public abstract byte RequiredMasterLevel { get; set; }

        [PersistedMember]
        public abstract bool IsOneTimeUse { get; set; }

        [PersistedMember]
        public abstract bool IsDestroyable { get; set; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<ShopItemInfoDto> ItemInfos { get; }
    }

    [DataAccessObject("shop_iteminfos")]
    public abstract class ShopItemInfoDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract ShopItemDto ShopItem { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract ShopPriceGroupDto PriceGroup { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract ShopEffectGroupDto EffectGroup { get; set; }

        [PersistedMember]
        public abstract byte DiscountPercentage { get; set; }

        [PersistedMember]
        public abstract bool IsEnabled { get; set; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<LicenseRewardDto> LicenseRewards { get; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<PlayerItemDto> PlayerItems { get; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<StartItemDto> StartItems { get; }
    }

    [DataAccessObject("shop_version")]
    public abstract class ShopVersionDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual byte Id { get; set; }

        [ValueRequired]
        [SizeConstraint(MaximumLength = 40, MinimumLength = 0, SizeFlexibility = SizeFlexibility.Variable)]
        [PersistedMember]
        public abstract string Version { get; set; }
    }

    [DataAccessObject("start_items")]
    public abstract class StartItemDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract ShopItemInfoDto ShopItemInfo { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract ShopPriceDto ShopPrice { get; set; }

        [ValueRequired]
        [BackReference]
        public abstract ShopEffectDto ShopEffect { get; set; }

        [PersistedMember]
        public abstract byte Color { get; set; }

        [PersistedMember]
        public abstract int Count { get; set; }

        [PersistedMember]
        public abstract byte RequiredSecurityLevel { get; set; }
    }
}
