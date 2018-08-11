using System;
using System.IO;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;

namespace ProudNet.Serialization.Serializers
{
    /// <summary>
    /// Serializes integer fields with a variable length
    /// Uses <see cref="ProudNetBinaryWriterExtensions.WriteScalar"/>/<see cref="ProudNetBinaryReaderExtensions.ReadScalar"/>
    /// </summary>
    public class ScalarSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            return type == typeof(byte) || type == typeof(sbyte) ||
                    type == typeof(short) || type == typeof(ushort) ||
                    type == typeof(int) || type == typeof(uint);
        }

        public void EmitDeserialize(CompilerContext context, Local value)
        {
            // value = ProudNetBinaryReaderExtensions.ReadScalar(reader)
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.Call(ReflectionHelper.GetMethod((BinaryReader _) => _.ReadScalar()));
            context.Emit.StoreLocal(value);
        }

        public void EmitSerialize(CompilerContext context, Local value)
        {
            // ProudNetBinaryWriterExtensions.WriteScalar(writer, value)
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.LoadLocal(value);
            context.Emit.Call(ReflectionHelper.GetMethod((BinaryWriter _) => _.WriteScalar(default(int))));
        }
    }
}
