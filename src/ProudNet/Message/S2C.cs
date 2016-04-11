using BlubLib.Serialization;
using ProudNet.Serializers;
using System;
using System.Net;

namespace ProudNet.Message
{
    internal class ReliablePongMessage : ProudMessage
    { }

    internal class ShutdownTcpAckMessage : ProudMessage
    { }

    internal class P2PGroup_MemberJoinMessage : ProudMessage
    {
        [Serialize(0)]
        public uint GroupId { get; set; }

        [Serialize(1)]
        public uint MemberId { get; set; }

        [Serialize(2, typeof(ArrayWithScalarSerializer))]
        public byte[] UserData { get; set; }

        [Serialize(3)]
        public uint EventId { get; set; }

        [Serialize(4, typeof(ArrayWithScalarSerializer))]
        public byte[] SessionKey { get; set; }

        [Serialize(5)]
        public uint P2PFirstFrameNumber { get; set; }

        [Serialize(6, typeof(GuidSerializer))]
        public Guid ConnectionMagicNumber { get; set; }

        [Serialize(7)]
        public bool EnableDirectP2P { get; set; }

        [Serialize(8)]
        public ushort BindPort { get; set; }

        public P2PGroup_MemberJoinMessage()
        {
            UserData = Array.Empty<byte>();
            SessionKey = Array.Empty<byte>();
            ConnectionMagicNumber = Guid.Empty;
        }

        public P2PGroup_MemberJoinMessage(uint groupId, uint memberId, uint eventId, byte[] sessionKey, bool enableDirectP2P)
            : this()
        {
            GroupId = groupId;
            MemberId = memberId;
            EventId = eventId;
            EnableDirectP2P = enableDirectP2P;
            SessionKey = sessionKey;
        }
    }

    internal class P2PGroup_MemberJoin_UnencryptedMessage : ProudMessage
    {
        [Serialize(0)]
        public uint GroupId { get; set; }

        [Serialize(1)]
        public uint MemberId { get; set; }

        [Serialize(2, typeof(ArrayWithScalarSerializer))]
        public byte[] UserData { get; set; }

        [Serialize(3)]
        public uint EventId { get; set; }

        [Serialize(4)]
        public uint P2PFirstFrameNumber { get; set; }

        [Serialize(5, typeof(GuidSerializer))]
        public Guid ConnectionMagicNumber { get; set; }

        [Serialize(6)]
        public bool EnableDirectP2P { get; set; }

        [Serialize(7)]
        public ushort BindPort { get; set; }

        public P2PGroup_MemberJoin_UnencryptedMessage()
        {
            UserData = Array.Empty<byte>();
            ConnectionMagicNumber = Guid.Empty;
        }

        public P2PGroup_MemberJoin_UnencryptedMessage(uint groupId, uint memberId, uint eventId, bool enableDirectP2P)
            : this()
        {
            GroupId = groupId;
            MemberId = memberId;
            EventId = eventId;
            EnableDirectP2P = enableDirectP2P;
        }
    }

    internal class P2PRecycleCompleteMessage : ProudMessage
    {
        [Serialize(0)]
        public uint HostId { get; set; }

        [Serialize(1)]
        public bool Recycled { get; set; }

        [Serialize(2, typeof(IPEndPointSerializer))]
        public IPEndPoint InternalAddress { get; set; }

        [Serialize(3, typeof(IPEndPointSerializer))]
        public IPEndPoint ExternalAddress { get; set; }

        [Serialize(4, typeof(IPEndPointSerializer))]
        public IPEndPoint SendAddress { get; set; }

        [Serialize(5, typeof(IPEndPointSerializer))]
        public IPEndPoint RecvAddress { get; set; }

        public P2PRecycleCompleteMessage()
        {
            InternalAddress = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 65535);
            ExternalAddress = InternalAddress;
            SendAddress = InternalAddress;
            RecvAddress = InternalAddress;
        }

