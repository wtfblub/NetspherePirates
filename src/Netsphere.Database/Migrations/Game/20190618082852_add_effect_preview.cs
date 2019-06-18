using Microsoft.EntityFrameworkCore.Migrations;

namespace Netsphere.Database.Migrations.Game
{
    public partial class add_effect_preview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "PreviewEffect",
                table: "shop_effect_groups",
                nullable: false,
                defaultValue: 0u);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviewEffect",
                table: "shop_effect_groups");
        }
    }
}
