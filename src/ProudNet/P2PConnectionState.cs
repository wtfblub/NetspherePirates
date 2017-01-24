using System;

namespace ProudNet
{
    internal class P2PConnectionState
    {
        public RemotePeer RemotePeer { get; }
        public uint EventId { get; }
        public bool IsJoined { get; set; }
        public bool JitTriggered { get; set; }
        public bool HolepunchSuccess { get; set; }

        public P2PConnectionState(RemotePeer remotePeer)
        {
            RemotePeer = remotePeer;
            EventId = (uint)Guid.NewGuid().GetHashCode();
        }
    }
}
