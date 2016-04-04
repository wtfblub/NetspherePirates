using System;
using System.Collections.Generic;
using System.IO;
using BlubLib.Serialization;
using ProudNet.Serializers;

namespace ProudNet.Message.Core
{
    internal static class CoreMapper
    {
        private static readonly Dictionary<ProudCoreOpCode, Type> _typeLookup = new Dictionary<ProudCoreOpCode, Type>();
        private static readonly Dictionary<Type, ProudCoreOpCode> _opCodeLookup = new Dictionary<Type, ProudCoreOpCode>();

        static CoreMapper()
        {
            Serializer.AddCompiler(new CompressedMessageSerializer());

            // S2C
            Create<ConnectServerTimedoutMessage>(ProudCoreOpCode.ConnectServerTimedout);
            Create<NotifyServerConnectionHintMessage>(ProudCoreOpCode.NotifyServerConnectionHint);
            Create<NotifyCSSessionKeySuccessMessage>(ProudCoreOpCode.NotifyCSSessionKeySuccess);
            Create<NotifyProtocolVersionMismatchMessage>(ProudCoreOpCode.NotifyProtocolVersionMismatch);
            Create<NotifyServerDeniedConnectionMessage>(ProudCoreOpCode.NotifyServerDeniedConnection);
            Create<NotifyServerConnectSuccessMessage>(ProudCoreOpCode.NotifyServerConnectSuccess);
            Create<RequestStartServerHolepunchMessage>(ProudCoreOpCode.RequestStartServerHolepunch);
            Create<ServerHolepunchAckMessage>(ProudCoreOpCode.ServerHolepunchAck);
            Create<NotifyClientServerUdpMatchedMessage>(ProudCoreOpCode.NotifyClientServerUdpMatched);
            Create<PeerUdp_ServerHolepunchAckMessage>(ProudCoreOpCode.PeerUdp_ServerHolepunchAck);
            Create<UnreliablePongMessage>(ProudCoreOpCode.UnreliablePong);
            Create<ReliableRelay2Message>(ProudCoreOpCode.ReliableRelay2);
            Create<UnreliableRelay2Message>(ProudCoreOpCode.UnreliableRelay2);

            // C2S
            Create<NotifyCSEncryptedSessionKeyMessage>(ProudCoreOpCode.NotifyCSEncryptedSessionKey);
            Create<NotifyServerConnectionRequestDataMessage>(ProudCoreOpCode.NotifyServerConnectionRequestData);
            Create<ServerHolepunchMessage>(ProudCoreOpCode.ServerHolepunch);
            Create<NotifyHolepunchSuccessMessage>(ProudCoreOpCode.NotifyHolepunchSuccess);
            Create<PeerUdp_ServerHolepunchMessage>(ProudCoreOpCode.PeerUdp_ServerHolepunch);
            Create<PeerUdp_NotifyHolepunchSuccessMessage>(ProudCoreOpCode.PeerUdp_NotifyHolepunchSuccess);
            Create<UnreliablePingMessage>(ProudCoreOpCode.UnreliablePing);
            Create<SpeedHackDetectorPingMessage>(ProudCoreOpCode.SpeedHackDetectorPing);
            Create<ReliableRelay1Message>(ProudCoreOpCode.ReliableRelay1);
            Create<UnreliableRelay1Message>(ProudCoreOpCode.UnreliableRelay1);

            // SC
            Create<RmiMessage>(ProudCoreOpCode.Rmi);
            Create<EncryptedReliableMessage>(ProudCoreOpCode.EncryptedReliable);
            Create<CompressedMessage>(ProudCoreOpCode.Compressed);
            Create<ReliableUdp_FrameMessage>(ProudCoreOpCode.ReliableUdp_Frame);
        }

        private static void Create<T>(ProudCoreOpCode opCode)
            where T : CoreMessage, new()
        {
            var type = typeof(T);
            _opCodeLookup.Add(type, opCode);
            _typeLookup.Add(opCode, type);
        }

        public static CoreMessage GetMessage(ProudCoreOpCode opCode, BinaryReader r)
        {
            var type = _typeLookup.GetValueOrDefault(opCode);
            if(type == null)
#if DEBUG
                throw new ProudBadOpCodeException(opCode, r.ReadToEnd());
#else
                throw new ProudBadOpCodeException(opCode);
#endif

            return (CoreMessage)Serializer.Deserialize(r, type);
        }

        public static ProudCoreOpCode GetOpCode<T>()
            where T : CoreMessage
        {
            return GetOpCode(typeof(T));
        }

        public static ProudCoreOpCode GetOpCode(Type type)
        {
            return _opCodeLookup[type];
        }
    }
}
