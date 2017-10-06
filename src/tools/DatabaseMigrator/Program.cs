using EntryPoint;

namespace DatabaseMigrator
{
    internal class Program
    {
        private static void Main()
        {
            Cli.Execute<Commands>();
        }
    }
}
