using SimpleMigrations;

namespace Netsphere.Database.Migration.Game
{
    [Migration(2)]
    public class LicenseRemoval : SimpleMigrations.Migration
    {
        protected override void Up()
        {
            Execute("DROP TABLE IF EXISTS `player_licenses`;");
            Execute("DROP TABLE IF EXISTS `license_rewards`;");
        }

        protected override void Down()
        {
            Execute(@"CREATE TABLE `license_rewards` (
              `Id` tinyint(3) unsigned NOT NULL DEFAULT '0',
              `ShopItemInfoId` int(11) NOT NULL,
              `ShopPriceId` int(11) NOT NULL,
              `Color` tinyint(3) unsigned NOT NULL DEFAULT '0',
              PRIMARY KEY (`Id`),
              KEY `ShopItemInfoId` (`ShopItemInfoId`),
              KEY `ShopPriceId` (`ShopPriceId`),
              CONSTRAINT `license_rewards_ibfk_1` FOREIGN KEY (`ShopItemInfoId`) REFERENCES `shop_iteminfos` (`Id`) ON DELETE CASCADE,
              CONSTRAINT `license_rewards_ibfk_2` FOREIGN KEY (`ShopPriceId`) REFERENCES `shop_prices` (`Id`) ON DELETE CASCADE
            );");

            Execute(@"CREATE TABLE `player_licenses` (
              `Id` int(11) NOT NULL,
              `PlayerId` int(11) NOT NULL,
              `License` tinyint(3) unsigned NOT NULL DEFAULT '0',
              `FirstCompletedDate` bigint(20) NOT NULL DEFAULT '0',
              `CompletedCount` int(11) NOT NULL DEFAULT '0',
              PRIMARY KEY (`Id`),
              KEY `PlayerId` (`PlayerId`),
              CONSTRAINT `player_licenses_ibfk_1` FOREIGN KEY (`PlayerId`) REFERENCES `players` (`Id`) ON DELETE CASCADE
            );");
        }
    }
}
