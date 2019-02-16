using System;
using System.Net;
using BlubLib.Serialization;
using ProudNet.Serialization.Serializers;

namespace ProudNet.Serialization.Messages.Core
{
    [BlubContract]
    internal class     NotifyCSEncryptedSessionKeyMessage : ICoreMessage
    {
        [BlubMember(0)]
        public byte[] SecureKey { get; set; }

        [BlubMember(1)]
        public byte[] FastKey { get; set; }
    }

    [BlubContract]
    internal class NotifyServerConnectionRequestDataMessage : ICoreMessage
    {
        [BlubMember(0)]
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
    internal class ServerHolepunchMessage : ICoreMessage
    {
        [BlubMember(0)]
        public Guid MagicNumber { get; set; }

        public ServerHolepunchMessage()
        {
            MagicNumber = Guid.Empty;
        }
    }

    [BlubContract]
    internal class NotifyHolepunchSuccessMessage : ICoreMessage
    {
        [BlubMember(0)]
        public Guid MagicNumber { get; set; }

        [BlubMember(1)]
        public IPEndPoint LocalEndPoint { get; set; }

        [BlubMember(2)]
        public IPEndPoint EndPoint { get; set; }

        public NotifyHolepunchSuccessMessage()
        {
            MagicNumber = Guid.Empty;
            LocalEndPoint = new IPEndPoint(0, 0);
            EndPoint = LocalEndPoint;
        }
    }

    [BlubContract]
    internal class PeerUdp_ServerHolepunchMessage : ICoreMessage
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
    internal class PeerUdp_NotifyHolepunchSuccessMessage : ICoreMessage
    {
        [BlubMember(0)]
        public IPEndPoint LocalEndPoint { get; set; }

        [BlubMember(1)]
        public IPEndPoint EndPoint { get; set; }

        [BlubMember(2)]
        public uint HostId { get; set; }
    }

    [BlubContract]
    internal class UnreliablePingMessage : ICoreMessage
    {
        [BlubMember(0)]
        public double ClientTime { get; set; }

        [BlubMember(1)]
        public double Ping { get; set; }
    }

    [BlubContract]
    internal class SpeedHackDetectorPingMessage : ICoreMessage
    {
    }

    [BlubContract]
    internal class ReliableRelay1Message : ICoreMessage
    {
        [BlubMember(0)]
        public RelayDestinationDto[] Destination { get; set; }

        [BlubMember(1)]
        public byte[] Data { get; set; }

        public ReliableRelay1Message()
        {
            Destination = Array.Empty<RelayDestinationDto>();
            Data = Array.Empty<byte>();
        }
    }

    [BlubContract]
    internal class UnreliableRelay1Message : ICoreMessage
    {
        [BlubMember(0)]
        public MessagePriority Priority { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ScalarSerializer))]
        public int UniqueId { get; set; }

        [BlubMember(2)]
        public uint[] Destination { get; set; }

        [BlubMember(3)]
        public byte[] Data { get; set; }

        public UnreliableRelay1Message()
        {
            Destination = Array.Empty<uint>();
            Data = Array.Empty<byte>();
        }
    }

    [BlubContract]
    internal class UnreliableRelay1_RelayDestListCompressedMessage : ICoreMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public byte[] Data { get; set; }

        public UnreliableRelay1_RelayDestListCompressedMessage()
        {
            Data = Array.Empty<byte>();
        }
    }
}
