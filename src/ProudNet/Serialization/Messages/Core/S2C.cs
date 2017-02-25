﻿using System;
using System.IO;
using System.Net;
using BlubLib.Serialization;
using ProudNet.Serialization.Serializers;

namespace ProudNet.Serialization.Messages.Core
{
    [BlubContract]
    internal class ConnectServerTimedoutMessage : ICoreMessage
    { }

    [BlubContract]
    internal class NotifyServerConnectionHintMessage : ICoreMessage
    {
        [BlubMember(0)]
        public NetConfigDto Config { get; set; }

        public NotifyServerConnectionHintMessage()
        {
            Config = new NetConfigDto();
        }

        public NotifyServerConnectionHintMessage(NetConfigDto config)
        {
            Config = config;
        }
    }

    [BlubContract]
    internal class NotifyCSSessionKeySuccessMessage : ICoreMessage
    {
        [BlubMember(0, typeof(ArrayWithScalarSerializer))]
        public byte[] Key { get; set; }

        public NotifyCSSessionKeySuccessMessage()
        { }

        public NotifyCSSessionKeySuccessMessage(byte[] key)
        {
            Key = key;
        }
    }

    [BlubContract]
    internal class NotifyProtocolVersionMismatchMessage : ICoreMessage
    { }

    [BlubContract]
    internal class NotifyServerDeniedConnectionMessage : ICoreMessage
    {
        [BlubMember(0)]
        public ushort Unk { get; set; }
    }

    [BlubContract]
    internal class NotifyServerConnectSuccessMessage : ICoreMessage
    {
        [BlubMember(0)]
        public uint HostId { get; set; }

        [BlubMember(1)]
        public Guid Version { get; set; }

        [BlubMember(2, typeof(ArrayWithScalarSerializer))]
        public byte[] UserData { get; set; }

        [BlubMember(3, typeof(IPEndPointSerializer))]
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

    [BlubContract]
    internal class RequestStartServerHolepunchMessage : ICoreMessage
    {
        [BlubMember(0)]
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

    [BlubContract]
    internal class ServerHolepunchAckMessage : ICoreMessage
    {
        [BlubMember(0)]
        public Guid MagicNumber { get; set; }

        [BlubMember(1, typeof(IPEndPointSerializer))]
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

    [BlubContract]
    internal class NotifyClientServerUdpMatchedMessage : ICoreMessage
    {
        [BlubMember(0)]
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

    [BlubContract]
    internal class PeerUdp_ServerHolepunchAckMessage : ICoreMessage
    {
        [BlubMember(0)]
        public Guid MagicNumber { get; set; }

        [BlubMember(1, typeof(IPEndPointSerializer))]
        public IPEndPoint EndPoint { get; set; }

        [BlubMember(2)]
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

    [BlubContract]
    internal class UnreliablePongMessage : ICoreMessage
    {
        [BlubMember(0)]
        public double ClientTime { get; set; }

        [BlubMember(1)]
        public double ServerTime { get; set; }

        public UnreliablePongMessage()
        { }

        public UnreliablePongMessage(double clientTime, double serverTime)
        {
            ClientTime = clientTime;
            ServerTime = serverTime;
        }
    }

    [BlubContract]
    internal class ReliableRelay2Message : ICoreMessage
    {
        [BlubMember(0)]
        public RelayDestinationDto Destination { get; set; }

        [BlubMember(1, typeof(ArrayWithScalarSerializer))]
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

    [BlubContract]
    internal class UnreliableRelay2Message : ICoreMessage
    {
        [BlubMember(0)]
        public uint HostId { get; set; }

        [BlubMember(1, typeof(ArrayWithScalarSerializer))]
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
