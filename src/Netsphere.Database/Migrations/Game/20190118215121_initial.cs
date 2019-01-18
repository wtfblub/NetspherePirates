using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Netsphere.Database.Migrations.Game
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TutorialState = table.Column<byte>(nullable: false),
                    TotalExperience = table.Column<int>(nullable: false),
                    PEN = table.Column<int>(nullable: false),
                    AP = table.Column<int>(nullable: false),
                    Coins1 = table.Column<int>(nullable: false),
                    Coins2 = table.Column<int>(nullable: false),
                    CurrentCharacterSlot = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "shop_effect_groups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shop_effect_groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "shop_items",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RequiredGender = table.Column<byte>(nullable: false),
                    RequiredLicense = table.Column<byte>(nullable: false),
                    Colors = table.Column<byte>(nullable: false),
                    UniqueColors = table.Column<byte>(nullable: false),
                    RequiredLevel = table.Column<byte>(nullable: false),
                    LevelLimit = table.Column<byte>(nullable: false),
                    RequiredMasterLevel = table.Column<byte>(nullable: false),
                    IsOneTimeUse = table.Column<bool>(nullable: false),
                    IsDestroyable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shop_items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "shop_price_groups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    PriceType = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shop_price_groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "shop_version",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Version = table.Column<string>(maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shop_version", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "player_deny",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<long>(nullable: false),
                    DenyPlayerId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_deny", x => x.Id);
                    table.ForeignKey(
                        name: "FK_player_deny_players_DenyPlayerId",
                        column: x => x.DenyPlayerId,
                        principalTable: "players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_player_deny_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_licenses",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<long>(nullable: false),
                    License = table.Column<byte>(nullable: false),
                    FirstCompletedDate = table.Column<long>(nullable: false),
                    CompletedCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_licenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_player_licenses_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_mails",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<long>(nullable: false),
                    SenderPlayerId = table.Column<long>(nullable: false),
                    SentDate = table.Column<long>(nullable: false),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    Message = table.Column<string>(maxLength: 500, nullable: false),
                    IsMailNew = table.Column<bool>(nullable: false),
                    IsMailDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_mails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_player_mails_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_player_mails_players_SenderPlayerId",
                        column: x => x.SenderPlayerId,
                        principalTable: "players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_settings",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<long>(nullable: false),
                    Setting = table.Column<string>(maxLength: 100, nullable: false),
                    Value = table.Column<string>(maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_player_settings_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shop_effects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EffectGroupId = table.Column<int>(nullable: false),
                    Effect = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shop_effects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_shop_effects_shop_effect_groups_EffectGroupId",
                        column: x => x.EffectGroupId,
                        principalTable: "shop_effect_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shop_iteminfos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ShopItemId = table.Column<long>(nullable: false),
                    PriceGroupId = table.Column<int>(nullable: false),
                    EffectGroupId = table.Column<int>(nullable: false),
                    DiscountPercentage = table.Column<byte>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shop_iteminfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_shop_iteminfos_shop_effect_groups_EffectGroupId",
                        column: x => x.EffectGroupId,
                        principalTable: "shop_effect_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_shop_iteminfos_shop_price_groups_PriceGroupId",
                        column: x => x.PriceGroupId,
                        principalTable: "shop_price_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_shop_iteminfos_shop_items_ShopItemId",
                        column: x => x.ShopItemId,
                        principalTable: "shop_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shop_prices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PriceGroupId = table.Column<int>(nullable: false),
                    PeriodType = table.Column<byte>(nullable: false),
                    Period = table.Column<int>(nullable: false),
                    Price = table.Column<int>(nullable: false),
                    IsRefundable = table.Column<bool>(nullable: false),
                    Durability = table.Column<int>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shop_prices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_shop_prices_shop_price_groups_PriceGroupId",
                        column: x => x.PriceGroupId,
                        principalTable: "shop_price_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "license_rewards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ShopItemInfoId = table.Column<int>(nullable: false),
                    ShopPriceId = table.Column<int>(nullable: false),
                    Color = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_license_rewards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_license_rewards_shop_iteminfos_ShopItemInfoId",
                        column: x => x.ShopItemInfoId,
                        principalTable: "shop_iteminfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_license_rewards_shop_prices_ShopPriceId",
                        column: x => x.ShopPriceId,
                        principalTable: "shop_prices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_items",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<long>(nullable: false),
                    ShopItemInfoId = table.Column<int>(nullable: false),
                    ShopPriceId = table.Column<int>(nullable: false),
                    Effect = table.Column<uint>(nullable: false),
                    Color = table.Column<byte>(nullable: false),
                    PurchaseDate = table.Column<long>(nullable: false),
                    Durability = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_player_items_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_player_items_shop_iteminfos_ShopItemInfoId",
                        column: x => x.ShopItemInfoId,
                        principalTable: "shop_iteminfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_player_items_shop_prices_ShopPriceId",
                        column: x => x.ShopPriceId,
                        principalTable: "shop_prices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "start_items",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ShopItemInfoId = table.Column<int>(nullable: false),
                    ShopPriceId = table.Column<int>(nullable: false),
                    ShopEffectId = table.Column<int>(nullable: false),
                    Color = table.Column<byte>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    RequiredSecurityLevel = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_start_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_start_items_shop_effects_ShopEffectId",
                        column: x => x.ShopEffectId,
                        principalTable: "shop_effects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_start_items_shop_iteminfos_ShopItemInfoId",
                        column: x => x.ShopItemInfoId,
                        principalTable: "shop_iteminfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_start_items_shop_prices_ShopPriceId",
                        column: x => x.ShopPriceId,
                        principalTable: "shop_prices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_characters",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<long>(nullable: false),
                    Slot = table.Column<byte>(nullable: false),
                    Gender = table.Column<byte>(nullable: false),
                    BasicHair = table.Column<byte>(nullable: false),
                    BasicFace = table.Column<byte>(nullable: false),
                    BasicShirt = table.Column<byte>(nullable: false),
                    BasicPants = table.Column<byte>(nullable: false),
                    Weapon1Id = table.Column<long>(nullable: true),
                    Weapon2Id = table.Column<long>(nullable: true),
                    Weapon3Id = table.Column<long>(nullable: true),
                    SkillId = table.Column<long>(nullable: true),
                    HairId = table.Column<long>(nullable: true),
                    FaceId = table.Column<long>(nullable: true),
                    ShirtId = table.Column<long>(nullable: true),
                    PantsId = table.Column<long>(nullable: true),
                    GlovesId = table.Column<long>(nullable: true),
                    ShoesId = table.Column<long>(nullable: true),
                    AccessoryId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_player_characters_player_items_AccessoryId",
                        column: x => x.AccessoryId,
                        principalTable: "player_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_player_characters_player_items_FaceId",
                        column: x => x.FaceId,
                        principalTable: "player_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_player_characters_player_items_GlovesId",
                        column: x => x.GlovesId,
                        principalTable: "player_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_player_characters_player_items_HairId",
                        column: x => x.HairId,
                        principalTable: "player_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_player_characters_player_items_PantsId",
                        column: x => x.PantsId,
                        principalTable: "player_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_player_characters_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_player_characters_player_items_ShirtId",
                        column: x => x.ShirtId,
                        principalTable: "player_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_player_characters_player_items_ShoesId",
                        column: x => x.ShoesId,
                        principalTable: "player_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_player_characters_player_items_SkillId",
                        column: x => x.SkillId,
                        principalTable: "player_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_player_characters_player_items_Weapon1Id",
                        column: x => x.Weapon1Id,
                        principalTable: "player_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_player_characters_player_items_Weapon2Id",
                        column: x => x.Weapon2Id,
                        principalTable: "player_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_player_characters_player_items_Weapon3Id",
                        column: x => x.Weapon3Id,
                        principalTable: "player_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_license_rewards_ShopItemInfoId",
                table: "license_rewards",
                column: "ShopItemInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_license_rewards_ShopPriceId",
                table: "license_rewards",
                column: "ShopPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_player_characters_AccessoryId",
                table: "player_characters",
                column: "AccessoryId");

            migrationBuilder.CreateIndex(
                name: "IX_player_characters_FaceId",
                table: "player_characters",
                column: "FaceId");

            migrationBuilder.CreateIndex(
                name: "IX_player_characters_GlovesId",
                table: "player_characters",
                column: "GlovesId");

            migrationBuilder.CreateIndex(
                name: "IX_player_characters_HairId",
                table: "player_characters",
                column: "HairId");

            migrationBuilder.CreateIndex(
                name: "IX_player_characters_PantsId",
                table: "player_characters",
                column: "PantsId");

            migrationBuilder.CreateIndex(
                name: "IX_player_characters_PlayerId",
                table: "player_characters",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_player_characters_ShirtId",
                table: "player_characters",
                column: "ShirtId");

            migrationBuilder.CreateIndex(
                name: "IX_player_characters_ShoesId",
                table: "player_characters",
                column: "ShoesId");

            migrationBuilder.CreateIndex(
                name: "IX_player_characters_SkillId",
                table: "player_characters",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_player_characters_Weapon1Id",
                table: "player_characters",
                column: "Weapon1Id");

            migrationBuilder.CreateIndex(
                name: "IX_player_characters_Weapon2Id",
                table: "player_characters",
                column: "Weapon2Id");

            migrationBuilder.CreateIndex(
                name: "IX_player_characters_Weapon3Id",
                table: "player_characters",
                column: "Weapon3Id");

            migrationBuilder.CreateIndex(
                name: "IX_player_deny_DenyPlayerId",
                table: "player_deny",
                column: "DenyPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_player_deny_PlayerId",
                table: "player_deny",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_player_items_PlayerId",
                table: "player_items",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_player_items_ShopItemInfoId",
                table: "player_items",
                column: "ShopItemInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_player_items_ShopPriceId",
                table: "player_items",
                column: "ShopPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_player_licenses_PlayerId",
                table: "player_licenses",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_player_mails_PlayerId",
                table: "player_mails",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_player_mails_SenderPlayerId",
                table: "player_mails",
                column: "SenderPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_player_settings_PlayerId",
                table: "player_settings",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_shop_effect_groups_Name",
                table: "shop_effect_groups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_shop_effects_EffectGroupId",
                table: "shop_effects",
                column: "EffectGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_shop_iteminfos_EffectGroupId",
                table: "shop_iteminfos",
                column: "EffectGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_shop_iteminfos_PriceGroupId",
                table: "shop_iteminfos",
                column: "PriceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_shop_iteminfos_ShopItemId",
                table: "shop_iteminfos",
                column: "ShopItemId");

            migrationBuilder.CreateIndex(
                name: "IX_shop_price_groups_Name",
                table: "shop_price_groups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_shop_prices_PriceGroupId",
                table: "shop_prices",
                column: "PriceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_start_items_ShopEffectId",
                table: "start_items",
                column: "ShopEffectId");

            migrationBuilder.CreateIndex(
                name: "IX_start_items_ShopItemInfoId",
                table: "start_items",
                column: "ShopItemInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_start_items_ShopPriceId",
                table: "start_items",
                column: "ShopPriceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "license_rewards");

            migrationBuilder.DropTable(
                name: "player_characters");

            migrationBuilder.DropTable(
                name: "player_deny");

            migrationBuilder.DropTable(
                name: "player_licenses");

            migrationBuilder.DropTable(
                name: "player_mails");

            migrationBuilder.DropTable(
                name: "player_settings");

            migrationBuilder.DropTable(
                name: "shop_version");

            migrationBuilder.DropTable(
                name: "start_items");

            migrationBuilder.DropTable(
                name: "player_items");

            migrationBuilder.DropTable(
                name: "shop_effects");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "shop_iteminfos");

            migrationBuilder.DropTable(
                name: "shop_prices");

            migrationBuilder.DropTable(
                name: "shop_effect_groups");

            migrationBuilder.DropTable(
                name: "shop_items");

            migrationBuilder.DropTable(
                name: "shop_price_groups");
        }
    }
}
