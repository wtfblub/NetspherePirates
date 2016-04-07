using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;

namespace ProudNet.Serializers
{
    public class EnumSerializer : ISerializerCompiler
    {
        private readonly Type _serializeAsType;

        public Type HandlesType
        {
            get { throw new NotImplementedException(); }
        }

        public EnumSerializer()
        { }

        public EnumSerializer(Type serializeAsType)
        {
            _serializeAsType = serializeAsType;
        }

        public void EmitDeserialize(Emit<Func<BinaryReader, object>> emiter, Local value)
        {
            var underlyingType = value.LocalType.GetEnumUnderlyingType();
            var typeToUse = _serializeAsType ?? underlyingType;

            using (var tmp = emiter.DeclareLocal(typeToUse))
            {
                emiter.CallDeserializerForType(typeToUse, tmp);
                emiter.LoadLocal(tmp);
                if (underlyingType != typeToUse)
                    emiter.Convert(underlyingType);
                emiter.StoreLocal(value);
            }
        }

        public void EmitSerialize(Emit<Action<BinaryWriter, object>> emiter, Local value)
        {
            var underlyingType = value.LocalType.GetEnumUnderlyingType();
            var typeToUse = _serializeAsType ?? underlyingType;

            using (var tmp = emiter.DeclareLocal(typeToUse))
            {
                emiter.LoadLocal(value);
                if (underlyingType != typeToUse)
                    emiter.Convert(underlyingType);
                emiter.StoreLocal(tmp);
                emiter.CallSerializerForType(typeToUse, tmp);
            }
        }
    }
}
