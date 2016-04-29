using System;
using System.Collections.Generic;
using System.IO;
using BlubLib.Serialization;

namespace Netsphere.Network.Message.Auth
{
    internal static class AuthMapper
    {
        private static readonly Dictionary<AuthOpCode, Type> s_typeLookup = new Dictionary<AuthOpCode, Type>();
        private static readonly Dictionary<Type, AuthOpCode> s_opCodeLookup = new Dictionary<Type, AuthOpCode>();

        static AuthMapper()
        {
            // S2C
            Create<SAuthInEuAckMessage>(AuthOpCode.SAuthInEuAck);
            Create<SServerListAckMessage>(AuthOpCode.SServerListAck);

            // C2S
            Create<CAuthInEUReqMessage>(AuthOpCode.CAuthInEuReq);
            Create<CServerListReqMessage>(AuthOpCode.CServerListReq);
        }

        public static void Create<T>(AuthOpCode opCode)
            where T : AuthMessage, new()
        {
            var type = typeof(T);
            s_opCodeLookup.Add(type, opCode);
            s_typeLookup.Add(opCode, type);
        }

        public static AuthMessage GetMessage(AuthOpCode opCode, BinaryReader r)
        {
            var type = s_typeLookup.GetValueOrDefault(opCode);
            if (type == null)
                throw new NetsphereBadOpCodeException(opCode);

            return (AuthMessage)Serializer.Deserialize(r, type);
        }

        public static AuthOpCode GetOpCode<T>()
            where T : AuthMessage
        {
            return GetOpCode(typeof(T));
        }

        public static AuthOpCode GetOpCode(Type type)
        {
            return s_opCodeLookup[type];
        }
    }
}