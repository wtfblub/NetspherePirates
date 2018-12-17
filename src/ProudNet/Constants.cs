using System;
using System.Net;
using System.Text;
using DotNetty.Common.Utilities;

namespace ProudNet
{
    internal enum ProudCoreOpCode : byte
    {
        // C2S
        NotifyCSEncryptedSessionKey = 5,

        NotifyServerConnectionRequestData = 7,
        ServerHolepunch = 12,
        NotifyHolepunchSuccess = 14,
        PeerUdp_ServerHolepunch = 16,
        PeerUdp_NotifyHolepunchSuccess = 18,
        ReliableRelay1 = 20,
        UnreliableRelay1 = 21,
        UnreliablePing = 26,
        SpeedHackDetectorPing = 27,

        // S2C
        ConnectServerTimedout = 3,

        NotifyServerConnectionHint = 4,
        NotifyCSSessionKeySuccess = 6,
        NotifyProtocolVersionMismatch = 8,
        NotifyServerDeniedConnection = 9,
        NotifyServerConnectSuccess = 10,
        RequestStartServerHolepunch = 11,
        ServerHolepunchAck = 13,
        NotifyClientServerUdpMatched = 15,
        PeerUdp_ServerHolepunchAck = 17,
        ReliableRelay2 = 23,
        UnreliableRelay2 = 24,
        UnreliablePong = 28,

        // SC
        Rmi = 1,

        UserMessage = 2,
        EncryptedReliable = 36,
        Encrypted_UnReliable = 37,
        Compressed = 38,

        // Unk
        ReliableUdp_Frame = 19,

        UnreliableRelay1_RelayDestListCompressed = 22,
        LingerDataFrame2 = 25,
        ArbitaryTouch = 29,
        PeerUdp_PeerHolepunch = 30,
        PeerUdp_PeerHolepunchAck = 31,
        P2PRequestIndirectServerTimeAndPing = 32,
        P2PReplyIndirectServerTimeAndPong = 33,
        S2CRoutedMulticast1 = 34,
        S2CRoutedMulticast2 = 35,
        RequestReceiveSpeedAtReceiverSide_NoRelay = 39,
        ReplyReceiveSpeedAtReceiverSide_NoRelay = 40,
        NotifyConnectionPeerRequestData = 41,
        NotifyCSP2PDisconnected = 42,
        NotifyConnectPeerRequestDataSucess = 43,
        NotifyCSConnectionPeerSuccess = 44,
        Ignore = 45,
        RequestServerConnectionHint = 46,
        PolicyRequest = 47,
        P2PReliablePing = 48,
        P2PReliablePong = 49
    }

    public enum ProudOpCode : ushort
    {
        // C2S
        ReliablePing = 64001,

        P2P_NotifyDirectP2PDisconnected = 64002,
        NotifyUdpToTcpFallbackByClient = 64003,
        P2PGroup_MemberJoin_Ack = 64004,
        NotifyP2PHolepunchSuccess = 64005,
        ShutdownTcp = 64006,
        ShutdownTcpHandshake = 64007,
        NotifyLog = 64008,
        NotifyLogHolepunchFreqFail = 64009,
        NotifyNatDeviceName = 64010,
        NotifyPeerUdpSocketRestored = 64011,
        NotifyJitDirectP2PTriggered = 64012,
        NotifyNatDeviceNameDetected = 64013,
        NotifySendSpeed = 64014,
        ReportP2PPeerPing = 64015,
        C2S_RequestCreateUdpSocket = 64016,
        C2S_CreateUdpSocketAck = 64017,
        ReportC2CUdpMessageCount = 64018,
        ReportC2SUdpMessageTrialCount = 64019,

        // S2C
        P2PGroup_MemberJoin = 64501,

        P2PGroup_MemberJoin_Unencrypted = 64502,
        P2PRecycleComplete = 64503,
        RequestP2PHolepunch = 64504,
        P2P_NotifyDirectP2PDisconnected2 = 64505,
        P2PGroup_MemberLeave = 64506,
        NotifyDirectP2PEstablish = 64507,
        ReliablePong = 64508,
        EnableLog = 64509,
        DisableLog = 64510,
        NotifyUdpToTcpFallbackByServer = 64511,
        NotifySpeedHackDetectorEnabled = 64512,
        ShutdownTcpAck = 64513,
        RequestAutoPrune = 64514,
        RenewP2PConnectionState = 64515,
        NewDirectP2PConnection = 64516,
        RequestMeasureSendSpeed = 64517,
        S2C_RequestCreateUdpSocket = 64518,
        S2C_CreateUdpSocketAck = 64519,
    }

    public enum EncryptMode : byte
    {
        None = 0,
        Secure = 1,
        Fast = 2
    }

    public enum FallbackMethod : byte
    {
        None = 0,
        PeersUdpToTcp = 1,
        ServerUdpToTcp = 2,
        CloseUdpSocket = 3
    }

    public enum DirectP2PStartCondition : byte
    {
        Jit = 0,
        Always = 1,
        Last = 2
    }

    public enum MessagePriority : byte
    {
        Ring0,
        Ring1,
        High,
        Medium,
        Low,
        Ring99,
        Last
    }

    internal enum TraceId : byte
    {
        System,
        Holepunch,
        HolepunchFreqFail
    }

    internal static class Constants
    {
        public const uint NetVersion = 196977;
        public const short NetMagic = 0x5713;
        public static readonly Encoding Encoding = CodePagesEncodingProvider.Instance.GetEncoding(1252);
        public static readonly IPEndPoint EmptyIPEndPoint = new IPEndPoint(0, 0);
        public const uint HostIdServer = 1;

        public static class Pipeline
        {
            public const string CoreMessageHandlerName = "CoreMessageHandler";
            public const string InternalMessageHandlerName = "InternalMessageHandler";
        }
    }

    internal static class ChannelAttributes
    {
        public static readonly AttributeKey<IServiceProvider> ServiceProvider =
            AttributeKey<IServiceProvider>.ValueOf($"ProudNet-{nameof(ServiceProvider)}");

        public static readonly AttributeKey<ProudSession> Session =
            AttributeKey<ProudSession>.ValueOf($"ProudNet-{nameof(Session)}");
    }
}
