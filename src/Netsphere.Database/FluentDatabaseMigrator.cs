using System;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using LinqToDB;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Netsphere.Common.Configuration;

namespace Netsphere.Database
{
    public class FluentDatabaseMigrator : IDatabaseMigrator
    {
        private readonly IMigrationRunner _auth;
        private readonly IMigrationRunner _game;

        public FluentDatabaseMigrator(IOptions<DatabasesOptions> options)
        {
            var provider = options.Value.UseSqlite ? ProviderName.SQLite : ProviderName.MySql;
            _auth = CreateRunner(nameof(Migration.Auth), options.Value.Auth, provider);
            _game = CreateRunner(nameof(Migration.Game), options.Value.Game, provider);
        }

        public bool HasMigrationsToApply()
        {
            return _auth.HasMigrationsToApplyUp() || _game.HasMigrationsToApplyUp();
        }

        public void MigrateTo(long version)
        {
            if (_auth.HasMigrationsToApplyDown(version))
                _auth.MigrateDown(version);
            else if (_auth.HasMigrationsToApplyUp(version))
                _auth.MigrateUp(version);

            if (_game.HasMigrationsToApplyDown(version))
                _game.MigrateDown(version);
            else if (_game.HasMigrationsToApplyUp(version))
                _game.MigrateUp(version);
        }

        public void MigrateTo()
        {
            if (_auth.HasMigrationsToApplyUp())
                _auth.MigrateUp();

            if (_game.HasMigrationsToApplyUp())
                _game.MigrateUp();
        }

        private static IMigrationRunner CreateRunner(string database, DatabaseOptions options, string dataProvider)
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(ConfigureRunner)
                .AddLogging(x => x.AddFluentMigratorConsole())
                .Configure<TypeFilterOptions>(x => x.Namespace = database == nameof(Migration.Auth)
                    ? typeof(Migration.Auth.Initial).Namespace
                    : typeof(Migration.Game.Initial).Namespace)
                .BuildServiceProvider()
                .GetRequiredService<IMigrationRunner>();

            void ConfigureRunner(IMigrationRunnerBuilder builder)
            {
                switch (dataProvider)
                {
                    case ProviderName.SQLite:
                    {
                        builder.AddSQLite();
                        var connectionStringBuilder = new SqliteConnectionStringBuilder();
                        if (string.IsNullOrWhiteSpace(options.ConnectionString))
                            connectionStringBuilder.DataSource = options.Host;
                        else
                            connectionStringBuilder.ConnectionString = options.ConnectionString;

                        builder.WithGlobalConnectionString(connectionStringBuilder.ConnectionString);
                        break;
                    }

                    case ProviderName.MySql:
                    {
                        builder.AddMySql5();
                        var connectionStringBuilder = new MySqlConnectionStringBuilder();
                        if (string.IsNullOrWhiteSpace(options.ConnectionString))
                        {
                            connectionStringBuilder.Server = options.Host;
                            connectionStringBuilder.Port = (uint)options.Port;
                            connectionStringBuilder.Database = options.Database;
                            connectionStringBuilder.UserID = options.Username;
                            connectionStringBuilder.Password = options.Password;
                            connectionStringBuilder.SslMode = MySqlSslMode.None;
                            connectionStringBuilder.Pooling = true;
                        }
                        else
                        {
                            connectionStringBuilder.ConnectionString = options.ConnectionString;
                        }

                        builder.WithGlobalConnectionString(connectionStringBuilder.ConnectionString);
                        break;
                    }

                    default:
                        throw new NotSupportedException($"DataProvider {dataProvider} is not supported");
                }

                builder.WithMigrationsIn(typeof(FluentDatabaseMigrator).Assembly);
            }
        }
    }
}
