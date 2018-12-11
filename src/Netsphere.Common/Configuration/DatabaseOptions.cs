namespace Netsphere.Common.Configuration
{
    public class DatabaseOptions
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public bool RunMigration { get; set; }
        public bool UseSqlite { get; set; }
    }
}
