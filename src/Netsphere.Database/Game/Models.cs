using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("license_rewards")]
    public class LicenseRewardDto
    {
        [Key]
        public byte Id { get; set; }

        [ForeignKey(nameof(ShopItemInfo))]
        public int ShopItemInfoId { get; set; }
        public ShopItemInfoDto ShopItemInfo { get; set; }

        [ForeignKey(nameof(ShopPrice))]
        public int ShopPriceId { get; set; }
        public ShopPriceDto ShopPrice { get; set; }

        public byte Color { get; set; }
    }

    [Table("players")]
    public class PlayerDto
    {
        [Key]
        public int Id { get; set; }
        public byte TutorialState { get; set; }
        public byte Level { get; set; }
        public int TotalExperience { get; set; }
        public int PEN { get; set; }
        public int AP { get; set; }
        public int Coins1 { get; set; }
        public int Coins2 { get; set; }
        public byte CurrentCharacterSlot { get; set; }

        public IEnumerable<PlayerCharacterDto> Characters { get; }
        public IEnumerable<PlayerDenyDto> Ignores { get; }
        public IEnumerable<PlayerDenyDto> IgnoredBy { get; }
        public IEnumerable<PlayerItemDto> Items { get; }
        public IEnumerable<PlayerLicenseDto> Licenses { get; }
        public IEnumerable<PlayerMailDto> Inbox { get; }
        public IEnumerable<PlayerMailDto> Outbox { get; }
        public IEnumerable<PlayerSettingDto> Settings { get; }
    }

    [Table("player_characters")]
    public class PlayerCharacterDto
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Player))]
        public int PlayerId { get; set; }
        public PlayerDto Player { get; set; }

        public byte Slot { get; set; }
        public byte Gender { get; set; }
        public byte BasicHair { get; set; }
        public byte BasicFace { get; set; }
        public byte BasicShirt { get; set; }
        public byte BasicPants { get; set; }

        [ForeignKey(nameof(Weapon1))]
        public int Weapon1Id { get; set; }
        public PlayerItemDto Weapon1 { get; set; }

        [ForeignKey(nameof(Weapon2))]
        public int Weapon2Id { get; set; }
        public PlayerItemDto Weapon2 { get; set; }

        [ForeignKey(nameof(Weapon3))]
        public int Weapon3Id { get; set; }
        public PlayerItemDto Weapon3 { get; set; }

        [ForeignKey(nameof(Skill))]
        public int SkillId { get; set; }
        public PlayerItemDto Skill { get; set; }

        [ForeignKey(nameof(Hair))]
        public int HairId { get; set; }
        public PlayerItemDto Hair { get; set; }

        [ForeignKey(nameof(Face))]
        public int FaceId { get; set; }
        public PlayerItemDto Face { get; set; }

        [ForeignKey(nameof(Shirt))]
        public int ShirtId { get; set; }
        public PlayerItemDto Shirt { get; set; }

        [ForeignKey(nameof(Pants))]
        public int PantsId { get; set; }
        public PlayerItemDto Pants { get; set; }

        [ForeignKey(nameof(Gloves))]
        public int GlovesId { get; set; }
        public PlayerItemDto Gloves { get; set; }

        [ForeignKey(nameof(Shoes))]
        public int ShoesId { get; set; }
        public PlayerItemDto Shoes { get; set; }

        [ForeignKey(nameof(Accessory))]
        public int AccessoryId { get; set; }
        public PlayerItemDto Accessory { get; set; }
    }

    [Table("player_deny")]
    public class PlayerDenyDto
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Player))]
        public int PlayerId { get; set; }
        public PlayerDto Player { get; set; }

        [ForeignKey(nameof(DenyPlayer))]
        public int DenyPlayerId { get; set; }
        public PlayerDto DenyPlayer { get; set; }
    }

    [Table("player_items")]
    public class PlayerItemDto
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Player))]
        public int PlayerId { get; set; }
        public PlayerDto Player { get; set; }

        [ForeignKey(nameof(ShopItemInfo))]
        public int ShopItemInfoId { get; set; }
        public ShopItemInfoDto ShopItemInfo { get; set; }

        [ForeignKey(nameof(ShopPrice))]
        public int ShopPriceId { get; set; }
        public ShopPriceDto ShopPrice { get; set; }

        public uint Effect { get; set; }
        public byte Color { get; set; }
        public long PurchaseDate { get; set; }
        public int Durability { get; set; }
        public int Count { get; set; }


        public IEnumerable<PlayerCharacterDto> Weapon1 { get; }
        public IEnumerable<PlayerCharacterDto> Weapon2 { get; }
        public IEnumerable<PlayerCharacterDto> Weapon3 { get; }
        public IEnumerable<PlayerCharacterDto> Skill { get; }
        public IEnumerable<PlayerCharacterDto> Hair { get; }
        public IEnumerable<PlayerCharacterDto> Face { get; }
        public IEnumerable<PlayerCharacterDto> Shirt { get; }
        public IEnumerable<PlayerCharacterDto> Pants { get; }
        public IEnumerable<PlayerCharacterDto> Gloves { get; }
        public IEnumerable<PlayerCharacterDto> Shoes { get; }
        public IEnumerable<PlayerCharacterDto> Accessory { get; }
    }

    [Table("player_licenses")]
    public class PlayerLicenseDto
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Player))]
        public int PlayerId { get; set; }
        public PlayerDto Player { get; set; }

        public byte License { get; set; }
        public long FirstCompletedDate { get; set; }
        public int CompletedCount { get; set; }
    }

    [Table("player_mails")]
    public class PlayerMailDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Player))]
        public int PlayerId { get; set; }
        public PlayerDto Player { get; set; }

        [ForeignKey(nameof(SenderPlayer))]
        public int SenderPlayerId { get; set; }
        public PlayerDto SenderPlayer { get; set; }

        public long SentDate { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsMailNew { get; set; }
        public bool IsMailDeleted { get; set; }
    }

    [Table("player_settings")]
    public class PlayerSettingDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Player))]
        public int PlayerId { get; set; }
        public PlayerDto Player { get; set; }

        public string Setting { get; set; }
        public string Value { get; set; }
    }

    [Table("shop_effect_groups")]
    public class ShopEffectGroupDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<ShopEffectDto> ShopEffects { get; }
        public IEnumerable<ShopItemInfoDto> ShopItemInfos { get; }
    }

    [Table("shop_effects")]
    public class ShopEffectDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(EffectGroup))]
        public int EffectGroupId { get; set; }
        public ShopEffectGroupDto EffectGroup { get; set; }

        public uint Effect { get; set; }

        public IEnumerable<StartItemDto> StartItems { get; }
    }

    [Table("shop_price_groups")]
    public class ShopPriceGroupDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public byte PriceType { get; set; }

        public IEnumerable<ShopPriceDto> ShopPrices { get; }
        public IEnumerable<ShopItemInfoDto> ShopItemInfos { get; }
    }

    [Table("shop_prices")]
    public class ShopPriceDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(PriceGroup))]
        public int PriceGroupId { get; set; }
        public ShopPriceGroupDto PriceGroup { get; set; }

        public byte PeriodType { get; set; }
        public int Period { get; set; }
        public int Price { get; set; }
        public bool IsRefundable { get; set; }
        public int Durability { get; set; }
        public bool IsEnabled { get; set; }

        public IEnumerable<LicenseRewardDto> LicenseRewards { get; }
        public IEnumerable<PlayerItemDto> PlayerItems { get; }
        public IEnumerable<StartItemDto> StartItems { get; }
    }

    [Table("shop_items")]
    public class ShopItemDto
    {
        [Key]
        public uint Id { get; set; }
        public byte RequiredGender { get; set; }
        public byte RequiredLicense { get; set; }
        public byte Colors { get; set; }
        public byte UniqueColors { get; set; }
        public byte RequiredLevel { get; set; }
        public byte LevelLimit { get; set; }
        public byte RequiredMasterLevel { get; set; }
        public bool IsOneTimeUse { get; set; }
        public bool IsDestroyable { get; set; }

        public IEnumerable<ShopItemInfoDto> ItemInfos { get; }
    }

    [Table("shop_iteminfos")]
    public class ShopItemInfoDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(ShopItem))]
        public int ShopItemId { get; set; }
        public ShopItemDto ShopItem { get; set; }

        [ForeignKey(nameof(PriceGroup))]
        public int PriceGroupId { get; set; }
        public ShopPriceGroupDto PriceGroup { get; set; }

        [ForeignKey(nameof(EffectGroup))]
        public int EffectGroupId { get; set; }
        public ShopEffectGroupDto EffectGroup { get; set; }

        public byte DiscountPercentage { get; set; }
        public bool IsEnabled { get; set; }

        public IEnumerable<LicenseRewardDto> LicenseRewards { get; }
        public IEnumerable<PlayerItemDto> PlayerItems { get; }
        public IEnumerable<StartItemDto> StartItems { get; }
    }

    [Table("shop_version")]
    public class ShopVersionDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte Id { get; set; }
        public string Version { get; set; }
    }

    [Table("start_items")]
    public class StartItemDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(ShopItemInfo))]
        public int ShopItemInfoId { get; set; }
        public ShopItemInfoDto ShopItemInfo { get; set; }

        [ForeignKey(nameof(ShopPrice))]
        public int ShopPriceId { get; set; }
        public ShopPriceDto ShopPrice { get; set; }

        [ForeignKey(nameof(ShopEffect))]
        public int ShopEffectId { get; set; }
        public ShopEffectDto ShopEffect { get; set; }

        public byte Color { get; set; }
        public int Count { get; set; }
        public byte RequiredSecurityLevel { get; set; }
    }
}
