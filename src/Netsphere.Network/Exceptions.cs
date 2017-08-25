using System;
using System.Collections.Generic;
using BlubLib;

namespace Netsphere.Network
{
    public class NetsphereBadFormatException : Exception
    {
        public NetsphereBadFormatException(Type type)
            : base($"Bad format in {type.Name}")
        { }

        public NetsphereBadFormatException(Type type, IEnumerable<byte> data)
            : base($"Bad format in {type.Name}: {data.ToHexString()}")
        { }
    }

    public class NetsphereBadOpCodeException : Exception
    {
        public NetsphereBadOpCodeException(ushort opCode)
            : base($"Bad opCode: {opCode}")
        { }

        public NetsphereBadOpCodeException(AuthOpCode opCode)
            : base($"Bad opCode: {opCode}")
        { }

        public NetsphereBadOpCodeException(ChatOpCode opCode)
            : base($"Bad opCode: {opCode}")
        { }

        public NetsphereBadOpCodeException(GameOpCode opCode)
            : base($"Bad opCode: {opCode}")
        { }

        public NetsphereBadOpCodeException(GameRuleOpCode opCode)
            : base($"Bad opCode: {opCode}")
        { }

        public NetsphereBadOpCodeException(RelayOpCode opCode)
            : base($"Bad opCode: {opCode}")
        { }

        public NetsphereBadOpCodeException(EventOpCode opCode)
            : base($"Bad opCode: {opCode}")
        { }
    }
}
