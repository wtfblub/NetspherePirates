using System;
using System.IO;
using System.Net;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace ProudNet.Serialization.Serializers
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
            emiter.Call(ReflectionHelper.GetMethod((BinaryReader x) => x.ReadProudString()));
            emiter.Call(ReflectionHelper.GetMethod(() => IPAddress.Parse(default(string))));
            emiter.LoadArgument(1);
            emiter.CallVirtual(ReflectionHelper.GetMethod((BinaryReader x) => x.ReadUInt16()));
            emiter.NewObject(typeof(IPEndPoint).GetConstructor(new[] { typeof(IPAddress), typeof(int) }));
            emiter.StoreLocal(value);
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            using (var address = emiter.DeclareLocal<string>("str"))
            using (var port = emiter.DeclareLocal<ushort>("port"))
            {
                var isNull = emiter.DefineLabel();
                var write = emiter.DefineLabel();

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
                emiter.Call(ReflectionHelper.GetMethod((BinaryWriter x) => x.WriteProudString(default(string), default(bool))));

                // writer.Write(port)
                emiter.CallSerializerForType(typeof(ushort), port);
            }
        }
    }
}
