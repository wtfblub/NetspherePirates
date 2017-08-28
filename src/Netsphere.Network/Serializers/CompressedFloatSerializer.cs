using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace Netsphere.Network.Serializers
{
    public class CompressedFloatSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            throw new NotImplementedException();
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.Call(typeof(NetsphereExtensions).GetMethod(nameof(NetsphereExtensions.WriteCompressed), new[] { typeof(BinaryWriter), typeof(float) }));
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.Call(typeof(NetsphereExtensions).GetMethod(nameof(NetsphereExtensions.ReadCompressedFloat)));
            emiter.StoreLocal(value);
        }
    }
}
