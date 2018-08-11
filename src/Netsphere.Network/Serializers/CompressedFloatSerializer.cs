using System;
using System.IO;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;

namespace Netsphere.Network.Serializers
{
    /// <summary>
    /// Serializes <see cref="float"/> as a compressed int16
    /// </summary>
    public class CompressedFloatSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            return type == typeof(float);
        }

        public void EmitSerialize(CompilerContext context, Local value)
        {
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.LoadLocal(value);
            context.Emit.Call(ReflectionHelper.GetMethod((BinaryWriter _) => _.WriteCompressed(default(float))));
        }

        public void EmitDeserialize(CompilerContext context, Local value)
        {
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.Call(ReflectionHelper.GetMethod((BinaryReader _) => _.ReadCompressedFloat()));
            context.Emit.StoreLocal(value);
        }
    }
}
