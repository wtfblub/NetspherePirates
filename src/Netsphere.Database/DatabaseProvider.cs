using System;
using System.Data.Common;
using BlubLib;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Netsphere.Common.Configuration;
using Netsphere.Common.Configuration.Orleans.Silo;

namespace Netsphere.Database
{
    public class DatabaseProvider : IDatabaseProvider
    {
        private readonly DatabasesOptions _options;

        public DatabaseProvider(IOptions<DatabasesOptions> options)
        {
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
            _options = options.Value;
        }

        public void Initialize()
        {
            if (_options.UseSqlite)
                return;

            // Make sure the databases exists
            var (_, authConnectionString) = GetConnectionString(_options.Auth);
            var (_, gameConnectionString) = GetConnectionString(_options.Game);
            CreateDatabaseIfNotExists(authConnectionString);
            CreateDatabaseIfNotExists(gameConnectionString);

            void CreateDatabaseIfNotExists(string connectionString)
            {
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
                var (provider, conn) = GetConnectionString(_options.Auth);
                return DynamicCast<TContext>.From(new AuthContext(provider, conn));
            }

            if (typeof(TContext) == typeof(GameContext))
            {
                var (provider, conn) = GetConnectionString(_options.Game);
                return DynamicCast<TContext>.From(new GameContext(provider, conn));
            }

            throw new ArgumentException("Invalid DataContext", nameof(TContext));
        }

        private (string provider, string connectionString) GetConnectionString(DatabaseOptions options)
        {
            DbConnectionStringBuilder connectionStringBuilder;

            if (_options.UseSqlite)
            {
                connectionStringBuilder = new SqliteConnectionStringBuilder
                {
                    DataSource = options.Host
                };
                if (!string.IsNullOrWhiteSpace(options.ConnectionString))
                    connectionStringBuilder.ConnectionString = options.ConnectionString;

                return (ProviderName.SQLite, connectionStringBuilder.ConnectionString);
            }

            connectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = options.Host,
                Port = (uint)options.Port,
                Database = options.Database,
                UserID = options.Username,
                Password = options.Password,
                SslMode = MySqlSslMode.None,
                Pooling = true
            };
            if (!string.IsNullOrWhiteSpace(options.ConnectionString))
                connectionStringBuilder.ConnectionString = options.ConnectionString;

            return (ProviderName.MySql, connectionStringBuilder.ConnectionString);
        }
    }
}
