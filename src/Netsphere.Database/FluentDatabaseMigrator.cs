using System;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using LinqToDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Netsphere.Common.Configuration;

namespace Netsphere.Database
{
    public class FluentDatabaseMigrator : IDatabaseMigrator
    {
        private readonly IMigrationRunner _auth;
        private readonly IMigrationRunner _game;

        public FluentDatabaseMigrator(IOptions<DatabaseOptions> options)
        {
            var provider = options.Value.UseSqlite ? ProviderName.SQLite : ProviderName.MySql;
            _auth = CreateRunner(nameof(Migration.Auth), options.Value.ConnectionStrings.Auth, provider);
            _game = CreateRunner(nameof(Migration.Game), options.Value.ConnectionStrings.Game, provider);
        }

        public bool HasMigrationsToApply()
        {
            return _auth?.HasMigrationsToApplyUp() == true || _game?.HasMigrationsToApplyUp() == true;
        }

        public void MigrateTo(long version)
        {
            if (_auth != null)
            {
                if (_auth.HasMigrationsToApplyDown(version))
                    _auth.MigrateDown(version);
                else if (_auth.HasMigrationsToApplyUp(version))
                    _auth.MigrateUp(version);
            }

            if (_game != null)
            {
                if (_game.HasMigrationsToApplyDown(version))
                    _game.MigrateDown(version);
                else if (_game.HasMigrationsToApplyUp(version))
                    _game.MigrateUp(version);
            }
        }

        public void MigrateTo()
        {
            if (_auth?.HasMigrationsToApplyUp() == true)
                _auth.MigrateUp();

            if (_game?.HasMigrationsToApplyUp() == true)
                _game.MigrateUp();
        }

        private static IMigrationRunner CreateRunner(string database, string connectionString, string dataProvider)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return null;

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
                        break;
                    }

                    case ProviderName.MySql:
                    {
                        builder.AddMySql5();
                        break;
                    }

                    default:
                        throw new NotSupportedException($"DataProvider {dataProvider} is not supported");
                }

                builder.WithGlobalConnectionString(connectionString);
                builder.WithMigrationsIn(typeof(FluentDatabaseMigrator).Assembly);
            }
        }
    }
}
