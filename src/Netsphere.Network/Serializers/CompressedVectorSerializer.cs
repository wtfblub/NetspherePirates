using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;
using SlimMath;

namespace Netsphere.Network.Serializers
{
    internal class CompressedVectorSerializer : ISerializerCompiler
    {
        public Type HandlesType
        {
            get { throw new NotImplementedException(); }
        }

        public void EmitSerialize(Emit<Action<BinaryWriter, object>> emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.Call(typeof (NetsphereExtensions).GetMethod(nameof(NetsphereExtensions.WriteCompressed), new[] {typeof(BinaryWriter), typeof (Vector3)}));
        }

        public void EmitDeserialize(Emit<Func<BinaryReader, object>> emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.Call(typeof (NetsphereExtensions).GetMethod(nameof(NetsphereExtensions.ReadCompressedVector3)));
            emiter.StoreLocal(value);
        }
    }
}
