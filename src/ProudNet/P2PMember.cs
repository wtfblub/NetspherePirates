using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ProudNet
{
    internal class ServerP2PMember : IP2PMemberInternal
    {
        public P2PGroup P2PGroup { get; }
        public uint HostId { get; }
        public Crypt Crypt { get; set; }
        public ConcurrentDictionary<uint, P2PConnectionState> ConnectionStates => throw new NotSupportedException();

        internal ServerP2PMember(P2PGroup group, Crypt crypt)
        {
            P2PGroup = group;
            HostId = (uint)ProudNet.HostId.Server;
            Crypt = crypt;
        }

        public virtual Task SendAsync(object message)
        {
            // TODO Inject into pipeline
            throw new NotImplementedException();
        }
    }

    public interface IP2PMember
    {
        P2PGroup P2PGroup { get; }
        uint HostId { get; }

        Task SendAsync(object message);
    }

    internal interface IP2PMemberInternal : IP2PMember
    {
        Crypt Crypt { get; set; }
        ConcurrentDictionary<uint, P2PConnectionState> ConnectionStates { get; }
    }
}
