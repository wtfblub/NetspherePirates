using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Netsphere.Database.Migrations.Game
{
    public partial class revert_back_to_int32_accountid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_player_settings_players_PlayerId", "player_settings");
            migrationBuilder.DropForeignKey("FK_player_mails_players_SenderPlayerId", "player_mails");
            migrationBuilder.DropForeignKey("FK_player_mails_players_PlayerId", "player_mails");
            migrationBuilder.DropForeignKey("FK_player_licenses_players_PlayerId", "player_licenses");
            migrationBuilder.DropForeignKey("FK_player_items_players_PlayerId", "player_items");
            migrationBuilder.DropForeignKey("FK_player_deny_players_PlayerId", "player_deny");
            migrationBuilder.DropForeignKey("FK_player_deny_players_DenyPlayerId", "player_deny");
            migrationBuilder.DropForeignKey("FK_player_characters_players_PlayerId", "player_characters");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "players",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "player_settings",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "SenderPlayerId",
                table: "player_mails",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "player_mails",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "player_licenses",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "player_items",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "player_deny",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "DenyPlayerId",
                table: "player_deny",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "player_characters",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(name: "FK_player_settings_players_PlayerId",
                table: "player_settings",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_player_mails_players_SenderPlayerId",
                table: "player_mails",
                column: "SenderPlayerId",
                principalTable: "players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_player_mails_players_PlayerId",
                table: "player_mails",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_player_licenses_players_PlayerId",
                table: "player_licenses",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_player_items_players_PlayerId",
                table: "player_items",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_player_deny_players_PlayerId",
                table: "player_deny",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_player_deny_players_DenyPlayerId",
                table: "player_deny",
                column: "DenyPlayerId",
                principalTable: "players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_player_characters_players_PlayerId",
                table: "player_characters",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_player_settings_players_PlayerId", "player_settings");
            migrationBuilder.DropForeignKey("FK_player_mails_players_SenderPlayerId", "player_mails");
            migrationBuilder.DropForeignKey("FK_player_mails_players_PlayerId", "player_mails");
            migrationBuilder.DropForeignKey("FK_player_licenses_players_PlayerId", "player_licenses");
            migrationBuilder.DropForeignKey("FK_player_items_players_PlayerId", "player_items");
            migrationBuilder.DropForeignKey("FK_player_deny_players_PlayerId", "player_deny");
            migrationBuilder.DropForeignKey("FK_player_deny_players_DenyPlayerId", "player_deny");
            migrationBuilder.DropForeignKey("FK_player_characters_players_PlayerId", "player_characters");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "players",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "PlayerId",
                table: "player_settings",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "SenderPlayerId",
                table: "player_mails",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "PlayerId",
                table: "player_mails",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "PlayerId",
                table: "player_licenses",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "PlayerId",
                table: "player_items",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "PlayerId",
                table: "player_deny",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "DenyPlayerId",
                table: "player_deny",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "PlayerId",
                table: "player_characters",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(name: "FK_player_settings_players_PlayerId",
                table: "player_settings",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_player_mails_players_SenderPlayerId",
                table: "player_mails",
                column: "SenderPlayerId",
                principalTable: "players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_player_mails_players_PlayerId",
                table: "player_mails",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_player_licenses_players_PlayerId",
                table: "player_licenses",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_player_items_players_PlayerId",
                table: "player_items",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_player_deny_players_PlayerId",
                table: "player_deny",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_player_deny_players_DenyPlayerId",
                table: "player_deny",
                column: "DenyPlayerId",
                principalTable: "players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_player_characters_players_PlayerId",
                table: "player_characters",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
