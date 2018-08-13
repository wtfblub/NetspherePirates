using System;

namespace Netsphere.Database
{
    public class DatabaseVersionMismatchException : Exception
    {
        public DatabaseVersionMismatchException()
            : base("Database does not have all migrations applied")
        {
        }
    }
}
