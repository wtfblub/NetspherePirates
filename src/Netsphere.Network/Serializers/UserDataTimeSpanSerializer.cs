using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace Netsphere.Network.Serializers
{
    internal class UserDataTimeSpanSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            throw new NotImplementedException();
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadLocalAddress(value);
            emiter.Call(typeof (TimeSpan).GetProperty(nameof(TimeSpan.TotalSeconds)).GetMethod);
            emiter.Convert<uint>();
            emiter.CallVirtual(typeof (BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] {typeof (uint)}));
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.Call(typeof (BinaryReader).GetMethod(nameof(BinaryReader.ReadUInt32)));
            emiter.Convert<double>();
            emiter.Call(typeof (TimeSpan).GetMethod(nameof(TimeSpan.FromSeconds)));
            emiter.StoreLocal(value);
        }
    }
}
