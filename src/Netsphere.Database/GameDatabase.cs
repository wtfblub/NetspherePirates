using System.Data.Common;
using Dapper;
using Dapper.FastCrud;
using Netsphere.Configuration;
using Netsphere.Database.Migration.Game;
using SimpleMigrations;
using SimpleMigrations.DatabaseProvider;

namespace Netsphere.Database
{
    public static class GameDatabase
    {
        private static string s_connectionString;

        public static void Initialize(DatabaseConfig config)
        {
            s_connectionString = $"SslMode=none;Server={config.Host};Port={config.Port};Database={config.Database};Uid={config.Username};Pwd={config.Password};Pooling=true;";
            OrmConfiguration.DefaultDialect = SqlDialect.MySql;

            using (var con = Open())
            {
                if (con.QueryFirstOrDefault($"SHOW DATABASES LIKE \"{config.Database}\"") == null)
                    throw new DatabaseNotFoundException(config.Database);

                var databaseProvider = new MysqlDatabaseProvider(con) { TableName = "__version" };
                var assemblyProvider = new AssemblyMigrationProvider(typeof(Base).Assembly, typeof(Base).Namespace);
                var migrator = new SimpleMigrator(assemblyProvider, databaseProvider);
                migrator.Load();
                if (migrator.CurrentMigration.Version != migrator.LatestMigration.Version)
                {
                    if (config.RunMigration)
                        migrator.MigrateToLatest();
                    else
                        throw new DatabaseVersionMismatchException(migrator.CurrentMigration.Version, migrator.LatestMigration.Version);
                }
            }
        }

        public static DbConnection Open()
        {
            var connection = new MySql.Data.MySqlClient.MySqlConnection(s_connectionString);
            connection.Open();
            return connection;
        }
    }
}
