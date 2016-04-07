using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;

namespace Netsphere.Network.Serializers
{
    internal class TimeSpanSerializer : ISerializerCompiler
    {
        public Type HandlesType
        {
            get { throw new NotImplementedException(); }
        }

        public void EmitSerialize(Emit<Action<BinaryWriter, object>> emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadLocalAddress(value);
            emiter.Call(typeof (TimeSpan).GetProperty(nameof(TimeSpan.TotalMinutes)).GetMethod);
            emiter.Convert<byte>();
            emiter.CallVirtual(typeof (BinaryWriter).GetMethod(nameof(BinaryWriter), new[] {typeof (byte)}));
        }

        public void EmitDeserialize(Emit<Func<BinaryReader, object>> emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.Call(typeof (BinaryReader).GetMethod(nameof(BinaryReader.ReadByte)));
            emiter.Convert<double>();
            emiter.Call(typeof (TimeSpan).GetMethod(nameof(TimeSpan.FromMinutes)));
            emiter.StoreLocal(value);
        }
    }
}
