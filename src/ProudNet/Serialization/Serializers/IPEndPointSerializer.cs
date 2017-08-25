using System;
using System.IO;
using System.Net;
using BlubLib.IO;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace ProudNet.Serialization.Serializers
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
            emiter.Call(ReflectionHelper.GetMethod((BinaryReader x) => x.ReadIPEndPoint()));
            emiter.StoreLocal(value);
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            // BinaryWriterExtensions.Write(writer, value)
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.Call(ReflectionHelper.GetMethod((BinaryWriter x) => x.Write(default(IPEndPoint))));
        }
    }
}
