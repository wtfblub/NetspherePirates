using BlubLib.Serialization;
using ProudNet.Serializers;
using System.Net;

namespace ProudNet.Message
{
    internal class ReliablePingMessage : ProudMessage
    {
        [Serialize(0)]
        public int RecentFrameRate { get; set; }
    }

    internal class P2PGroup_MemberJoin_AckMessage : ProudMessage
    {
        [Serialize(0)]
        public uint GroupHostId { get; set; }

        [Serialize(1)]
        public uint AddedMemberHostId { get; set; }

        [Serialize(2)]
        public uint EventId { get; set; }

        [Serialize(3)]
        public bool LocalPortReuseSuccess { get; set; }
    }

    internal class NotifyP2PHolepunchSuccessMessage : ProudMessage
    {
        [Serialize(0)]
        public uint A { get; set; }

        [Serialize(1)]
        public uint B { get; set; }

        [Serialize(2, Compiler = typeof(IPEndPointSerializer))]
        public IPEndPoint ABSendAddr { get; set; }

        [Serialize(3, Compiler = typeof(IPEndPointSerializer))]
        public IPEndPoint ABRecvAddr { get; set; }

        [Serialize(4, Compiler = typeof(IPEndPointSerializer))]
        public IPEndPoint BASendAddr { get; set; }

        [Serialize(5, Compiler = typeof(IPEndPointSerializer))]
        public IPEndPoint BARecvAddr { get; set; }

        public NotifyP2PHolepunchSuccessMessage()
        {
            ABSendAddr = new IPEndPoint(0, 0);
            ABRecvAddr = ABRecvAddr;
            BASendAddr = ABRecvAddr;
            BARecvAddr = ABRecvAddr;
        }
    }

    internal class ShutdownTcpMessage : ProudMessage
    {
        [Serialize(0)]
        public short Unk { get; set; }
    }

    internal class NotifyLogMessage : ProudMessage
    {
        [Serialize(0, Compiler = typeof(EnumSerializer))]
        public TraceId TraceId { get; set; }

        [Serialize(1, Compiler = typeof(StringSerializer))]
        public string Message { get; set; }
    }

    internal class NotifyJitDirectP2PTriggeredMessage : ProudMessage
    {
        [Serialize(0)]
        public uint HostId { get; set; }
    }

    internal class NotifyNatDeviceNameDetectedMessage : ProudMessage
    {
        [Serialize(0, Compiler = typeof(StringSerializer))]
        public string Name { get; set; }
    }

    internal class C2S_RequestCreateUdpSocketMessage : ProudMessage
    { }

    internal class C2S_CreateUdpSocketAckMessage : ProudMessage
    {
        [Serialize(0)]
        public bool Success { get; set; }
    }

    internal class ReportC2SUdpMessageTrialCountMessage : ProudMessage
    {
        [Serialize(0)]
        public int TrialCount { get; set; }
    }
}
