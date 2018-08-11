using System;
using System.IO;
using BlubLib.IO;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;

namespace ProudNet.Serialization.Serializers
{
    /// <summary>
    /// Serializes a raw byte array. This serializer consumes the entire stream on deserialize
    /// </summary>
    public class ReadToEndSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            return type.IsAssignableFrom(typeof(byte[]));
        }

        public void EmitDeserialize(CompilerContext context, Local value)
        {
            // value = BinaryReaderExtensions.ReadToEnd(reader);
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.Call(ReflectionHelper.GetMethod((BinaryReader _) => _.ReadToEnd()));
            context.Emit.StoreLocal(value);
        }

        public void EmitSerialize(CompilerContext context, Local value)
        {
            var writeLabel = context.Emit.DefineLabel();

            // if (value != null) goto write
            context.Emit.LoadLocal(value);
            context.Emit.LoadNull();
            context.Emit.CompareEqual();
            context.Emit.BranchIfFalse(writeLabel);

            // value = Array.Empty<byte>()
            context.Emit.Call(ReflectionHelper.GetMethod(() => Array.Empty<byte>()));
            context.Emit.StoreLocal(value);

            // writer.Write(value)
            context.Emit.MarkLabel(writeLabel);
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.LoadLocal(value);
            context.Emit.CallVirtual(ReflectionHelper.GetMethod((BinaryWriter _) => _.Write(default(byte[]))));
        }
    }
}
