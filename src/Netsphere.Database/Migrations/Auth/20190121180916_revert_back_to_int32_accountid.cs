using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Netsphere.Database.Migrations.Auth
{
    public partial class revert_back_to_int32_accountid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_nickname_history_accounts_AccountId", "nickname_history");
            migrationBuilder.DropForeignKey("FK_login_history_accounts_AccountId", "login_history");
            migrationBuilder.DropForeignKey("FK_bans_accounts_AccountId", "bans");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "nickname_history",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "login_history",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "bans",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "accounts",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddForeignKey(name: "FK_nickname_history_accounts_AccountId",
                table: "nickname_history",
                column: "AccountId",
                principalTable: "accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_login_history_accounts_AccountId",
                table: "login_history",
                column: "AccountId",
                principalTable: "accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_bans_accounts_AccountId",
                table: "bans",
                column: "AccountId",
                principalTable: "accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_nickname_history_accounts_AccountId", "nickname_history");
            migrationBuilder.DropForeignKey("FK_login_history_accounts_AccountId", "login_history");
            migrationBuilder.DropForeignKey("FK_bans_accounts_AccountId", "bans");

            migrationBuilder.AlterColumn<long>(
                name: "AccountId",
                table: "nickname_history",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "AccountId",
                table: "login_history",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "AccountId",
                table: "bans",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "accounts",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddForeignKey(name: "FK_nickname_history_accounts_AccountId",
                table: "nickname_history",
                column: "AccountId",
                principalTable: "accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_login_history_accounts_AccountId",
                table: "login_history",
                column: "AccountId",
                principalTable: "accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(name: "FK_bans_accounts_AccountId",
                table: "bans",
                column: "AccountId",
                principalTable: "accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
