using SimpleMigrations;

namespace Netsphere.Database.Migration.Game
{
    [Migration(4)]
    public class ShopTabs : SimpleMigrations.Migration
    {
        protected override void Up()
        {
            Execute(@"ALTER TABLE `shop_items` 
                        ADD COLUMN `MainTab` tinyint(3) NOT NULL DEFAULT 0;");

            Execute(@"ALTER TABLE `shop_items` 
                        ADD COLUMN `SubTab` tinyint(3) NOT NULL DEFAULT 0;");
        }

        protected override void Down()
        {
            Execute("ALTER TABLE `shop_items` DROP COLUMN `SubTab`;");
            Execute("ALTER TABLE `shop_items` DROP COLUMN `MainTab`;");
        }
    }
}
