namespace Netsphere.Database
{
    public interface IDatabaseMigrator
    {
        bool HasMigrationsToApply();

        void MigrateTo(long version);

        void MigrateTo();
    }
}
