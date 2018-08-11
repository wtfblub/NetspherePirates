using System;
using System.IO;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;

namespace Netsphere.Network.Serializers
{
    /// <summary>
    /// Serializes <see cref="ItemNumber"/> as int32
    /// </summary>
    public class ItemNumberSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            return type == typeof(ItemNumber);
        }

        public void EmitSerialize(CompilerContext context, Local value)
        {
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.LoadLocalAddress(value);
            context.Emit.Call(typeof(ItemNumber).GetProperty(nameof(ItemNumber.Id)).GetMethod);
            context.Emit.CallVirtual(ReflectionHelper.GetMethod((BinaryWriter _) => _.Write(default(uint))));
        }

        public void EmitDeserialize(CompilerContext context, Local value)
        {
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.CallVirtual(ReflectionHelper.GetMethod((BinaryReader _) => _.ReadUInt32()));
            context.Emit.NewObject<ItemNumber, uint>();
            context.Emit.StoreLocal(value);
        }
    }
}
