using System;
using System.IO;
using System.Numerics;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;

namespace Netsphere.Network.Serializers
{
    /// <summary>
    /// Serializes <see cref="Vector3"/> as a array of 3 compressed floats. A compressed float is int16
    /// </summary>
    public class CompressedVector3Serializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            return type == typeof(Vector3);
        }

        public void EmitSerialize(CompilerContext context, Local value)
        {
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.LoadLocal(value);
            context.Emit.Call(ReflectionHelper.GetMethod((BinaryWriter _) => _.WriteCompressed(default(Vector3))));
        }

        public void EmitDeserialize(CompilerContext context, Local value)
        {
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.Call(ReflectionHelper.GetMethod((BinaryReader _) => _.ReadCompressedVector3()));
            context.Emit.StoreLocal(value);
        }
    }
}
