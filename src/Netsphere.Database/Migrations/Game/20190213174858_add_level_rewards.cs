using Microsoft.EntityFrameworkCore.Migrations;

namespace Netsphere.Database.Migrations.Game
{
    public partial class add_level_rewards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "level_rewards",
                columns: table => new
                {
                    Level = table.Column<int>(nullable: false),
                    MoneyType = table.Column<byte>(nullable: false),
                    Money = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_level_rewards", x => x.Level);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "level_rewards");
        }
    }
}
