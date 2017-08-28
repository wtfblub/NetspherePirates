using System;
using BlubLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace Netsphere.Network.Serializers
{
    public class RotationVectorSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            throw new NotImplementedException();
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.Call(typeof (NetsphereExtensions).GetMethod(nameof(NetsphereExtensions.WriteRotation)));
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.Call(typeof (NetsphereExtensions).GetMethod(nameof(NetsphereExtensions.ReadRotation)));
            emiter.StoreLocal(value);
        }
    }
}
