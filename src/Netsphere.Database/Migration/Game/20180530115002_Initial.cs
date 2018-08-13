using System.Data;
using FluentMigrator;

namespace Netsphere.Database.Migration.Game
{
    [Migration(20180530115002)]
    public class Initial : FluentMigrator.Migration
    {
        public override void Up()
        {
            Create.Table("shop_version")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Version").AsString(40).NotNullable();

            Create.Table("shop_effect_groups")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Name").AsString(20).NotNullable().Unique();

            Create.Table("shop_effects")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("EffectGroupId").AsInt32().NotNullable().ForeignKey("shop_effect_groups", "Id").OnDelete(Rule.Cascade)
                .WithColumn("Effect").AsInt64().NotNullable();

            Create.Table("shop_price_groups")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Name").AsString(20).NotNullable().Unique()
                .WithColumn("PriceType").AsByte().NotNullable();

            Create.Table("shop_prices")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("PriceGroupId").AsInt32().NotNullable().ForeignKey("shop_price_groups", "Id").OnDelete(Rule.Cascade)
                .WithColumn("PeriodType").AsByte().NotNullable()
                .WithColumn("Period").AsInt32().NotNullable()
                .WithColumn("Price").AsInt32().NotNullable()
                .WithColumn("IsRefundable").AsBoolean().NotNullable()
                .WithColumn("Durability").AsInt32().NotNullable()
                .WithColumn("IsEnabled").AsBoolean().NotNullable();

            Create.Table("shop_items")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("ItemNumber").AsInt32().NotNullable().Unique()
                .WithColumn("RequiredGender").AsByte().NotNullable()
                .WithColumn("RequiredLicense").AsByte().NotNullable()
                .WithColumn("Colors").AsByte().NotNullable()
                .WithColumn("UniqueColors").AsByte().NotNullable()
                .WithColumn("RequiredLevel").AsByte().NotNullable()
                .WithColumn("LevelLimit").AsByte().NotNullable()
                .WithColumn("RequiredMasterLevel").AsByte().NotNullable()
                .WithColumn("IsOneTimeUse").AsBoolean().NotNullable()
                .WithColumn("IsDestroyable").AsBoolean().NotNullable();

            Create.Table("shop_iteminfos")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("ShopItemId").AsInt32().NotNullable().ForeignKey("shop_items", "Id").OnDelete(Rule.Cascade)
                .WithColumn("PriceGroupId").AsInt32().NotNullable().ForeignKey("shop_price_groups", "Id").OnDelete(Rule.Cascade)
                .WithColumn("EffectGroupId").AsInt32().NotNullable().ForeignKey("shop_effect_groups", "Id").OnDelete(Rule.Cascade)
                .WithColumn("DiscountPercentage").AsByte().NotNullable()
                .WithColumn("IsEnabled").AsBoolean().NotNullable();

            Create.Table("license_rewards")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("ShopItemInfoId").AsInt32().NotNullable().ForeignKey("shop_iteminfos", "Id").OnDelete(Rule.Cascade)
                .WithColumn("ShopPriceId").AsInt32().NotNullable().ForeignKey("shop_prices", "Id").OnDelete(Rule.Cascade)
                .WithColumn("Color").AsByte().NotNullable();

            Create.Table("start_items")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("ShopItemInfoId").AsInt32().NotNullable().ForeignKey("shop_iteminfos", "Id").OnDelete(Rule.Cascade)
                .WithColumn("ShopPriceId").AsInt32().NotNullable().ForeignKey("shop_prices", "Id").OnDelete(Rule.Cascade)
                .WithColumn("ShopEffectId").AsInt32().NotNullable().ForeignKey("shop_effects", "Id").OnDelete(Rule.Cascade)
                .WithColumn("Color").AsByte().NotNullable()
                .WithColumn("Count").AsInt32().NotNullable()
                .WithColumn("RequiredSecurityLevel").AsByte().NotNullable();

            Create.Table("players")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("AccountId").AsInt32().NotNullable().Unique()
                .WithColumn("TutorialState").AsByte().NotNullable()
                .WithColumn("TotalExperience").AsInt32().NotNullable()
                .WithColumn("PEN").AsInt32().NotNullable()
                .WithColumn("AP").AsInt32().NotNullable()
                .WithColumn("Coins1").AsInt32().NotNullable()
                .WithColumn("Coins2").AsInt32().NotNullable()
                .WithColumn("CurrentCharacterSlot").AsByte().NotNullable();

            Create.Table("player_items")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("PlayerId").AsInt32().NotNullable().ForeignKey("players", "Id").OnDelete(Rule.Cascade)
                .WithColumn("ShopItemInfoId").AsInt32().NotNullable().ForeignKey("shop_iteminfos", "Id").OnDelete(Rule.Cascade)
                .WithColumn("ShopPriceId").AsInt32().NotNullable().ForeignKey("shop_prices", "Id").OnDelete(Rule.Cascade)
                .WithColumn("Effect").AsInt64().NotNullable()
                .WithColumn("Color").AsByte().NotNullable()
                .WithColumn("PurchaseDate").AsInt64().NotNullable()
                .WithColumn("Durability").AsInt32().NotNullable()
                .WithColumn("Count").AsInt32().NotNullable();

            {
                var table = Create.Table("player_characters")
                    .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                    .WithColumn("PlayerId").AsInt32().NotNullable().ForeignKey("players", "Id").OnDelete(Rule.Cascade)
                    .WithColumn("Slot").AsByte().NotNullable()
                    .WithColumn("Gender").AsByte().NotNullable()
                    .WithColumn("BasicHair").AsByte().NotNullable()
                    .WithColumn("BasicFace").AsByte().NotNullable()
                    .WithColumn("BasicShirt").AsByte().NotNullable()
                    .WithColumn("BasicPants").AsByte().NotNullable();

                AddItemColumn("Weapon1Id");
                AddItemColumn("Weapon2Id");
                AddItemColumn("Weapon3Id");
                AddItemColumn("SkillId");
                AddItemColumn("HairId");
                AddItemColumn("FaceId");
                AddItemColumn("ShirtId");
                AddItemColumn("PantsId");
                AddItemColumn("GlovesId");
                AddItemColumn("ShoesId");
                AddItemColumn("AccessoryId");

                void AddItemColumn(string name)
                {
                    table.WithColumn(name)
                        .AsInt32()
                        .Nullable()
                        .WithDefaultValue(null)
                        .ForeignKey("player_items", "Id").OnDelete(Rule.SetNull);
                }
            }

            Create.Table("player_deny")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("PlayerId").AsInt32().NotNullable().ForeignKey("players", "Id").OnDelete(Rule.Cascade)
                .WithColumn("DenyPlayerId").AsInt32().NotNullable().ForeignKey("players", "Id").OnDelete(Rule.Cascade);

            Create.Table("player_licenses")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("PlayerId").AsInt32().NotNullable().ForeignKey("players", "Id").OnDelete(Rule.Cascade)
                .WithColumn("License").AsByte().NotNullable()
                .WithColumn("FirstCompletedDate").AsInt64().NotNullable()
                .WithColumn("CompletedCount").AsInt32().NotNullable();

            Create.Table("player_mails")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("PlayerId").AsInt32().NotNullable().ForeignKey("players", "Id")
                .WithColumn("SenderPlayerId").AsInt32().NotNullable().ForeignKey("players", "Id")
                .WithColumn("SentDate").AsInt64().NotNullable()
                .WithColumn("Title").AsString(100).NotNullable()
                .WithColumn("Message").AsString(500).NotNullable()
                .WithColumn("IsMailNew").AsBoolean().NotNullable()
                .WithColumn("IsMailDeleted").AsBoolean().NotNullable();

            Create.Table("player_settings")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("PlayerId").AsInt32().NotNullable().ForeignKey("players", "Id").OnDelete(Rule.Cascade)
                .WithColumn("Setting").AsString(100).NotNullable()
                .WithColumn("Value").AsString(512).NotNullable();
        }

        public override void Down()
        {
            Delete.Table("player_settings");
            Delete.Table("player_mails");
            Delete.Table("player_licenses");
            Delete.Table("player_deny");
            Delete.Table("player_characters");
            Delete.Table("player_items");
            Delete.Table("players");
            Delete.Table("start_items");
            Delete.Table("license_rewards");
            Delete.Table("shop_iteminfos");
            Delete.Table("shop_items");
            Delete.Table("shop_prices");
            Delete.Table("shop_price_groups");
            Delete.Table("shop_effects");
            Delete.Table("shop_effect_groups");
            Delete.Table("shop_version");
        }
    }
}
