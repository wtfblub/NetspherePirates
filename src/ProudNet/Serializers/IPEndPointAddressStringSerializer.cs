using BlubLib.Serialization;
using Sigil;
using System;
using System.IO;
using System.Net;
using Sigil.NonGeneric;

namespace ProudNet.Serializers
{
    public class IPEndPointAddressStringSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            throw new NotImplementedException();
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            // value = new IPEndPoint(IPAddress.Parse(ProudNetBinaryReaderExtensions.ReadProudString(reader)), reader.ReadUInt16())
            emiter.LoadArgument(1);
            emiter.Call(typeof(ProudNetBinaryReaderExtensions).GetMethod(nameof(ProudNetBinaryReaderExtensions.ReadProudString)));
            emiter.Call(typeof(IPAddress).GetMethod(nameof(IPAddress.Parse)));
            emiter.LoadArgument(1);
            emiter.CallVirtual(typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadUInt16)));
            emiter.NewObject(typeof(IPEndPoint).GetConstructor(new Type[] { typeof(IPAddress), typeof(int) }));
            emiter.StoreLocal(value);
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            using (var address = emiter.DeclareLocal<string>("str"))
            using (var port = emiter.DeclareLocal<ushort>("port"))
            {
                var isNull = emiter.DefineLabel(nameof(IPEndPointAddressStringSerializer) + "IsNull" + Guid.NewGuid());
                var write = emiter.DefineLabel(nameof(IPEndPointAddressStringSerializer) + "Write" + Guid.NewGuid());

                // if (value == null) goto isNull
                emiter.LoadLocal(value);
                emiter.LoadNull();
                emiter.BranchIfEqual(isNull);

                // address = value.Address.ToString()
                emiter.LoadLocal(value);
                emiter.Call(typeof(IPEndPoint).GetProperty(nameof(IPEndPoint.Address)).GetMethod);
                emiter.CallVirtual(typeof(IPAddress).GetMethod(nameof(IPAddress.ToString)));
                emiter.StoreLocal(address);

                // port = (ushort)value.Port
                emiter.LoadLocal(value);
                emiter.Call(typeof(IPEndPoint).GetProperty(nameof(IPEndPoint.Port)).GetMethod);
                emiter.Convert<ushort>();
                emiter.StoreLocal(port);
                emiter.Branch(write);

                emiter.MarkLabel(isNull);

                // address = "255.255.255.255"
                emiter.LoadConstant("255.255.255.255");
                emiter.StoreLocal(address);

                emiter.MarkLabel(write);

                // ProudNetBinaryWriterExtensions.WriteProudString(writer, address, false)
                emiter.LoadArgument(1);
                emiter.LoadLocal(address);
                emiter.LoadConstant(false);
                emiter.Call(typeof(ProudNetBinaryWriterExtensions).GetMethod(nameof(ProudNetBinaryWriterExtensions.WriteProudString)));

                // writer.Write(port)
                emiter.CallSerializerForType(typeof(ushort), port);
            }
        }
    }
}
