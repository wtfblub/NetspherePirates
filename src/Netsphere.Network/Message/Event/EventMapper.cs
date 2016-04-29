using System;
using System.Collections.Generic;
using System.IO;
using BlubLib.Serialization;

namespace Netsphere.Network.Message.Event
{
    public static class EventMapper
    {
        private static readonly Dictionary<EventOpCode, Type> s_typeLookup = new Dictionary<EventOpCode, Type>();
        private static readonly Dictionary<Type, EventOpCode> s_opCodeLookup = new Dictionary<Type, EventOpCode>();

        static EventMapper()
        {
            Create<ChatMessage>(EventOpCode.Chat);
            Create<EventMessageMessage>(EventOpCode.EventMessage);
            Create<ChangeTargetMessage>(EventOpCode.ChangeTarget);
            Create<ArcadeSyncMessage>(EventOpCode.ArcadeSync);
            Create<ArcadeSyncReqMessage>(EventOpCode.ArcadeSyncReq);
            Create<PacketMessage>(EventOpCode.Packet);
        }

        public static void Create<T>(EventOpCode opCode)
            where T : EventMessage, new()
        {
            var type = typeof(T);
            s_opCodeLookup.Add(type, opCode);
            s_typeLookup.Add(opCode, type);
        }

        public static EventMessage GetMessage(EventOpCode opCode, BinaryReader r)
        {
            var type = s_typeLookup.GetValueOrDefault(opCode);
            if (type == null)
                throw new NetsphereBadOpCodeException(opCode);

            return (EventMessage)Serializer.Deserialize(r, type);
        }

        public static EventOpCode GetOpCode<T>()
            where T : EventMessage
        {
            return GetOpCode(typeof(T));
        }

        public static EventOpCode GetOpCode(Type type)
        {
            return s_opCodeLookup[type];
        }
    }
}