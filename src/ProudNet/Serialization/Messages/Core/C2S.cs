using System;
using System.IO;
using System.Net;
using BlubLib.Serialization;
using ProudNet.Serialization.Serializers;

namespace ProudNet.Serialization.Messages.Core
{
    [BlubContract]
    internal class NotifyCSEncryptedSessionKeyMessage
    {
        [BlubMember(0, typeof(ArrayWithScalarSerializer))]
        public byte[] Key { get; set; }
    }

    [BlubContract]
    internal class NotifyServerConnectionRequestDataMessage
    {
        [BlubMember(0, typeof(ArrayWithScalarSerializer))]
        public byte[] UserData { get; set; }

        [BlubMember(1)]
        public Guid Version { get; set; }

        [BlubMember(2)]
        public uint InternalNetVersion { get; set; }

        public NotifyServerConnectionRequestDataMessage()
        {
            Version = Guid.Empty;
            InternalNetVersion = Constants.NetVersion;
        }
    }

    [BlubContract]
    internal class ServerHolepunchMessage
    {
        [BlubMember(0)]
        public Guid MagicNumber { get; set; }

        public ServerHolepunchMessage()
        {
            MagicNumber = Guid.Empty;
        }
    }

    [BlubContract]
    internal class NotifyHolepunchSuccessMessage
    {
        [BlubMember(0)]
        public Guid MagicNumber { get; set; }

        [BlubMember(1, typeof(IPEndPointSerializer))]
        public IPEndPoint LocalEndPoint { get; set; }

        [BlubMember(2, typeof(IPEndPointSerializer))]
        public IPEndPoint EndPoint { get; set; }

        public NotifyHolepunchSuccessMessage()
        {
            MagicNumber = Guid.Empty;
            LocalEndPoint = new IPEndPoint(0, 0);
            EndPoint = LocalEndPoint;
        }
    }

    [BlubContract]
    internal class PeerUdp_ServerHolepunchMessage
    {
        [BlubMember(0)]
        public Guid MagicNumber { get; set; }

        [BlubMember(1)]
        public uint HostId { get; set; }

        public PeerUdp_ServerHolepunchMessage()
        {
            MagicNumber = Guid.Empty;
        }
    }

    [BlubContract]
    internal class PeerUdp_NotifyHolepunchSuccessMessage
    {
        [BlubMember(0, typeof(IPEndPointSerializer))]
        public IPEndPoint LocalEndPoint { get; set; }

        [BlubMember(1, typeof(IPEndPointSerializer))]
        public IPEndPoint EndPoint { get; set; }

        [BlubMember(2)]
        public uint HostId { get; set; }

        public PeerUdp_NotifyHolepunchSuccessMessage()
        {
            LocalEndPoint = new IPEndPoint(0, 0);
            EndPoint = new IPEndPoint(0, 0);
        }
    }

    [BlubContract]
    internal class UnreliablePingMessage
    {
        [BlubMember(0)]
        public double ClientTime { get; set; }

        [BlubMember(1)]
        public double Ping { get; set; }
    }

    [BlubContract]
    internal class SpeedHackDetectorPingMessage
    { }

    [BlubContract]
    internal class ReliableRelay1Message
    {
        [BlubMember(0, typeof(ArrayWithScalarSerializer))]
        public RelayDestinationDto[] Destination { get; set; }

        [BlubMember(1, typeof(StreamWithScalarSerializer))]
        public Stream Data { get; set; }

        public ReliableRelay1Message()
        {
            Destination = Array.Empty<RelayDestinationDto>();
        }
    }

    [BlubContract]
    internal class UnreliableRelay1Message
    {
        [BlubMember(0)]
        public MessagePriority Priority { get; set; }

        [BlubMember(1, typeof(ScalarSerializer))]
        public int UniqueId { get; set; }

        [BlubMember(2, typeof(ArrayWithScalarSerializer))]
        public uint[] Destination { get; set; }

        [BlubMember(3, typeof(StreamWithScalarSerializer))]
        public Stream Data { get; set; }

        public UnreliableRelay1Message()
        {
            Destination = Array.Empty<uint>();
        }
    }
}
