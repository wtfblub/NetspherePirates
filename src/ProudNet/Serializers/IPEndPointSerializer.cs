using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;
using System.Net;
using Sigil.NonGeneric;

namespace ProudNet.Serializers
{
    public class IPEndPointSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            throw new NotImplementedException();
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            // value = BinaryReaderExtensions.ReadIPEndPoint(reader)
            emiter.LoadArgument(1);
            emiter.Call(typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadIPEndPoint)));
            emiter.StoreLocal(value);
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            // BinaryWriterExtensions.Write(writer, value)
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.Call(typeof(BinaryWriterExtensions).GetMethod(nameof(BinaryWriterExtensions.Write), new Type[] { typeof(BinaryWriter), typeof(IPEndPoint) }));
        }
    }
}
