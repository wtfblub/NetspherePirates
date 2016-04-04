using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;

namespace ProudNet.Serializers
{
    public class EnumSerializer : ISerializerCompiler
    {
        public Type HandlesType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void EmitDeserialize(Emit<Func<BinaryReader, object>> emiter, Local value)
        {
            var underlyingType = value.LocalType.GetEnumUnderlyingType();
            emiter.CallDeserializerForType(underlyingType, value);
        }

        public void EmitSerialize(Emit<Action<BinaryWriter, object>> emiter, Local value)
        {
            var underlyingType = value.LocalType.GetEnumUnderlyingType();
            emiter.CallSerializerForType(underlyingType, value);
        }
    }
}
