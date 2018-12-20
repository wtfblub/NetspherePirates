using System;
using System.Threading.Tasks;
using Logging;
using Netsphere.Common;

namespace Netsphere.Server.Relay
{
    public class Player
    {
        public Session Session { get; private set; }
        public Account Account { get; private set; }

        public event EventHandler<PlayerEventArgs> Disconnected;

        internal void OnDisconnected()
        {
            Disconnected?.Invoke(this, new PlayerEventArgs(this));
        }

        internal void Initialize(Session session, Account account)
        {
            Session = session;
            Account = account;
        }

        public void Disconnect()
        {
            var _ = DisconnectAsync();
        }

        public Task DisconnectAsync()
        {
            return Session.CloseAsync();
        }

        public ILogger AddContextToLogger(ILogger logger)
        {
            return logger.ForContext(
                ("AccountId", Account.Id),
                ("HostId", Session.HostId),
                ("EndPoint", Session.RemoteEndPoint.ToString()));
        }
    }
}
