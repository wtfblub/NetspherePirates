using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;

namespace ProudNet.Serializers
{
    public class ReadToEndSerializer : ISerializerCompiler
    {
        public Type HandlesType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void EmitDeserialize(Emit<Func<BinaryReader, object>> emiter, Local value)
        {
            // value = BinaryReaderExtensions.ReadToEnd(reader);
            emiter.LoadArgument(1);
            emiter.Call(typeof (BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadToEnd)));
            emiter.StoreLocal(value);
        }

        public void EmitSerialize(Emit<Action<BinaryWriter, object>> emiter, Local value)
        {
            // writer.Write(value)
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.CallVirtual(typeof (BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] {typeof (byte[])}));
        }
    }
}
