using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Netsphere.Database.Migrations.Game
{
    public partial class remove_licenses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "license_rewards");

            migrationBuilder.DropTable(
                name: "player_licenses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "license_rewards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Color = table.Column<byte>(nullable: false),
                    ShopItemInfoId = table.Column<int>(nullable: false),
                    ShopPriceId = table.Column<int>(nullable: false)
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
                name: "player_licenses",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CompletedCount = table.Column<int>(nullable: false),
                    FirstCompletedDate = table.Column<long>(nullable: false),
                    License = table.Column<byte>(nullable: false),
                    PlayerId = table.Column<int>(nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_license_rewards_ShopItemInfoId",
                table: "license_rewards",
                column: "ShopItemInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_license_rewards_ShopPriceId",
                table: "license_rewards",
                column: "ShopPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_player_licenses_PlayerId",
                table: "player_licenses",
                column: "PlayerId");
        }
    }
}
