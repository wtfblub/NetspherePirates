using System;
using System.IO;
using System.Linq;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;

namespace Netsphere.Network.Serializers
{
    /// <summary>
    /// Serializes <see cref="LongPeerId"/> as int64
    /// </summary>
    public class LongPeerIdSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            return type == typeof(LongPeerId);
        }

        public void EmitSerialize(CompilerContext context, Local value)
        {
            // BinaryWriter.Write(value)

            context.Emit.LoadReaderOrWriterParam();
            context.Emit.LoadLocal(value);
            context.Emit.Call(typeof(LongPeerId).GetMethods().First(m => m.Name == "op_Implicit" && m.ReturnType == typeof(ulong)));
            context.Emit.CallVirtual(ReflectionHelper.GetMethod((BinaryWriter _) => _.Write(default(ulong))));
        }

        public void EmitDeserialize(CompilerContext context, Local value)
        {
            // value = BinaryReader.ReadUInt64()

            context.Emit.LoadReaderOrWriterParam();
            context.Emit.CallVirtual(ReflectionHelper.GetMethod((BinaryReader _) => _.ReadUInt64()));
            context.Emit.Call(typeof(LongPeerId).GetMethod("op_Implicit", new[] { typeof(ulong) }));
            context.Emit.StoreLocal(value);
        }
    }
}
