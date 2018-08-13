using System.Data;
using FluentMigrator;

namespace Netsphere.Database.Migration.Auth
{
    [Migration(20180530115002)]
    public class Initial : FluentMigrator.Migration
    {
        public override void Up()
        {
            Create.Table("accounts")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Username").AsString(40).NotNullable().Unique()
                .WithColumn("Nickname").AsString(40).Nullable().Unique()
                .WithColumn("Password").AsString(40).Nullable()
                .WithColumn("Salt").AsString(40).Nullable()
                .WithColumn("SecurityLevel").AsByte().NotNullable();

            Create.Table("bans")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("AccountId").AsInt32().NotNullable().ForeignKey("accounts", "Id").OnDelete(Rule.Cascade)
                .WithColumn("Date").AsInt64().NotNullable()
                .WithColumn("Duration").AsInt64().Nullable()
                .WithColumn("Reason").AsString().Nullable();

            Create.Table("login_history")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("AccountId").AsInt32().NotNullable().ForeignKey("accounts", "Id").OnDelete(Rule.Cascade)
                .WithColumn("Date").AsInt64().NotNullable()
                .WithColumn("IP").AsString(15).NotNullable();

            Create.Table("nickname_history")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("AccountId").AsInt32().NotNullable().ForeignKey("accounts", "Id").OnDelete(Rule.Cascade)
                .WithColumn("Nickname").AsString(40).NotNullable()
                .WithColumn("ExpireDate").AsInt64().Nullable();
        }

        public override void Down()
        {
            Delete.Table("nickname_history");
            Delete.Table("login_history");
            Delete.Table("bans");
            Delete.Table("accounts");
        }
    }
}
