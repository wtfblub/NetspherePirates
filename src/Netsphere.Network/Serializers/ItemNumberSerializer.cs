using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;

namespace Netsphere.Network.Serializers
{
    internal class ItemNumberSerializer : ISerializerCompiler
    {
        public Type HandlesType => typeof (ItemNumber);

        public void EmitSerialize(Emit<Action<BinaryWriter, object>> emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadLocalAddress(value);
            emiter.Call(typeof(ItemNumber).GetProperty(nameof(ItemNumber.Id)).GetMethod);
            emiter.CallVirtual(typeof(BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] { typeof(uint) }));
        }

        public void EmitDeserialize(Emit<Func<BinaryReader, object>> emiter, Local value)
        {
            emiter.LoadLocalAddress(value);
            emiter.LoadArgument(1);
            emiter.CallVirtual(typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadUInt32)));
            emiter.Call(typeof(ItemNumber).GetConstructor(new[] { typeof(uint) }));
        }
    }
}
