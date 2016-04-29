using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace ProudNet.Serializers
{
    public class ReadToEndSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            throw new NotImplementedException();
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            // value = BinaryReaderExtensions.ReadToEnd(reader);
            emiter.LoadArgument(1);
            emiter.Call(typeof (BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadToEnd)));
            emiter.StoreLocal(value);
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            // writer.Write(value)
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.CallVirtual(typeof (BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] {typeof (byte[])}));
        }
    }
}
