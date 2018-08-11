using System;
using System.IO;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;

namespace Netsphere.Network.Serializers
{
    /// <summary>
    /// Serializes <see cref="TimeSpan"/> as a byte using TotalMinutes
    /// </summary>
    public class TimeLimitSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            return type == typeof(TimeSpan);
        }

        public void EmitSerialize(CompilerContext context, Local value)
        {
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.LoadLocalAddress(value);
            context.Emit.Call(typeof(TimeSpan).GetProperty(nameof(TimeSpan.TotalMinutes)).GetMethod);
            context.Emit.Convert<byte>();
            context.Emit.CallVirtual(ReflectionHelper.GetMethod((BinaryWriter _) => _.Write(default(byte))));
        }

        public void EmitDeserialize(CompilerContext context, Local value)
        {
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.CallVirtual(ReflectionHelper.GetMethod((BinaryReader _) => _.ReadByte()));
            context.Emit.Convert<double>();
            context.Emit.Call(typeof(TimeSpan).GetMethod(nameof(TimeSpan.FromMinutes)));
            context.Emit.StoreLocal(value);
        }
    }
}
