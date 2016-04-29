using System;
using BlubLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace ProudNet.Serializers
{
    public class ScalarSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            throw new NotImplementedException();
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            // value = ProudNetBinaryReaderExtensions.ReadScalar(reader)
            emiter.LoadArgument(1);
            emiter.Call(typeof(ProudNetBinaryReaderExtensions).GetMethod(nameof(ProudNetBinaryReaderExtensions.ReadScalar)));
            emiter.StoreLocal(value);
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            // ProudNetBinaryWriterExtensions.WriteScalar(writer, value)
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.Call(typeof(ProudNetBinaryWriterExtensions).GetMethod(nameof(ProudNetBinaryWriterExtensions.WriteScalar)));
        }
    }
}
