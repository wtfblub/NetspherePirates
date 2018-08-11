using System;
using System.IO;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Netsphere.Network.Data.Relay;
using Sigil;

namespace Netsphere.Network.Serializers
{
    /// <summary>
    /// Serializes <see cref="RoomLocation"/> as int32
    /// </summary>
    public class RoomLocationSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            return type == typeof(RoomLocation);
        }

        public void EmitSerialize(CompilerContext context, Local value)
        {
            // BinaryWriter.Write(value.Value)

            context.Emit.LoadReaderOrWriterParam();
            context.Emit.LoadLocalAddress(value);
            context.Emit.Call(typeof(RoomLocation).GetProperty(nameof(RoomLocation.Value)).GetMethod);
            context.Emit.CallVirtual(ReflectionHelper.GetMethod((BinaryWriter _) => _.Write(default(uint))));
        }

        public void EmitDeserialize(CompilerContext context, Local value)
        {
            // value = new RoomLocation(BinaryReader.ReadUInt32())

            context.Emit.LoadReaderOrWriterParam();
            context.Emit.CallVirtual(ReflectionHelper.GetMethod((BinaryReader _) => _.ReadUInt32()));
            context.Emit.NewObject<RoomLocation, uint>();
            context.Emit.StoreLocal(value);
        }
    }
}
