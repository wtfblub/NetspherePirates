using System;
using System.IO;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;

namespace Netsphere.Network.Serializers
{
    /// <summary>
    /// Serializes <see cref="MatchKey"/> as int32
    /// </summary>
    public class MatchKeySerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            return type == typeof(MatchKey);
        }

        public void EmitSerialize(CompilerContext context, Local value)
        {
            // BinaryWriter.Write(value)

            context.Emit.LoadReaderOrWriterParam();
            context.Emit.LoadLocal(value);
            context.Emit.Call(typeof(MatchKey).GetMethod("op_Implicit", new[] { typeof(MatchKey) }));
            context.Emit.CallVirtual(ReflectionHelper.GetMethod((BinaryWriter _) => _.Write(default(uint))));
        }

        public void EmitDeserialize(CompilerContext context, Local value)
        {
            // value = BinaryReader.ReadUInt32()

            context.Emit.LoadReaderOrWriterParam();
            context.Emit.CallVirtual(ReflectionHelper.GetMethod((BinaryReader _) => _.ReadUInt32()));
            context.Emit.Call(typeof(MatchKey).GetMethod("op_Implicit", new[] { typeof(uint) }));
            context.Emit.StoreLocal(value);
        }
    }
}
