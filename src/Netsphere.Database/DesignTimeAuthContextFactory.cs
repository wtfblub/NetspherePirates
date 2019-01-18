using Hjson;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Netsphere.Common.Configuration;

namespace Netsphere.Database
{
    public class DesignTimeAuthContextFactory : IDesignTimeDbContextFactory<AuthContext>
    {
        public AuthContext CreateDbContext(string[] args)
        {
            var connectionString =
                HjsonValue.Load("config.hjson")
                    ["Database"]
                    [nameof(DatabaseOptions.ConnectionStrings)]
                    [nameof(ConnectionStrings.Auth)]
                    .ToValue().ToString();

            return new AuthContext(
                new DbContextOptionsBuilder<AuthContext>()
                    .UseMySql(connectionString)
                    .Options
            );
        }
    }
}
