using Microsoft.EntityFrameworkCore.Migrations;

namespace Netsphere.Database.Migrations.Game
{
    public partial class update_multiple_item_effects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Effect",
                table: "player_items");

            migrationBuilder.AddColumn<string>(
                name: "Effects",
                table: "player_items",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Effects",
                table: "player_items");

            migrationBuilder.AddColumn<uint>(
                name: "Effect",
                table: "player_items",
                nullable: false,
                defaultValue: 0u);
        }
    }
}
