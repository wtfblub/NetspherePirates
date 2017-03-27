using System;
using System.IO;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace ProudNet.Serialization.Serializers
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
            emiter.Call(ReflectionHelper.GetMethod((BinaryReader x) => x.ReadScalar()));
            emiter.StoreLocal(value);
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            // ProudNetBinaryWriterExtensions.WriteScalar(writer, value)
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.Call(ReflectionHelper.GetMethod((BinaryWriter x) => x.WriteScalar(default(int))));
        }
    }
}
