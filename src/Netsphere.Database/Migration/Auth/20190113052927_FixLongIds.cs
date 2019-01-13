using System.Data;
using FluentMigrator;

namespace Netsphere.Database.Migration.Auth
{
    [Migration(20190113052927)]
    public class FixLongIds : FluentMigrator.Migration
    {
        public override void Up()
        {
            Delete.ForeignKey()
                .FromTable("bans").ForeignColumn("AccountId")
                .ToTable("accounts").PrimaryColumn("Id");
            Delete.ForeignKey()
                .FromTable("login_history").ForeignColumn("AccountId")
                .ToTable("accounts").PrimaryColumn("Id");
            Delete.ForeignKey()
                .FromTable("nickname_history").ForeignColumn("AccountId")
                .ToTable("accounts").PrimaryColumn("Id");

            Alter.Table("accounts").AlterColumn("Id").AsInt64().PrimaryKey().Identity();
            Alter.Table("bans").AlterColumn("AccountId").AsInt64();
            Alter.Table("login_history").AlterColumn("AccountId").AsInt64();
            Alter.Table("nickname_history").AlterColumn("AccountId").AsInt64();

            Create.ForeignKey()
                .FromTable("bans").ForeignColumn("AccountId")
                .ToTable("accounts").PrimaryColumn("Id")
                .OnDelete(Rule.Cascade);
            Create.ForeignKey()
                .FromTable("login_history").ForeignColumn("AccountId")
                .ToTable("accounts").PrimaryColumn("Id")
                .OnDelete(Rule.Cascade);
            Create.ForeignKey()
                .FromTable("nickname_history").ForeignColumn("AccountId")
                .ToTable("accounts").PrimaryColumn("Id")
                .OnDelete(Rule.Cascade);
        }

        public override void Down()
        {
            Delete.ForeignKey()
                .FromTable("bans").ForeignColumn("AccountId")
                .ToTable("accounts").PrimaryColumn("Id");
            Delete.ForeignKey()
                .FromTable("login_history").ForeignColumn("AccountId")
                .ToTable("accounts").PrimaryColumn("Id");
            Delete.ForeignKey()
                .FromTable("nickname_history").ForeignColumn("AccountId")
                .ToTable("accounts").PrimaryColumn("Id");

            Alter.Table("accounts").AlterColumn("Id").AsInt32().PrimaryKey().Identity();
            Alter.Table("bans").AlterColumn("AccountId").AsInt32();
            Alter.Table("login_history").AlterColumn("AccountId").AsInt32();
            Alter.Table("nickname_history").AlterColumn("AccountId").AsInt32();

            Create.ForeignKey()
                .FromTable("bans").ForeignColumn("AccountId")
                .ToTable("accounts").PrimaryColumn("Id")
                .OnDelete(Rule.Cascade);
            Create.ForeignKey()
                .FromTable("login_history").ForeignColumn("AccountId")
                .ToTable("accounts").PrimaryColumn("Id")
                .OnDelete(Rule.Cascade);
            Create.ForeignKey()
                .FromTable("nickname_history").ForeignColumn("AccountId")
                .ToTable("accounts").PrimaryColumn("Id")
                .OnDelete(Rule.Cascade);
        }
    }
}
