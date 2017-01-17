using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ProudNet.Server
{
    internal class RemotePeer : IRemotePeer
    {
        public IP2PGroup Group { get; }
        public uint HostId { get; }
        internal Crypt Crypt { get; }
        internal ConcurrentDictionary<uint, P2PConnectionState> ConnectionStates { get; }
        internal ProudSession Session { get; }

        public RemotePeer(IP2PGroup group, ProudSession session, Crypt crypt)
        {
            Group = group;
            HostId = session.HostId;
            Crypt = crypt;
            ConnectionStates = new ConcurrentDictionary<uint, P2PConnectionState>();
            Session = session;
        }

        public Task SendAsync(object message) => Session.SendAsync(message);
    }
}
