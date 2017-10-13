using SimpleMigrations;

namespace Netsphere.Database.Migration.Game
{
    [Migration(3)]
    public class Channels : SimpleMigrations.Migration
    {
        protected override void Up()
        {
            Execute(@"CREATE TABLE `channels` (
              `Id`  int NOT NULL AUTO_INCREMENT ,
              `Name`  varchar(255) NOT NULL DEFAULT '' ,
              `Description`  varchar(255) NOT NULL DEFAULT '' ,
              `PlayerLimit`  tinyint NOT NULL DEFAULT 0 ,
              `MinLevel`  tinyint NOT NULL DEFAULT 0 ,
              `MaxLevel`  tinyint NOT NULL DEFAULT 0 ,
              `Color`  int NOT NULL DEFAULT 0 ,
              `TooltipColor`  int NOT NULL DEFAULT 0 ,
              PRIMARY KEY (`Id`)
            );");
        }

        protected override void Down()
        {
            Execute("DROP TABLE IF EXISTS `channels`;");
        }
    }
}
