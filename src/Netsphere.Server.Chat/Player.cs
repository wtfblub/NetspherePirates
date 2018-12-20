using System;
using System.Threading.Tasks;
using Logging;
using Netsphere.Common;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;

namespace Netsphere.Server.Chat
{
    public class Player : ISaveable
    {
        public Session Session { get; private set; }
        public Account Account { get; private set; }
        public Mailbox Mailbox { get; }
        public DenyManager Ignore { get; }
        public PlayerSettingManager Settings { get; }
        public uint TotalExperience { get; internal set; }
        public Channel Channel { get; internal set; }
        public uint RoomId { get; internal set; }
        public TeamId TeamId { get; internal set; }
        internal bool SentPlayerList { get; set; }

        public event EventHandler<PlayerEventArgs> Disconnected;

        internal void OnDisconnected()
        {
            Channel?.Leave(this);

            Disconnected?.Invoke(this, new PlayerEventArgs(this));
        }

        public Player(Mailbox mailbox, DenyManager denyManager, PlayerSettingManager settings)
        {
            Mailbox = mailbox;
            Ignore = denyManager;
            Settings = settings;
        }

        internal async Task Initialize(Session session, Account account, PlayerEntity entity)
        {
            Session = session;
            Account = account;
            TotalExperience = (uint)entity.TotalExperience;
            await Mailbox.Initialize(this, entity);
            await Ignore.Initialize(this, entity);
            Settings.Initialize(this, entity);
        }

        public void Disconnect()
        {
            var _ = DisconnectAsync();
        }

        public Task DisconnectAsync()
        {
            return Session.CloseAsync();
        }

        public async Task Save(GameContext db)
        {
            await Mailbox.Save(db);
            await Ignore.Save(db);
            await Settings.Save(db);
        }

        public ILogger AddContextToLogger(ILogger logger)
        {
            return logger.ForContext(
                ("AccountId", Account.Id),
                ("HostId", Session.HostId),
                ("EndPoint", Session.RemoteEndPoint)
            );
        }
    }
}
