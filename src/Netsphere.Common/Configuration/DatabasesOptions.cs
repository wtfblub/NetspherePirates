namespace Netsphere.Common.Configuration
{
    public class DatabasesOptions
    {
        public DatabaseOptions Auth { get; set; }
        public DatabaseOptions Game { get; set; }
        public bool RunMigration { get; set; }
        public bool UseSqlite { get; set; }

        public DatabasesOptions()
        {
            Auth = new DatabaseOptions { Database = "auth" };
            Game = new DatabaseOptions { Database = "game" };
        }
    }
}
