using System;
using System.IO;
using BlubLib.Serialization;
using Sigil;

namespace ProudNet.Serializers
{
    public class ArrayWithScalarSerializer : ISerializerCompiler
    {
        public Type HandlesType
        {
            get { throw new NotImplementedException(); }
        }

        public void EmitDeserialize(Emit<Func<BinaryReader, object>> emiter, Local value)
        {
            var elementType = value.LocalType.GetElementType();
            var emptyArray = emiter.DefineLabel(nameof(ArrayWithScalarSerializer) + "EmptyArray" + Guid.NewGuid());
            var end = emiter.DefineLabel(nameof(ArrayWithScalarSerializer) + "End" + Guid.NewGuid());

            using (var length = emiter.DeclareLocal<int>("length"))
            {
                // length = ProudNetBinaryReaderExtensions.ReadScalar(reader)
                emiter.LoadArgument(1);
                emiter.Call(typeof(ProudNetBinaryReaderExtensions).GetMethod(nameof(ProudNetBinaryReaderExtensions.ReadScalar)));
                emiter.StoreLocal(length);

                // if(length < 1) {
                //  value = Array.Empty<>()
                //  return
                // }
                emiter.LoadLocal(length);
                emiter.LoadConstant(1);
                emiter.BranchIfLess(emptyArray);

                // value = new [length]
                emiter.LoadLocal(length);
                emiter.NewArray(elementType);
                emiter.StoreLocal(value);

                // Little optimization for byte arrays
                if (elementType == typeof (byte))
                {
                    // value = reader.ReadBytes(length);
                    emiter.LoadArgument(1);
                    emiter.LoadLocal(length);
                    emiter.CallVirtual(typeof (BinaryReader).GetMethod(nameof(BinaryReader.ReadBytes)));
                    emiter.StoreLocal(value);
                }
                else
                {
                    var loop = emiter.DefineLabel(nameof(ArrayWithScalarSerializer) + "Loop" + Guid.NewGuid());
                    var loopCheck = emiter.DefineLabel(nameof(ArrayWithScalarSerializer) + "LoopCheck" + Guid.NewGuid());

                    using (var element = emiter.DeclareLocal(elementType, "element"))
                    using (var i = emiter.DeclareLocal<int>("i"))
                    {
                        emiter.MarkLabel(loop);
                        emiter.CallDeserializerForType(elementType, element);

                        // value[i] = element
                        emiter.LoadLocal(value);
                        emiter.LoadLocal(i);
                        emiter.LoadLocal(element);
                        emiter.StoreElement(elementType);

                        // ++i
                        emiter.LoadLocal(i);
                        emiter.LoadConstant(1);
                        emiter.Add();
                        emiter.StoreLocal(i);

                        // i < length
                        emiter.MarkLabel(loopCheck);
                        emiter.LoadLocal(i);
                        emiter.LoadLocal(length);
                        emiter.BranchIfLess(loop);
                    }
                }
                emiter.Branch(end);
            }

            // value = Array.Empty<>()
            emiter.MarkLabel(emptyArray);
            emiter.Call(typeof(Array)
                .GetMethod(nameof(Array.Empty))
                .GetGenericMethodDefinition()
                .MakeGenericMethod(elementType));
            emiter.StoreLocal(value);
            emiter.MarkLabel(end);
        }

        public void EmitSerialize(Emit<Action<BinaryWriter, object>> emiter, Local value)
        {
            // ToDo check for null

            var elementType = value.LocalType.GetElementType();
            using (var length = emiter.DeclareLocal<int>("length"))
            {
                // length = value.Length
                emiter.LoadLocal(value);
                emiter.Call(value.LocalType.GetProperty(nameof(Array.Length)).GetMethod);
                emiter.StoreLocal(length);

                // ProudNetBinaryWriterExtensions.WriteScalar(writer, length)
                emiter.LoadArgument(1);
                emiter.LoadLocal(length);
                emiter.Call(typeof(ProudNetBinaryWriterExtensions).GetMethod(nameof(ProudNetBinaryWriterExtensions.WriteScalar)));

                var loop = emiter.DefineLabel(nameof(ArrayWithScalarSerializer) + "Loop" + Guid.NewGuid());
                var loopCheck = emiter.DefineLabel(nameof(ArrayWithScalarSerializer) + "LoopCheck" + Guid.NewGuid());

                using (var element = emiter.DeclareLocal(elementType, "element"))
                using (var i = emiter.DeclareLocal<int>("i"))
                {
                    emiter.Branch(loopCheck);
                    emiter.MarkLabel(loop);

                    // element = value[i]
                    emiter.LoadLocal(value);
                    emiter.LoadLocal(i);
                    emiter.LoadElement(elementType);
                    emiter.StoreLocal(element);

                    emiter.CallSerializerForType(elementType, element);

                    // ++i
                    emiter.LoadLocal(i);
                    emiter.LoadConstant(1);
                    emiter.Add();
                    emiter.StoreLocal(i);

                    // i < length
                    emiter.MarkLabel(loopCheck);
                    emiter.LoadLocal(i);
                    emiter.LoadLocal(length);
                    emiter.BranchIfLess(loop);
                }
            }
        }
    }
}
