using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Netsphere.Common;

namespace Netsphere.Server.Relay
{
    public class Player
    {
        private readonly ILogger _logger;
        private IDisposable _scope;

        public Session Session { get; private set; }
        public Account Account { get; private set; }

        public event EventHandler<PlayerEventArgs> Disconnected;

        internal void OnDisconnected()
        {
            Disconnected?.Invoke(this, new PlayerEventArgs(this));
        }

        public Player(ILogger<Player> logger)
        {
            _logger = logger;
        }

        internal async Task Initialize(Session session, Account account)
        {
            Session = session;
            Account = account;
            _scope = AddContextToLogger(_logger);
        }

        public void Disconnect()
        {
            var _ = DisconnectAsync();
        }

        public Task DisconnectAsync()
        {
            return Session.CloseAsync();
        }

        public IDisposable AddContextToLogger(ILogger logger)
        {
            return logger.BeginScope("AccountId={AccountId} HostId={HostId} EndPoint={EndPoint}",
                Account.Id, Session.HostId, Session.RemoteEndPoint);
        }
    }
}
