using System;
using System.IO;
using System.Numerics;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;

namespace Netsphere.Network.Serializers
{
    /// <summary>
    /// Serializes rotation <see cref="Vector2"/>
    /// </summary>
    public class RotationVectorSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            return type == typeof(Vector2);
        }

        public void EmitSerialize(CompilerContext context, Local value)
        {
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.LoadLocal(value);
            context.Emit.Call(ReflectionHelper.GetMethod((BinaryWriter _) => _.WriteRotation(default(Vector2))));
        }

        public void EmitDeserialize(CompilerContext context, Local value)
        {
            context.Emit.LoadReaderOrWriterParam();
            context.Emit.Call(ReflectionHelper.GetMethod((BinaryReader _) => _.ReadRotation()));
            context.Emit.StoreLocal(value);
        }
    }
}
