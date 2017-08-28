using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace Netsphere.Network.Serializers
{
    public class PeerIdSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type) => type == typeof(PeerId);

        public void EmitSerialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.Call(typeof(PeerId).GetMethod("op_Implicit", new[] { typeof(PeerId) }));
            emiter.CallVirtual(typeof(BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] { typeof(ushort) }));
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.CallVirtual(typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadUInt16)));
            emiter.Call(typeof(PeerId).GetMethod("op_Implicit", new[] { typeof(ushort) }));
            emiter.StoreLocal(value);
        }
    }
}