        public P2PRecycleCompleteMessage(uint hostId)
            : this()
        {
            HostId = hostId;
        }
    }

    internal class RequestP2PHolepunchMessage : ProudMessage
    {
        [Serialize(0)]
        public uint HostId { get; set; }

        [Serialize(1, typeof(IPEndPointSerializer))]
        public IPEndPoint LocalEndPoint { get; set; }

        [Serialize(2, typeof(IPEndPointSerializer))]
        public IPEndPoint EndPoint { get; set; }

        public RequestP2PHolepunchMessage()
        {
            LocalEndPoint = new IPEndPoint(0, 0);
            EndPoint = new IPEndPoint(0, 0);
        }

        public RequestP2PHolepunchMessage(uint hostId, IPEndPoint localEndPoint, IPEndPoint endPoint)
        {
            HostId = hostId;
            LocalEndPoint = localEndPoint;
            EndPoint = endPoint;
        }
    }

    internal class P2PGroup_MemberLeaveMessage : ProudMessage
    {
        [Serialize(0)]
        public uint MemberId { get; set; }

        [Serialize(1)]
        public uint GroupId { get; set; }

        public P2PGroup_MemberLeaveMessage()
        {
        }

        public P2PGroup_MemberLeaveMessage(uint memberId, uint groupId)
        {
            MemberId = memberId;
            GroupId = groupId;
        }
    }

    internal class NotifyDirectP2PEstablishMessage : ProudMessage
    {
        [Serialize(0)]
        public uint A { get; set; }

        [Serialize(1)]
        public uint B { get; set; }

        [Serialize(2, typeof(IPEndPointSerializer))]
        public IPEndPoint ABSendAddr { get; set; }

        [Serialize(3, typeof(IPEndPointSerializer))]
        public IPEndPoint ABRecvAddr { get; set; }

        [Serialize(4, typeof(IPEndPointSerializer))]
        public IPEndPoint BASendAddr { get; set; }

        [Serialize(5, typeof(IPEndPointSerializer))]
        public IPEndPoint BARecvAddr { get; set; }

        public NotifyDirectP2PEstablishMessage()
        {
            ABSendAddr = new IPEndPoint(0, 0);
            ABRecvAddr = ABSendAddr;
            BASendAddr = ABSendAddr;
            BARecvAddr = ABSendAddr;
        }

        public NotifyDirectP2PEstablishMessage(uint a, uint b, IPEndPoint abSendAddr, IPEndPoint abRecvAddr, IPEndPoint baSendAddr, IPEndPoint baRecvAddr)
        {
            A = a;
            B = b;
            ABSendAddr = abSendAddr;
            ABRecvAddr = abRecvAddr;
            BASendAddr = baSendAddr;
            BARecvAddr = baRecvAddr;
        }
    }

    internal class NewDirectP2PConnectionMessage : ProudMessage
    {
        [Serialize(0)]
        public uint HostId { get; set; }

        public NewDirectP2PConnectionMessage()
        { }

        public NewDirectP2PConnectionMessage(uint hostId)
        {
            HostId = hostId;
        }
    }

    internal class S2C_RequestCreateUdpSocketMessage : ProudMessage
    {
        [Serialize(0, typeof(IPEndPointAddressStringSerializer))]
        public IPEndPoint EndPoint { get; set; }

        public S2C_RequestCreateUdpSocketMessage()
        { }

        public S2C_RequestCreateUdpSocketMessage(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
        }
    }

    internal class S2C_CreateUdpSocketAckMessage : ProudMessage
    {
        [Serialize(0)]
        public bool Success { get; set; }

        [Serialize(1, typeof(IPEndPointAddressStringSerializer))]
        public IPEndPoint EndPoint { get; set; }

        public S2C_CreateUdpSocketAckMessage()
        { }

        public S2C_CreateUdpSocketAckMessage(bool success, IPEndPoint endPoint)
        {
            Success = success;
            EndPoint = endPoint;
        }
    }
}
