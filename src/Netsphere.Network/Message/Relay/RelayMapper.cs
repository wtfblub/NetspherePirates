using System;
using System.Collections.Generic;
using System.IO;
using BlubLib.Serialization;

namespace Netsphere.Network.Message.Relay
{
    public static class RelayMapper
    {
        private static readonly Dictionary<RelayOpCode, Type> TypeLookup = new Dictionary<RelayOpCode, Type>();
        private static readonly Dictionary<Type, RelayOpCode> OpCodeLookup = new Dictionary<Type, RelayOpCode>();

        static RelayMapper()
        {
            // S2C
            Create<SEnterLoginPlayerMessage>(RelayOpCode.SEnterLoginPlayer);
            Create<SNotifyLoginResultMessage>(RelayOpCode.SNotifyLoginResult);

            // C2S
            Create<CRequestLoginMessage>(RelayOpCode.CRequestLogin);
        }

        public static void Create<T>(RelayOpCode opCode)
            where T : RelayMessage, new()
        {
            var type = typeof(T);
            OpCodeLookup.Add(type, opCode);
            TypeLookup.Add(opCode, type);
        }

        public static RelayMessage GetMessage(RelayOpCode opCode, BinaryReader r)
        {
            var type = TypeLookup.GetValueOrDefault(opCode);
            if (type == null)
                return new RelayUnknownMessage(opCode, r.ReadToEnd());
                //throw new NetsphereBadOpCodeException(opCode);

            return (RelayMessage)Serializer.Deserialize(r, type);
        }

        public static RelayOpCode GetOpCode<T>()
            where T : RelayMessage
        {
            return GetOpCode(typeof(T));
        }

        public static RelayOpCode GetOpCode(Type type)
        {
            return OpCodeLookup[type];
        }
    }
}