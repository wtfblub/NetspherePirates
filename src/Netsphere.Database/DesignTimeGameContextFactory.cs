using Hjson;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Netsphere.Common.Configuration;

namespace Netsphere.Database
{
    public class DesignTimeGameContextFactory : IDesignTimeDbContextFactory<GameContext>
    {
        public GameContext CreateDbContext(string[] args)
        {
            var connectionString =
                HjsonValue.Load("config.hjson")
                    ["Database"]
                    [nameof(DatabaseOptions.ConnectionStrings)]
                    [nameof(ConnectionStrings.Game)]
                    .ToValue().ToString();

            return new GameContext(
                new DbContextOptionsBuilder<GameContext>()
                    .UseMySql(connectionString)
                    .Options
            );
        }
    }
}
