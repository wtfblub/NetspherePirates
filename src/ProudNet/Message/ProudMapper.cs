using System;
using System.Collections.Generic;
using System.IO;
using BlubLib.Serialization;

namespace ProudNet.Message
{
    internal static class ProudMapper
    {
        private static readonly Dictionary<ProudOpCode, Type> TypeLookup = new Dictionary<ProudOpCode, Type>();
        private static readonly Dictionary<Type, ProudOpCode> OpCodeLookup = new Dictionary<Type, ProudOpCode>();

        static ProudMapper()
        {
            // C2S
            Create<ReliablePingMessage>(ProudOpCode.ReliablePing);
            Create<P2PGroup_MemberJoin_AckMessage>(ProudOpCode.P2PGroup_MemberJoin_Ack);
            Create<NotifyP2PHolepunchSuccessMessage>(ProudOpCode.NotifyP2PHolepunchSuccess);
            Create<ShutdownTcpMessage>(ProudOpCode.ShutdownTcp);
            Create<NotifyLogMessage>(ProudOpCode.NotifyLog);
            Create<NotifyJitDirectP2PTriggeredMessage>(ProudOpCode.NotifyJitDirectP2PTriggered);
            Create<NotifyNatDeviceNameDetectedMessage>(ProudOpCode.NotifyNatDeviceNameDetected);
            Create<C2S_RequestCreateUdpSocketMessage>(ProudOpCode.C2S_RequestCreateUdpSocket);
            Create<C2S_CreateUdpSocketAckMessage>(ProudOpCode.C2S_CreateUdpSocketAck);
            Create<ReportC2SUdpMessageTrialCountMessage>(ProudOpCode.ReportC2SUdpMessageTrialCount);

            // S2C
            Create<ReliablePongMessage>(ProudOpCode.ReliablePong);
            Create<ShutdownTcpAckMessage>(ProudOpCode.ShutdownTcpAck);
            Create<P2PGroup_MemberJoinMessage>(ProudOpCode.P2PGroup_MemberJoin);
            Create<P2PGroup_MemberJoin_UnencryptedMessage>(ProudOpCode.P2PGroup_MemberJoin_Unencrypted);
            Create<P2PRecycleCompleteMessage>(ProudOpCode.P2PRecycleComplete);
            Create<RequestP2PHolepunchMessage>(ProudOpCode.RequestP2PHolepunch);
            Create<P2PGroup_MemberLeaveMessage>(ProudOpCode.P2PGroup_MemberLeave);
            Create<NotifyDirectP2PEstablishMessage>(ProudOpCode.NotifyDirectP2PEstablish);
            Create<NewDirectP2PConnectionMessage>(ProudOpCode.NewDirectP2PConnection);
            Create<S2C_RequestCreateUdpSocketMessage>(ProudOpCode.S2C_RequestCreateUdpSocket);
            Create<S2C_CreateUdpSocketAckMessage>(ProudOpCode.S2C_CreateUdpSocketAck);

            // SC
        }

        private static void Create<T>(ProudOpCode opCode)
            where T : ProudMessage, new()
        {
            var type = typeof(T);
            OpCodeLookup.Add(type, opCode);
            TypeLookup.Add(opCode, type);
        }

        public static ProudMessage GetMessage(ProudOpCode opCode, BinaryReader r)
        {
            var type = TypeLookup.GetValueOrDefault(opCode);
            if (type == null)
                return new ProudUnknownMessage(opCode, r.ReadToEnd());

            return (ProudMessage)Serializer.Deserialize(r, type);
        }

        public static ProudOpCode GetOpCode<T>()
            where T : ProudMessage
        {
            return GetOpCode(typeof(T));
        }

        public static ProudOpCode GetOpCode(Type type)
        {
            return OpCodeLookup[type];
        }
    }
}
