using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProudNet
{
    public interface IP2PGroup
    {
        uint HostId { get; }
        bool AllowDirectP2P { get; }
        IReadOnlyDictionary<uint, IRemotePeer> Members { get; }

        void Join(uint hostId);
        void Leave(uint hostId);
    }

    public interface IRemotePeer
    {
        IP2PGroup Group { get; }
        uint HostId { get; }

        Task SendAsync(object message);
    }

    internal class P2PConnectionState
    {
        public IRemotePeer RemotePeer { get; }
        public uint EventId { get; }
        public bool IsJoined { get; set; }
        public bool JitTriggered { get; set; }
        public bool HolepunchSuccess { get; set; }

        public P2PConnectionState(IRemotePeer remotePeer)
        {
            RemotePeer = remotePeer;
            EventId = (uint)Guid.NewGuid().GetHashCode();
        }
    }
}
