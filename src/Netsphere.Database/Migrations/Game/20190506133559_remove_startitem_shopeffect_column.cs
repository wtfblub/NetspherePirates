using Microsoft.EntityFrameworkCore.Migrations;

namespace Netsphere.Database.Migrations.Game
{
    public partial class remove_startitem_shopeffect_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_start_items_shop_effects_ShopEffectId",
                table: "start_items");

            migrationBuilder.DropIndex(
                name: "IX_start_items_ShopEffectId",
                table: "start_items");

            migrationBuilder.DropColumn(
                name: "ShopEffectId",
                table: "start_items");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShopEffectId",
                table: "start_items",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_start_items_ShopEffectId",
                table: "start_items",
                column: "ShopEffectId");

            migrationBuilder.AddForeignKey(
                name: "FK_start_items_shop_effects_ShopEffectId",
                table: "start_items",
                column: "ShopEffectId",
                principalTable: "shop_effects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
