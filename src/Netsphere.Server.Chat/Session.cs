using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using ProudNet;

namespace Netsphere.Server.Chat
{
    public class Session : ProudSession
    {
        public ulong AccountId { get; set; }
        public string Nickname { get; set; }
        public Mailbox Mailbox { get; set; }
        public DenyManager Ignore { get; set; }
        public PlayerSettingManager Settings { get; set; }

        public Session(ILogger logger, uint hostId, IChannel channel)
            : base(logger, hostId, channel)
        {
        }
    }

    internal class SessionFactory : ISessionFactory
    {
        public ProudSession Create(ILogger logger, uint hostId, IChannel channel)
        {
            return new Session(logger, hostId, channel);
        }
    }
}
