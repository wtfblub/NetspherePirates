using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace Netsphere.Network.Serializers
{
    internal class MatchKeySerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type) => type == typeof(MatchKey);

        public void EmitSerialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.Call(typeof(MatchKey).GetMethod("op_Implicit", new[] { typeof(MatchKey) }));
            emiter.CallVirtual(typeof(BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] { typeof(uint) }));
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.CallVirtual(typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadUInt32)));
            emiter.Call(typeof(MatchKey).GetMethod("op_Implicit", new[] { typeof(uint) }));
            emiter.StoreLocal(value);
        }
    }
}
