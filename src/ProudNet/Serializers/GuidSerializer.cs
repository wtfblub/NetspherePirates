using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;

namespace ProudNet.Serializers
{
    internal class GuidSerializer : ISerializerCompiler
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
            // value = new Guid(reader.ReadBytes(16))
            emiter.LoadLocalAddress(value);
            emiter.LoadArgument(1);
            emiter.LoadConstant(16);
            emiter.CallVirtual(typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadBytes)));
            emiter.Call(typeof(Guid).GetConstructor(new Type[] { typeof(byte[]) }));
        }

        public void EmitSerialize(Emit<Action<BinaryWriter, object>> emiter, Local value)
        {
            // writer.Write(value.ToByteArray())
            emiter.LoadArgument(1);
            emiter.LoadLocalAddress(value);
            emiter.Call(typeof(Guid).GetMethod(nameof(Guid.ToByteArray)));
            emiter.CallVirtual(typeof(BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new Type[] { typeof(byte[]) }));
        }
    }
}
