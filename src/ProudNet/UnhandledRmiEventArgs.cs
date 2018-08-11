namespace ProudNet
{
    public class UnhandledRmiEventArgs
    {
        public ProudSession Session { get; }
        public object Message { get; }

        public UnhandledRmiEventArgs(ProudSession session, object message)
        {
            Session = session;
            Message = message;
        }
    }
}
