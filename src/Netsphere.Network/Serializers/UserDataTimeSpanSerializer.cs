using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;

namespace Netsphere.Network.Serializers
{
    internal class UserDataTimeSpanSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            throw new NotImplementedException();
        }

        public void EmitSerialize(CompilerContext context, Local value)
        {
            context.Emit.LoadArgument(1);
            context.Emit.LoadLocalAddress(value);
            context.Emit.Call(typeof (TimeSpan).GetProperty(nameof(TimeSpan.TotalSeconds)).GetMethod);
            context.Emit.Convert<uint>();
            context.Emit.CallVirtual(typeof (BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] {typeof (uint)}));
        }

        public void EmitDeserialize(CompilerContext context, Local value)
        {
            context.Emit.LoadArgument(1);
            context.Emit.Call(typeof (BinaryReader).GetMethod(nameof(BinaryReader.ReadUInt32)));
            context.Emit.Convert<double>();
            context.Emit.Call(typeof (TimeSpan).GetMethod(nameof(TimeSpan.FromSeconds)));
            context.Emit.StoreLocal(value);
        }
    }
}
