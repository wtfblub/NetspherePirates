using System;
using System.IO;
using BlubLib.IO;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace ProudNet.Serialization.Serializers
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
            emiter.Call(ReflectionHelper.GetMethod((BinaryReader x) => x.ReadToEnd()));
            emiter.StoreLocal(value);
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            // writer.Write(value)
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.CallVirtual(ReflectionHelper.GetMethod((BinaryWriter x) => x.Write(default(byte[]))));
        }
    }
}
