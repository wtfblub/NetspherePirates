using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Netsphere.Database
{
    public class DatabaseService
    {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TContext Open<TContext>()
            where TContext : DbContext
        {
            return _serviceProvider.GetRequiredService<TContext>();
        }
    }
}
