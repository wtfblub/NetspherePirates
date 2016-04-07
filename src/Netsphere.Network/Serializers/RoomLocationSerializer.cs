using System;
using System.IO;
using BlubLib.Serialization;
using Netsphere.Network.Data.Relay;
using Sigil;

namespace Netsphere.Network.Serializers
{
    internal class RoomLocationSerializer : ISerializerCompiler
    {
        public Type HandlesType
        {
            get { throw new NotImplementedException(); }
        }

        public void EmitSerialize(Emit<Action<BinaryWriter, object>> emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadLocalAddress(value);
            emiter.Call(typeof(RoomLocation).GetProperty(nameof(RoomLocation.Value)).GetMethod);
            emiter.CallVirtual(typeof(BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] { typeof(uint) }));
        }

        public void EmitDeserialize(Emit<Func<BinaryReader, object>> emiter, Local value)
        {
            emiter.LoadLocalAddress(value);
            emiter.LoadArgument(1);
            emiter.CallVirtual(typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadUInt32)));
            emiter.Call(typeof(RoomLocation).GetConstructor(new[] { typeof(uint) }));
        }
    }
}
