using System;
using System.IO;
using BlubLib.Serialization;
using Netsphere.Network.Data.Relay;
using Sigil;
using Sigil.NonGeneric;

namespace Netsphere.Network.Serializers
{
    internal class RoomLocationSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            throw new NotImplementedException();
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadLocalAddress(value);
            emiter.Call(typeof(RoomLocation).GetProperty(nameof(RoomLocation.Value)).GetMethod);
            emiter.CallVirtual(typeof(BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] { typeof(uint) }));
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.CallVirtual(typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadUInt32)));
            emiter.NewObject<RoomLocation, uint>();
            emiter.StoreLocal(value);
        }
    }
}
