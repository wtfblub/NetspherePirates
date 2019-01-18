using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Netsphere.Database
{
    public static class DatabaseServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContext<TContext>(this IServiceCollection This,
            Action<DbContextOptionsBuilder> optionsBuilder)
            where TContext : DbContext
        {
            return This
                .AddSingleton(x =>
                {
                    var builder = new DbContextOptionsBuilder<TContext>();
                    optionsBuilder(builder);
                    return builder.Options;
                })
                .AddTransient<TContext>()
                .AddTransient<DbContext>(x => x.GetRequiredService<TContext>());
        }
    }
}
