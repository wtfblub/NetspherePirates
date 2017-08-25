using System;
using System.Collections.Generic;
using BlubLib;

namespace ProudNet
{
    public class ProudException : Exception
    {
        public ProudException()
        { }

        public ProudException(string message)
            : base(message)
        { }

        public ProudException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    public class ProudBadOpCodeException : Exception
    {
        internal ProudBadOpCodeException(ProudCoreOpCode opCode)
            : base($"Invalid opcopde {opCode}")
        { }

        internal ProudBadOpCodeException(ProudCoreOpCode opCode, IEnumerable<byte> data)
            : base($"Invalid opcopde {opCode}: {data.ToHexString()}")
        { }

        internal ProudBadOpCodeException(ushort opCode)
            : base($"Invalid opcopde {opCode}")
        { }

        internal ProudBadOpCodeException(ushort opCode, IEnumerable<byte> data)
            : base($"Invalid opcopde {opCode}: {data.ToHexString()}")
        { }
    }

    public class ProudBadFormatException : Exception
    {
        public ProudBadFormatException(Type type)
            : base($"Bad format in {type.Name}")
        { }

        internal ProudBadFormatException(Type type, IEnumerable<byte> data)
            : base($"Bad format in {type.Name}: {data.ToHexString()}")
        { }
    }
}
