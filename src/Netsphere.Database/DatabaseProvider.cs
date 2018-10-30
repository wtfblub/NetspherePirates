using System;
using BlubLib;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Netsphere.Common.Configuration;

namespace Netsphere.Database
{
    public class DatabaseProvider : IDatabaseProvider
    {
        private readonly DatabaseOptions _options;

        public DatabaseProvider(IOptions<DatabaseOptions> options)
        {
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
            _options = options.Value;
        }

        public void Initialize()
        {
            if (_options.UseSqlite)
                return;

            // Make sure the databases exists
            CreateDatabaseIfNotExists(_options.ConnectionStrings.Auth);
            CreateDatabaseIfNotExists(_options.ConnectionStrings.Game);

            void CreateDatabaseIfNotExists(string connectionString)
            {
                if (string.IsNullOrWhiteSpace(connectionString))
                    return;

                var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);
                var database = connectionStringBuilder.Database;
                connectionStringBuilder.Database = null;
                using (var db = new MySqlConnection(connectionStringBuilder.ConnectionString))
                {
                    db.Open();
                    using (var cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {database}"))
                    {
                        cmd.Connection = db;
                        cmd.ExecuteNonQuery();
                    }
                }

                connectionStringBuilder.Database = database;
            }
        }

        public TContext Open<TContext>()
            where TContext : DataConnection
        {
            if (typeof(TContext) == typeof(AuthContext))
            {
                if (string.IsNullOrWhiteSpace(_options.ConnectionStrings.Auth))
                    throw new Exception("No auth database was configured");

                return DynamicCast<TContext>.From(new AuthContext(GetProvider(), _options.ConnectionStrings.Auth));
            }

            if (typeof(TContext) == typeof(GameContext))
            {
                if (string.IsNullOrWhiteSpace(_options.ConnectionStrings.Game))
                    throw new Exception("No game database was configured");

                return DynamicCast<TContext>.From(new GameContext(GetProvider(), _options.ConnectionStrings.Game));
            }

            throw new ArgumentException("Invalid DataContext", nameof(TContext));
        }

        private string GetProvider()
        {
            return _options.UseSqlite ? ProviderName.SQLite : ProviderName.MySql;
        }
    }
}
