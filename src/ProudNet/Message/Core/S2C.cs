using System;
using System.Net;
using ProudNet.Data;
using BlubLib.Serialization;
using ProudNet.Serializers;

namespace ProudNet.Message.Core
{
    internal class ConnectServerTimedoutMessage : CoreMessage
    { }

    internal class NotifyServerConnectionHintMessage : CoreMessage
    {
        [Serialize(0)]
        public ProudConfig Config { get; set; }

        public NotifyServerConnectionHintMessage()
        {
            Config = new ProudConfig();
        }

        public NotifyServerConnectionHintMessage(ProudConfig config)
        {
            Config = config;
        }
    }

    internal class NotifyCSSessionKeySuccessMessage : CoreMessage
    {
        [Serialize(0, Compiler = typeof(ArrayWithScalarSerializer))]
        public byte[] Key { get; set; }

        public NotifyCSSessionKeySuccessMessage()
        { }

        public NotifyCSSessionKeySuccessMessage(byte[] key)
        {
            Key = key;
        }
    }

    internal class NotifyProtocolVersionMismatchMessage : CoreMessage
    { }

    internal class NotifyServerDeniedConnectionMessage : CoreMessage
    {
        [Serialize(0)]
        public ushort Unk { get; set; }
    }

    internal class NotifyServerConnectSuccessMessage : CoreMessage
    {
        [Serialize(0)]
        public uint HostId { get; set; }

        [Serialize(1, Compiler = typeof(GuidSerializer))]
        public Guid Version { get; set; }

        [Serialize(2, Compiler = typeof(ArrayWithScalarSerializer))]
        public byte[] UserData { get; set; }

        [Serialize(3, Compiler = typeof(IPEndPointSerializer))]
        public IPEndPoint EndPoint { get; set; }

        public NotifyServerConnectSuccessMessage()
        {
            Version = Guid.Empty;
            UserData = Array.Empty<byte>();
            EndPoint = new IPEndPoint(0, 0);
        }

        public NotifyServerConnectSuccessMessage(uint hostId, Guid version, IPEndPoint endPoint)
            : this()
        {
            HostId = hostId;
            Version = version;
            EndPoint = endPoint;
        }
    }

    internal class RequestStartServerHolepunchMessage : CoreMessage
    {
        [Serialize(0, Compiler = typeof(GuidSerializer))]
        public Guid MagicNumber { get; set; }

        public RequestStartServerHolepunchMessage()
        {
            MagicNumber = Guid.Empty;
        }

        public RequestStartServerHolepunchMessage(Guid magicNumber)
        {
            MagicNumber = magicNumber;
        }
    }

    internal class ServerHolepunchAckMessage : CoreMessage
    {
        [Serialize(0, Compiler = typeof(GuidSerializer))]
        public Guid MagicNumber { get; set; }

        [Serialize(1, Compiler = typeof(IPEndPointSerializer))]
        public IPEndPoint EndPoint { get; set; }

        public ServerHolepunchAckMessage()
        {
            MagicNumber = Guid.Empty;
            EndPoint = new IPEndPoint(0, 0);
        }

        public ServerHolepunchAckMessage(Guid magicNumber, IPEndPoint endPoint)
        {
            MagicNumber = magicNumber;
            EndPoint = endPoint;
        }
    }

    internal class NotifyClientServerUdpMatchedMessage : CoreMessage
    {
        [Serialize(1, Compiler = typeof(GuidSerializer))]
        public Guid MagicNumber { get; set; }

        public NotifyClientServerUdpMatchedMessage()
        {
            MagicNumber = Guid.Empty;
        }

        public NotifyClientServerUdpMatchedMessage(Guid magicNumber)
        {
            MagicNumber = magicNumber;
        }
    }

    internal class PeerUdp_ServerHolepunchAckMessage : CoreMessage
    {
        [Serialize(0, Compiler = typeof(GuidSerializer))]
        public Guid MagicNumber { get; set; }

        [Serialize(1, Compiler = typeof(IPEndPointSerializer))]
        public IPEndPoint EndPoint { get; set; }

        [Serialize(2)]
        public uint HostId { get; set; }

        public PeerUdp_ServerHolepunchAckMessage()
        {
            MagicNumber = Guid.Empty;
            EndPoint = new IPEndPoint(0, 0);
        }

        public PeerUdp_ServerHolepunchAckMessage(Guid magicNumber, IPEndPoint endPoint, uint hostId)
        {
            MagicNumber = magicNumber;
            EndPoint = endPoint;
            HostId = hostId;
        }
    }

    internal class UnreliablePongMessage : CoreMessage
    {
        [Serialize(0)]
        public double ClientTime { get; set; }

        [Serialize(1)]
        public double ServerTime { get; set; }

        public UnreliablePongMessage()
        { }

        public UnreliablePongMessage(double clientTime, double serverTime)
        {
            ClientTime = clientTime;
            ServerTime = serverTime;
        }
    }

    internal class ReliableRelay2Message : CoreMessage
    {
        [Serialize(0)]
        public RelayDestinationDto Destination { get; set; }

        [Serialize(1, Compiler = typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; }

        public ReliableRelay2Message()
        {
            Destination = new RelayDestinationDto();
        }

        public ReliableRelay2Message(RelayDestinationDto destination, byte[] data)
        {
            Destination = destination;
            Data = data;
        }
    }

    internal class UnreliableRelay2Message : CoreMessage
    {
        [Serialize(0)]
        public uint HostId { get; set; }

        [Serialize(1, Compiler = typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; }

        public UnreliableRelay2Message()
        { }

        public UnreliableRelay2Message(uint hostId, byte[] data)
        {
            HostId = hostId;
            Data = data;
        }
    }
}
