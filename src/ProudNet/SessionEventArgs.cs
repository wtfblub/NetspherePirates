using System;

namespace ProudNet
{
    public class SessionEventArgs : EventArgs
    {
        public ProudSession Session { get; }

        public SessionEventArgs(ProudSession session)
        {
            Session = session;
        }
    }
}
