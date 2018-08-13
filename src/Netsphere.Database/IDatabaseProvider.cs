using LinqToDB.Data;

namespace Netsphere.Database
{
    public interface IDatabaseProvider
    {
        void Initialize();

        TContext Open<TContext>()
            where TContext : DataConnection;
    }
}
