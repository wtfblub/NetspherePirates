using BlubLib.Serialization;
using Sigil;
using System;
using Sigil.NonGeneric;

namespace ProudNet.Serializers
{
    public class StringSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            throw new NotImplementedException();
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.Call(typeof(ProudNetBinaryReaderExtensions).GetMethod(nameof(ProudNetBinaryReaderExtensions.ReadProudString)));
            emiter.StoreLocal(value);
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            var write = emiter.DefineLabel(nameof(StringSerializer) + "Write" + Guid.NewGuid());

            // if (value != null) goto write
            emiter.LoadLocal(value);
            emiter.LoadNull();
            emiter.CompareEqual();
            emiter.BranchIfFalse(write);

            // value = string.Empty
            emiter.LoadField(typeof(string).GetField(nameof(string.Empty)));
            emiter.StoreLocal(value);

            // ProudNetBinaryWriterExtensions.WriteProudString(writer, value, false)
            emiter.MarkLabel(write);
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.LoadConstant(false);
            emiter.Call(typeof(ProudNetBinaryWriterExtensions).GetMethod(nameof(ProudNetBinaryWriterExtensions.WriteProudString)));
        }
    }
}
