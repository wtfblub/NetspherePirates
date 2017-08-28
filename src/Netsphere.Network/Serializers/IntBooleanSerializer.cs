using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace Netsphere.Network.Serializers
{
    public class IntBooleanSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            throw new NotImplementedException();
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.LoadConstant(true);
            emiter.CompareEqual();
            emiter.CallVirtual(typeof(BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] { typeof(int) }));
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.CallVirtual(typeof (BinaryReader).GetMethod(nameof(BinaryReader.ReadInt32)));
            emiter.Convert<bool>();
            emiter.StoreLocal(value);
        }
    }
}
