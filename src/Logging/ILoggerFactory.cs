namespace Logging
{
    public interface ILoggerFactory
    {
        ILogger CreateLogger();

        ILogger<T> CreateLogger<T>();
    }

    public class LoggerFactory : ILoggerFactory
    {
        public ILogger CreateLogger()
        {
            return new Logger();
        }

        public ILogger<T> CreateLogger<T>()
        {
            return new Logger<T>();
        }
    }
}
