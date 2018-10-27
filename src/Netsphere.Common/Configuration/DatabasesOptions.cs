namespace Netsphere.Common.Configuration
{
    public class DatabasesOptions
    {
        public DatabaseOptions Auth { get; set; }
        public DatabaseOptions Game { get; set; }
        public bool RunMigration { get; set; }
        public bool UseSqlite { get; set; }
    }
}
