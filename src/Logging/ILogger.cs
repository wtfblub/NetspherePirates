using System;
using Serilog;

namespace Logging
{
    // Methods taken from Serilog.ILogger

    public interface ILogger
    {
        ILogger ForContext(params (string propertyName, object value)[] context);

        ILogger ForContext(string propertyName, object value, bool destructureObjects = false);

        ILogger ForContext<TSource>();

        ILogger ForContext(Type source);

        #region Verbose
        void Verbose(string messageTemplate);

        void Verbose<T1>(string messageTemplate, T1 propertyValue);

        void Verbose<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1);

        void Verbose<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2);

        void Verbose(string messageTemplate, params object[] propertyValues);

        void Verbose(Exception exception, string messageTemplate);

        void Verbose<T1>(Exception exception, string messageTemplate, T1 propertyValue);

        void Verbose<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1);

        void Verbose<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2);

        void Verbose(Exception exception, string messageTemplate, params object[] propertyValues);
        #endregion

        #region Debug
        void Debug(string messageTemplate);

        void Debug<T1>(string messageTemplate, T1 propertyValue);

        void Debug<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1);

        void Debug<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2);

        void Debug(string messageTemplate, params object[] propertyValues);

        void Debug(Exception exception, string messageTemplate);

        void Debug<T1>(Exception exception, string messageTemplate, T1 propertyValue);

        void Debug<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1);

        void Debug<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2);

        void Debug(Exception exception, string messageTemplate, params object[] propertyValues);
        #endregion

        #region Information
        void Information(string messageTemplate);

        void Information<T1>(string messageTemplate, T1 propertyValue);

        void Information<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1);

        void Information<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2);

        void Information(string messageTemplate, params object[] propertyValues);

        void Information(Exception exception, string messageTemplate);

        void Information<T1>(Exception exception, string messageTemplate, T1 propertyValue);

        void Information<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1);

        void Information<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2);

        void Information(Exception exception, string messageTemplate, params object[] propertyValues);
        #endregion

        #region Warning
        void Warning(string messageTemplate);

        void Warning<T1>(string messageTemplate, T1 propertyValue);

        void Warning<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1);

        void Warning<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2);

        void Warning(string messageTemplate, params object[] propertyValues);

        void Warning(Exception exception, string messageTemplate);

        void Warning<T1>(Exception exception, string messageTemplate, T1 propertyValue);

        void Warning<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1);

        void Warning<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2);

        void Warning(Exception exception, string messageTemplate, params object[] propertyValues);
        #endregion

        #region Error
        void Error(string messageTemplate);

        void Error<T1>(string messageTemplate, T1 propertyValue);

        void Error<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1);

        void Error<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2);

        void Error(string messageTemplate, params object[] propertyValues);

        void Error(Exception exception, string messageTemplate);

        void Error<T1>(Exception exception, string messageTemplate, T1 propertyValue);

        void Error<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1);

        void Error<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2);

        void Error(Exception exception, string messageTemplate, params object[] propertyValues);
        #endregion
    }

    // ReSharper disable once UnusedTypeParameter
    public interface ILogger<T> : ILogger
    {
    }

    public class Logger : ILogger
    {
        protected readonly Serilog.ILogger _logger;

        public Logger()
            : this(Log.Logger)
        {
        }

        public Logger(Serilog.ILogger logger)
        {
            _logger = logger;
        }

        public ILogger ForContext(params (string propertyName, object value)[] context)
        {
            var logger = _logger;
            foreach (var (propertyName, value) in context)
                logger = logger.ForContext(propertyName, value);

            return new Logger(logger);
        }

        public ILogger ForContext(string propertyName, object value, bool destructureObjects = false)
        {
            return new Logger(_logger.ForContext(propertyName, value, destructureObjects));
        }

        public ILogger ForContext<TSource>()
        {
            return new Logger(_logger.ForContext<TSource>());
        }

        public ILogger ForContext(Type source)
        {
            return new Logger(_logger.ForContext(source));
        }

        #region Verbose
        public void Verbose(string messageTemplate)
        {
            _logger.Verbose(messageTemplate);
        }

        public void Verbose<T1>(string messageTemplate, T1 propertyValue)
        {
            _logger.Verbose(messageTemplate, propertyValue);
        }

        public void Verbose<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            _logger.Verbose(messageTemplate, propertyValue0, propertyValue1);
        }

        public void Verbose<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            _logger.Verbose(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Verbose(string messageTemplate, params object[] propertyValues)
        {
            _logger.Verbose(messageTemplate, propertyValues);
        }

        public void Verbose(Exception exception, string messageTemplate)
        {
            _logger.Verbose(exception, messageTemplate);
        }

        public void Verbose<T1>(Exception exception, string messageTemplate, T1 propertyValue)
        {
            _logger.Verbose(exception, messageTemplate, propertyValue);
        }

        public void Verbose<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            _logger.Verbose(exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Verbose<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2)
        {
            _logger.Verbose(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Verbose(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Verbose(exception, messageTemplate, propertyValues);
        }
        #endregion

        #region Debug
        public void Debug(string messageTemplate)
        {
            _logger.Debug(messageTemplate);
        }

        public void Debug<T1>(string messageTemplate, T1 propertyValue)
        {
            _logger.Debug(messageTemplate, propertyValue);
        }

        public void Debug<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            _logger.Debug(messageTemplate, propertyValue0, propertyValue1);
        }

        public void Debug<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            _logger.Debug(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Debug(string messageTemplate, params object[] propertyValues)
        {
            _logger.Debug(messageTemplate, propertyValues);
        }

        public void Debug(Exception exception, string messageTemplate)
        {
            _logger.Debug(exception, messageTemplate);
        }

        public void Debug<T1>(Exception exception, string messageTemplate, T1 propertyValue)
        {
            _logger.Debug(exception, messageTemplate, propertyValue);
        }

        public void Debug<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            _logger.Debug(exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Debug<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2)
        {
            _logger.Debug(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Debug(exception, messageTemplate, propertyValues);
        }
        #endregion

        #region Information
        public void Information(string messageTemplate)
        {
            _logger.Information(messageTemplate);
        }

        public void Information<T1>(string messageTemplate, T1 propertyValue)
        {
            _logger.Information(messageTemplate, propertyValue);
        }

        public void Information<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            _logger.Information(messageTemplate, propertyValue0, propertyValue1);
        }

        public void Information<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            _logger.Information(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Information(string messageTemplate, params object[] propertyValues)
        {
            _logger.Information(messageTemplate, propertyValues);
        }

        public void Information(Exception exception, string messageTemplate)
        {
            _logger.Information(exception, messageTemplate);
        }

        public void Information<T1>(Exception exception, string messageTemplate, T1 propertyValue)
        {
            _logger.Information(exception, messageTemplate, propertyValue);
        }

        public void Information<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            _logger.Information(exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Information<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2)
        {
            _logger.Information(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Information(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Information(exception, messageTemplate, propertyValues);
        }
        #endregion

        #region Warning
        public void Warning(string messageTemplate)
        {
            _logger.Warning(messageTemplate);
        }

        public void Warning<T1>(string messageTemplate, T1 propertyValue)
        {
            _logger.Warning(messageTemplate, propertyValue);
        }

        public void Warning<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            _logger.Warning(messageTemplate, propertyValue0, propertyValue1);
        }

        public void Warning<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            _logger.Warning(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Warning(string messageTemplate, params object[] propertyValues)
        {
            _logger.Warning(messageTemplate, propertyValues);
        }

        public void Warning(Exception exception, string messageTemplate)
        {
            _logger.Warning(exception, messageTemplate);
        }

        public void Warning<T1>(Exception exception, string messageTemplate, T1 propertyValue)
        {
            _logger.Warning(exception, messageTemplate, propertyValue);
        }

        public void Warning<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            _logger.Warning(exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Warning<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2)
        {
            _logger.Warning(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Warning(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Warning(exception, messageTemplate, propertyValues);
        }
        #endregion

        #region Error
        public void Error(string messageTemplate)
        {
            _logger.Error(messageTemplate);
        }

        public void Error<T1>(string messageTemplate, T1 propertyValue)
        {
            _logger.Error(messageTemplate, propertyValue);
        }

        public void Error<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            _logger.Error(messageTemplate, propertyValue0, propertyValue1);
        }

        public void Error<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            _logger.Error(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Error(string messageTemplate, params object[] propertyValues)
        {
            _logger.Error(messageTemplate, propertyValues);
        }

        public void Error(Exception exception, string messageTemplate)
        {
            _logger.Error(exception, messageTemplate);
        }

        public void Error<T1>(Exception exception, string messageTemplate, T1 propertyValue)
        {
            _logger.Error(exception, messageTemplate, propertyValue);
        }

        public void Error<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            _logger.Error(exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Error<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2)
        {
            _logger.Error(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Error(exception, messageTemplate, propertyValues);
        }
        #endregion
    }

    public class Logger<T> : Logger, ILogger<T>
    {
        public Logger()
            : base(Log.ForContext<T>())
        {
        }
    }
}
