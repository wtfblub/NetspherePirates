using System;
using System.IO;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;

namespace Netsphere.Network.Serializers
{
    /// <summary>
    /// Serializes an array with a int32 length prefix and the element index as int8 before each element
    /// </summary>
    public class ArrayWithIntPrefixAndIndexSerializer : ISerializerCompiler
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

                context.EmitSerialize(length);

                var loop = context.Emit.DefineLabel();
                var loopCheck = context.Emit.DefineLabel();

                using (var element = context.Emit.DeclareLocal(elementType, "element"))
                using (var i = context.Emit.DeclareLocal<int>("i"))
                {
                    context.Emit.Branch(loopCheck);
                    context.Emit.MarkLabel(loop);

                    // element = value[i]
                    context.Emit.LoadLocal(value);
                    context.Emit.LoadLocal(i);
                    context.Emit.LoadElement(elementType);
                    context.Emit.StoreLocal(element);

                    // writer.Write((byte)i)
                    context.Emit.LoadReaderOrWriterParam();
                    context.Emit.LoadLocal(i);
                    context.Emit.Convert<byte>();
                    context.Emit.CallVirtual(ReflectionHelper.GetMethod((BinaryWriter _) => _.Write(default(byte))));

                    context.EmitSerialize(element);

                    // ++i
                    context.Emit.LoadLocal(i);
                    context.Emit.LoadConstant(1);
                    context.Emit.Add();
                    context.Emit.StoreLocal(i);

                    // i < length
                    context.Emit.MarkLabel(loopCheck);
                    context.Emit.LoadLocal(i);
                    context.Emit.LoadLocal(length);
                    context.Emit.BranchIfLess(loop);
                }
            }
        }

        public void EmitDeserialize(CompilerContext context, Local value)
        {
            var elementType = value.LocalType.GetElementType();
            var emptyArray = context.Emit.DefineLabel();
            var end = context.Emit.DefineLabel();

            using (var length = context.Emit.DeclareLocal<int>("length"))
            {
                context.EmitDeserialize(length);

                // if(length < 1) {
                //  value = Array.Empty<>()
                //  return
                // }
                context.Emit.LoadLocal(length);
                context.Emit.LoadConstant(1);
                context.Emit.BranchIfLess(emptyArray);

                // value = new [length]
                context.Emit.LoadLocal(length);
                context.Emit.NewArray(elementType);
                context.Emit.StoreLocal(value);

                var loop = context.Emit.DefineLabel();
                var loopCheck = context.Emit.DefineLabel();

                using (var element = context.Emit.DeclareLocal(elementType, "element"))
                using (var i = context.Emit.DeclareLocal<int>("i"))
                {
                    context.Emit.MarkLabel(loop);

                    // reader.ReadByte() -> index
                    context.Emit.LoadReaderOrWriterParam();
                    context.Emit.CallVirtual(ReflectionHelper.GetMethod((BinaryReader _) => _.ReadByte()));
                    context.Emit.Pop();

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
                    context.Emit.MarkLabel(loopCheck);
                    context.Emit.LoadLocal(i);
                    context.Emit.LoadLocal(length);
                    context.Emit.BranchIfLess(loop);
                }

                context.Emit.Branch(end);
            }

            // value = Array.Empty<>()
            context.Emit.MarkLabel(emptyArray);
            context.Emit.Call(typeof(Array)
                .GetMethod(nameof(Array.Empty))
                .GetGenericMethodDefinition()
                .MakeGenericMethod(elementType));
            context.Emit.StoreLocal(value);
            context.Emit.MarkLabel(end);
        }
    }
}
