using System;

namespace Netsphere.Database
{
    public class DatabaseNotFoundException : Exception
    {
        public string Name { get; }

        public DatabaseNotFoundException(string name)
            : base($"Database {name} does not exist")
        {
            Name = name;
        }
    }

    public class DatabaseVersionMismatchException : Exception
    {
        public long CurrentVersion { get; }
        public long RequiredVersion { get; }

        public DatabaseVersionMismatchException(long currentVersion, long requiredVersion)
            : base("Database does not match the required version")
        {
            CurrentVersion = currentVersion;
            RequiredVersion = requiredVersion;
        }
    }
}
