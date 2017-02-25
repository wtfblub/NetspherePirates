using System;

namespace ProudNet
{
    public class ErrorEventArgs : EventArgs
    {
        public ProudSession Session { get; }
        public Exception Exception { get; }

        public ErrorEventArgs(Exception exception)
            : this(null, exception)
        { }

        public ErrorEventArgs(ProudSession session, Exception exception)
        {
            Session = session;
            Exception = exception;
        }
    }
}
