using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;

namespace Netsphere.Network.Serializers
{
    internal class RotationVectorSerializer : ISerializerCompiler
    {
        public Type HandlesType
        {
            get { throw new NotImplementedException(); }
        }

        public void EmitSerialize(Emit<Action<BinaryWriter, object>> emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.Call(typeof (NetsphereExtensions).GetMethod(nameof(NetsphereExtensions.WriteRotation)));
        }

        public void EmitDeserialize(Emit<Func<BinaryReader, object>> emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.Call(typeof (NetsphereExtensions).GetMethod(nameof(NetsphereExtensions.ReadRotation)));
            emiter.StoreLocal(value);
        }
    }
}
