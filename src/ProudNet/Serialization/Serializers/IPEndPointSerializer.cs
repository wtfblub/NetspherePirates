using System;
using System.IO;
using System.Net;
using BlubLib.IO;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;

namespace ProudNet.Serialization.Serializers
{
    /// <summary>
    /// Serializes a <see cref="IPEndPoint"/>
    /// </summary>
    /// <remarks>
    /// Serializes the <see cref="IPEndPoint"/> as int32 address, int16 port
    /// </remarks>
    public class IPEndPointSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            return type.IsAssignableFrom(typeof(IPEndPoint));
        }

        public void EmitDeserialize(CompilerContext context, Local value)
        {
            // value = BinaryReaderExtensions.ReadIPEndPoint(reader)
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.Call(ReflectionHelper.GetMethod((BinaryReader _) => _.ReadIPEndPoint()));
            context.Emit.StoreLocal(value);
        }

        public void EmitSerialize(CompilerContext context, Local value)
        {
            var writeLabel = context.Emit.DefineLabel();

            // if (value != null) goto write
            context.Emit.LoadLocal(value);
            context.Emit.LoadNull();
            context.Emit.CompareEqual();
            context.Emit.BranchIfFalse(writeLabel);

            // value = Constants.EmptyIPEndPoint
            context.Emit.LoadField(typeof(Constants).GetField(nameof(Constants.EmptyIPEndPoint)));
            context.Emit.StoreLocal(value);

            // BinaryWriterExtensions.Write(writer, value)
            context.Emit.MarkLabel(writeLabel);
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.LoadLocal(value);
            context.Emit.Call(ReflectionHelper.GetMethod((BinaryWriter _) => _.Write(default(IPEndPoint))));
        }
    }
}
