using Microsoft.EntityFrameworkCore.Migrations;

namespace Netsphere.Database.Migrations.Game
{
    public partial class add_petId_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PetId",
                table: "player_characters",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_player_characters_PetId",
                table: "player_characters",
                column: "PetId");

            migrationBuilder.AddForeignKey(
                name: "FK_player_characters_player_items_PetId",
                table: "player_characters",
                column: "PetId",
                principalTable: "player_items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_player_characters_player_items_PetId",
                table: "player_characters");

            migrationBuilder.DropIndex(
                name: "IX_player_characters_PetId",
                table: "player_characters");

            migrationBuilder.DropColumn(
                name: "PetId",
                table: "player_characters");
        }
    }
}
