using System;
using System.Net;

namespace ProudNet
{
    internal class P2PConnectionState
    {
        public IP2PMemberInternal RemotePeer { get; }
        public uint EventId { get; }
        public bool IsInitialized { get; set; }
        public bool IsJoined { get; set; }
        public bool JitTriggered { get; set; }
        public bool PeerUdpHolepunchSuccess { get; set; }
        public bool HolepunchSuccess { get; set; }
        public DateTimeOffset LastHolepunch { get; set; }

        public IPEndPoint EndPoint { get; set; }
        public IPEndPoint LocalEndPoint { get; set; }

        public P2PConnectionState(IP2PMemberInternal remotePeer)
        {
            RemotePeer = remotePeer;
            EventId = (uint)Guid.NewGuid().GetHashCode();
        }
    }
}
