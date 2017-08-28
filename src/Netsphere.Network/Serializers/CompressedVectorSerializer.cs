using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;
using Sigil.NonGeneric;
using SlimMath;

namespace Netsphere.Network.Serializers
{
    public class CompressedVectorSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            throw new NotImplementedException();
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.Call(typeof (NetsphereExtensions).GetMethod(nameof(NetsphereExtensions.WriteCompressed), new[] {typeof(BinaryWriter), typeof (Vector3)}));
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.Call(typeof (NetsphereExtensions).GetMethod(nameof(NetsphereExtensions.ReadCompressedVector3)));
            emiter.StoreLocal(value);
        }
    }
}
