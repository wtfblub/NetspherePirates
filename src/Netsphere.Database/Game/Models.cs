using Shaolinq;

namespace Netsphere.Database.Game
{
    [DataAccessObject("license_rewards")]
    public abstract class LicenseRewardDto : DataAccessObject
    {
        [PrimaryKey]
        [PersistedMember]
        public virtual byte Id { get; set; }

        [BackReference]
        public abstract ShopItemInfoDto ShopItemInfo { get; set; }

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

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<PlayerItemDto> Items { get; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<PlayerLicenseDto> Licenses { get; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<PlayerMailDto> Mails { get; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<PlayerSettingDto> Settings { get; }
    }

    [DataAccessObject("player_characters")]
    public abstract class PlayerCharacterDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

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
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [BackReference(Name = "Player")]
        public abstract PlayerDto Player { get; set; }

        [BackReference]
        public abstract PlayerDto DenyPlayer { get; set; }
    }

    [DataAccessObject("player_items")]
    public abstract class PlayerItemDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [BackReference]
        public abstract PlayerDto Player { get; set; }

        [BackReference]
        public abstract ShopItemInfoDto ShopItemInfo { get; set; }

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
    }

    [DataAccessObject("player_licenses")]
    public abstract class PlayerLicenseDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

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

        [BackReference]
        public abstract PlayerDto Player { get; set; }

        [BackReference]
        public abstract PlayerDto SenderPlayer { get; set; }

        [PersistedMember]
        public abstract long SentDate { get; set; }

        [PersistedMember]
        public abstract string Title { get; set; }

        [PersistedMember]
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

        [BackReference]
        public abstract PlayerDto Player { get; set; }

        [PersistedMember]
        public abstract byte Setting { get; set; }

        [PersistedMember]
        public abstract byte Value { get; set; }
    }

    [DataAccessObject("shop_effect_groups")]
    public abstract class ShopEffectGroupDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [PersistedMember]
        public abstract string Name { get; set; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<ShopEffectDto> Effects { get; }
    }

    [DataAccessObject("shop_effects")]
    public abstract class ShopEffectDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [BackReference]
        public abstract ShopEffectGroupDto EffectGroup { get; set; }

        [PersistedMember]
        public abstract uint Effect { get; set; }
    }

    [DataAccessObject("shop_price_groups")]
    public abstract class ShopPriceGroupDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [PersistedMember]
        public abstract string Name { get; set; }

        [PersistedMember]
        public abstract byte PriceType { get; set; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<ShopPriceDto> Prices { get; }
    }

    [DataAccessObject("shop_prices")]
    public abstract class ShopPriceDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

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
    }

    [DataAccessObject("shop_iteminfos")]
    public abstract class ShopItemInfoDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [BackReference]
        public abstract ShopItemDto Item { get; set; }

        [BackReference]
        public abstract ShopPriceGroupDto PriceGroup { get; set; }

        [BackReference]
        public abstract ShopEffectGroupDto EffectGroup { get; set; }

        [PersistedMember]
        public abstract byte DiscountPercentage { get; set; }

        [PersistedMember]
        public abstract bool IsEnabled { get; set; }
    }

    [DataAccessObject("shop_version")]
    public abstract class ShopVersionDto : DataAccessObject
    {
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

        [BackReference]
        public abstract ShopItemInfoDto ShopItemInfo { get; set; }

        [PersistedMember]
        public abstract byte Color { get; set; }

        [PersistedMember]
        public abstract int Count { get; set; }

        [PersistedMember]
        public abstract byte RequiredSecurityLevel { get; set; }
    }
}
