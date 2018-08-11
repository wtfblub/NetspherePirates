using System;
using System.IO;
using System.Net;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;

namespace ProudNet.Serialization.Serializers
{
    /// <summary>
    /// Serializes a <see cref="IPEndPoint"/>
    /// </summary>
    /// <remarks>
    /// Serializes the <see cref="IPEndPoint"/> as ProudString address, int16 port
    /// </remarks>
    public class IPEndPointAddressStringSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            return type.IsAssignableFrom(typeof(IPEndPoint));
        }

        public void EmitDeserialize(CompilerContext context, Local value)
        {
            // value = new IPEndPoint(IPAddress.Parse(ProudNetBinaryReaderExtensions.ReadProudString(reader)), reader.ReadUInt16())
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.Call(ReflectionHelper.GetMethod((BinaryReader _) => _.ReadProudString()));
            context.Emit.Call(ReflectionHelper.GetMethod(() => IPAddress.Parse(default(string))));
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.CallVirtual(ReflectionHelper.GetMethod((BinaryReader _) => _.ReadUInt16()));
            context.Emit.NewObject(typeof(IPEndPoint).GetConstructor(new[] { typeof(IPAddress), typeof(int) }));
            context.Emit.StoreLocal(value);
        }

        public void EmitSerialize(CompilerContext context, Local value)
        {
            using (var address = context.Emit.DeclareLocal<string>("str"))
            using (var port = context.Emit.DeclareLocal<ushort>("port"))
            {
                var isNullLabel = context.Emit.DefineLabel();
                var writeLabel = context.Emit.DefineLabel();

                // if (value == null) goto isNull
                context.Emit.LoadLocal(value);
                context.Emit.LoadNull();
                context.Emit.BranchIfEqual(isNullLabel);

                // address = value.Address.ToString()
                context.Emit.LoadLocal(value);
                context.Emit.Call(typeof(IPEndPoint).GetProperty(nameof(IPEndPoint.Address)).GetMethod);
                context.Emit.CallVirtual(ReflectionHelper.GetMethod((IPEndPoint _) => _.ToString()));
                context.Emit.StoreLocal(address);

                // port = (ushort)value.Port
                context.Emit.LoadLocal(value);
                context.Emit.Call(typeof(IPEndPoint).GetProperty(nameof(IPEndPoint.Port)).GetMethod);
                context.Emit.Convert<ushort>();
                context.Emit.StoreLocal(port);
                context.Emit.Branch(writeLabel);

                context.Emit.MarkLabel(isNullLabel);

                // address = "255.255.255.255"
                context.Emit.LoadConstant("255.255.255.255");
                context.Emit.StoreLocal(address);

                context.Emit.MarkLabel(writeLabel);

                // ProudNetBinaryWriterExtensions.WriteProudString(writer, address, false)
                context.Emit.LoadReaderOrWriterParam();
                context.Emit.LoadLocal(address);
                context.Emit.LoadConstant(false);
                context.Emit.Call(ReflectionHelper.GetMethod((BinaryWriter _) => _.WriteProudString(default(string), default(bool))));

                // writer.Write(port)
                context.EmitSerialize(port);
            }
        }
    }
}
