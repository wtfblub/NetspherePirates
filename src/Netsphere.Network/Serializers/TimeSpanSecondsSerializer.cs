using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;

namespace Netsphere.Network.Serializers
{
    public class TimeSpanSecondsSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            return typeof(TimeSpan) == type;
        }

        public void EmitSerialize(CompilerContext ctx, Local value)
        {
            ctx.Emit.LoadArgument(1);
            ctx.Emit.LoadLocalAddress(value);
            ctx.Emit.Call(typeof (TimeSpan).GetProperty(nameof(TimeSpan.TotalSeconds)).GetMethod);
            ctx.Emit.Convert<uint>();
            ctx.Emit.CallVirtual(typeof (BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] {typeof (uint)}));
        }

        public void EmitDeserialize(CompilerContext ctx, Local value)
        {
            ctx.Emit.LoadArgument(1);
            ctx.Emit.Call(typeof (BinaryReader).GetMethod(nameof(BinaryReader.ReadUInt32)));
            ctx.Emit.Convert<double>();
            ctx.Emit.Call(typeof (TimeSpan).GetMethod(nameof(TimeSpan.FromSeconds)));
            ctx.Emit.StoreLocal(value);
        }
    }
}
