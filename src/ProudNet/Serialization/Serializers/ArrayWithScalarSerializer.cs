using System;
using System.IO;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;

namespace ProudNet.Serialization.Serializers
{
    /// <summary>
    /// Serializes an array with a scalar length prefix
    /// </summary>
    public class ArrayWithScalarSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            return type.IsArray || typeof(Array).IsAssignableFrom(type);
        }

        public void EmitSerialize(CompilerContext context, Local value)
        {
            var elementType = value.LocalType.GetElementType();
            using (var length = context.Emit.DeclareLocal<int>("length"))
            {
                var writeLabel = context.Emit.DefineLabel();

                // if (value != null) goto write
                context.Emit.LoadLocal(value);
                context.Emit.LoadNull();
                context.Emit.CompareEqual();
                context.Emit.BranchIfFalse(writeLabel);

                // value = Array.Empty<>()
                context.Emit.Call(typeof(Array)
                    .GetMethod(nameof(Array.Empty))
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(elementType));
                context.Emit.StoreLocal(value);

                // length = value.Length
                context.Emit.MarkLabel(writeLabel);
                context.Emit.LoadLocal(value);
                context.Emit.Call(value.LocalType.GetProperty(nameof(Array.Length)).GetMethod);
                context.Emit.StoreLocal(length);

                // ProudNetBinaryWriterExtensions.WriteScalar(writer, length)
                context.Emit.LoadReaderOrWriterParam();
                context.Emit.LoadLocal(length);
                context.Emit.Call(ReflectionHelper.GetMethod((BinaryWriter _) => _.WriteScalar(default(int))));

                var loopLabel = context.Emit.DefineLabel();
                var loopCheckLabel = context.Emit.DefineLabel();

                using (var element = context.Emit.DeclareLocal(elementType, "element"))
                using (var i = context.Emit.DeclareLocal<int>("i"))
                {
                    context.Emit.Branch(loopCheckLabel);
                    context.Emit.MarkLabel(loopLabel);

                    // element = value[i]
                    context.Emit.LoadLocal(value);
                    context.Emit.LoadLocal(i);
                    context.Emit.LoadElement(elementType);
                    context.Emit.StoreLocal(element);

                    context.EmitSerialize(element);

                    // ++i
                    context.Emit.LoadLocal(i);
                    context.Emit.LoadConstant(1);
                    context.Emit.Add();
                    context.Emit.StoreLocal(i);

                    // i < length
                    context.Emit.MarkLabel(loopCheckLabel);
                    context.Emit.LoadLocal(i);
                    context.Emit.LoadLocal(length);
                    context.Emit.BranchIfLess(loopLabel);
                }
            }
        }

        public void EmitDeserialize(CompilerContext context, Local value)
        {
            var elementType = value.LocalType.GetElementType();
            var emptyArrayLabel = context.Emit.DefineLabel();
            var endLabel = context.Emit.DefineLabel();

            using (var length = context.Emit.DeclareLocal<int>("length"))
            {
                // length = ProudNetBinaryReaderExtensions.ReadScalar(reader)
                context.Emit.LoadReaderOrWriterParam();
                context.Emit.Call(ReflectionHelper.GetMethod((BinaryReader _) => _.ReadScalar()));
                context.Emit.StoreLocal(length);

                // if(length < 1) {
                //  value = Array.Empty<>()
                //  return
                // }
                context.Emit.LoadLocal(length);
                context.Emit.LoadConstant(1);
                context.Emit.BranchIfLess(emptyArrayLabel);

                // value = new [length]
                context.Emit.LoadLocal(length);
                context.Emit.NewArray(elementType);
                context.Emit.StoreLocal(value);

                // Little optimization for byte arrays
                if (elementType == typeof(byte))
                {
                    // value = reader.ReadBytes(length);
                    context.Emit.LoadReaderOrWriterParam();
                    context.Emit.LoadLocal(length);
                    context.Emit.Call(ReflectionHelper.GetMethod((BinaryReader _) => _.ReadBytes(default(int))));
                    context.Emit.StoreLocal(value);
                }
                else
                {
                    var loopLabel = context.Emit.DefineLabel();
                    var loopCheckLabel = context.Emit.DefineLabel();

                    using (var element = context.Emit.DeclareLocal(elementType, "element"))
                    using (var i = context.Emit.DeclareLocal<int>("i"))
                    {
                        context.Emit.MarkLabel(loopLabel);
                        context.EmitDeserialize(element);

                        // value[i] = element
                        context.Emit.LoadLocal(value);
                        context.Emit.LoadLocal(i);
                        context.Emit.LoadLocal(element);
                        context.Emit.StoreElement(elementType);

                        // ++i
                        context.Emit.LoadLocal(i);
                        context.Emit.LoadConstant(1);
                        context.Emit.Add();
                        context.Emit.StoreLocal(i);

                        // i < length
                        context.Emit.MarkLabel(loopCheckLabel);
                        context.Emit.LoadLocal(i);
                        context.Emit.LoadLocal(length);
                        context.Emit.BranchIfLess(loopLabel);
                    }
                }

                context.Emit.Branch(endLabel);
            }

            // value = Array.Empty<>()
            context.Emit.MarkLabel(emptyArrayLabel);
            context.Emit.Call(typeof(Array)
                .GetMethod(nameof(Array.Empty))
                .GetGenericMethodDefinition()
                .MakeGenericMethod(elementType));
            context.Emit.StoreLocal(value);
            context.Emit.MarkLabel(endLabel);
        }
    }
}
