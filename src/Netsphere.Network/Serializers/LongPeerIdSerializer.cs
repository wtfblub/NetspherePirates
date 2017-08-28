using System;
using System.IO;
using System.Linq;
using BlubLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace Netsphere.Network.Serializers
{
    public class LongPeerIdSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type) => type == typeof(LongPeerId);

        public void EmitSerialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.Call(typeof(LongPeerId).GetMethods().First(m => m.Name == "op_Implicit" && m.ReturnType == typeof(ulong)));
            emiter.CallVirtual(typeof(BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] { typeof(ulong) }));
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.CallVirtual(typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadUInt64)));
            emiter.Call(typeof(LongPeerId).GetMethod("op_Implicit", new[] { typeof(ulong) }));
            emiter.StoreLocal(value);
        }
    }
}
