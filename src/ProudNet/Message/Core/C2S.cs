using System;
using System.Net;
using ProudNet.Data;
using BlubLib.Serialization;
using ProudNet.Serializers;

namespace ProudNet.Message.Core
{
    internal class NotifyCSEncryptedSessionKeyMessage : CoreMessage
    {
        [Serialize(0, Compiler = typeof(ArrayWithScalarSerializer))]
        public byte[] Key { get; set; }
    }

    internal class NotifyServerConnectionRequestDataMessage : CoreMessage
    {
        [Serialize(0, Compiler = typeof(ArrayWithScalarSerializer))]
        public byte[] UserData { get; set; }

        [Serialize(1, Compiler = typeof(GuidSerializer))]
        public Guid Version { get; set; }

        [Serialize(2)]
        public uint InternalNetVersion { get; set; }

        public NotifyServerConnectionRequestDataMessage()
        {
            Version = Guid.Empty;
            InternalNetVersion = ProudConfig.InternalNetVersion;
        }
    }

    internal class ServerHolepunchMessage : CoreMessage
    {
        [Serialize(0, Compiler = typeof(GuidSerializer))]
        public Guid MagicNumber { get; set; }

        public ServerHolepunchMessage()
        {
            MagicNumber = Guid.Empty;
        }
    }

    internal class NotifyHolepunchSuccessMessage : CoreMessage
    {
        [Serialize(0, Compiler = typeof(GuidSerializer))]
        public Guid MagicNumber { get; set; }

        [Serialize(1, Compiler = typeof(IPEndPointSerializer))]
        public IPEndPoint LocalEndPoint { get; set; }

        [Serialize(2, Compiler = typeof(IPEndPointSerializer))]
        public IPEndPoint EndPoint { get; set; }

        public NotifyHolepunchSuccessMessage()
        {
            MagicNumber = Guid.Empty;
            LocalEndPoint = new IPEndPoint(0, 0);
            EndPoint = LocalEndPoint;
        }
    }

    internal class PeerUdp_ServerHolepunchMessage : CoreMessage
    {
        [Serialize(0, Compiler = typeof(GuidSerializer))]
        public Guid MagicNumber { get; set; }

        [Serialize(1)]
        public uint HostId { get; set; }

        public PeerUdp_ServerHolepunchMessage()
        {
            MagicNumber = Guid.Empty;
        }
    }

    internal class PeerUdp_NotifyHolepunchSuccessMessage : CoreMessage
    {
        [Serialize(0, Compiler = typeof(IPEndPointSerializer))]
        public IPEndPoint LocalEndPoint { get; set; }

        [Serialize(1, Compiler = typeof(IPEndPointSerializer))]
        public IPEndPoint EndPoint { get; set; }

        [Serialize(2, Compiler = typeof(GuidSerializer))]
        public uint HostId { get; set; }

        public PeerUdp_NotifyHolepunchSuccessMessage()
        {
            LocalEndPoint = new IPEndPoint(0, 0);
            EndPoint = new IPEndPoint(0, 0);
        }
    }

    internal class UnreliablePingMessage : CoreMessage
    {
        [Serialize(0)]
        public double ClientTime { get; set; }

        [Serialize(1)]
        public double Ping { get; set; }
    }

    internal class SpeedHackDetectorPingMessage : CoreMessage
    { }

    internal class ReliableRelay1Message : CoreMessage
    {
        [Serialize(0, Compiler = typeof(ArrayWithScalarSerializer))]
        public RelayDestinationDto[] Destination { get; set; }

        [Serialize(1, Compiler = typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; }

        public ReliableRelay1Message()
        {
            Destination = Array.Empty<RelayDestinationDto>();
        }
    }

    internal class UnreliableRelay1Message : CoreMessage
    {
        [Serialize(0, Compiler = typeof(EnumSerializer))]
        public MessagePriority Priority { get; set; }

        [Serialize(1, Compiler = typeof(ScalarSerializer))]
        public int UniqueId { get; set; }

        [Serialize(2, Compiler = typeof(ArrayWithScalarSerializer))]
        public uint[] Destination { get; set; }

        [Serialize(3, Compiler = typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; }

        public UnreliableRelay1Message()
        {
            Destination = Array.Empty<uint>();
        }
    }
}
