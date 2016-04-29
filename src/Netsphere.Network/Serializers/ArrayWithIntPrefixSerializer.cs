using System;
using System.IO;
using BlubLib.Serialization;
using ProudNet.Serializers;
using Sigil;
using Sigil.NonGeneric;

namespace Netsphere.Network.Serializers
{
    internal class ArrayWithIntPrefixSerializer : ISerializerCompiler
    {
        private readonly ISerializer _serializer;
        private readonly ISerializerCompiler _compiler;

        public bool CanHandle(Type type)
        {
            throw new NotImplementedException();
        }

        public ArrayWithIntPrefixSerializer()
        { }

        public ArrayWithIntPrefixSerializer(Type serializer, object[] parameters = null)
        {
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            if (typeof(ISerializer).IsAssignableFrom(serializer))
                _serializer = (ISerializer)Activator.CreateInstance(serializer, parameters);
            else if (typeof(ISerializerCompiler).IsAssignableFrom(serializer))
                _compiler = (ISerializerCompiler)Activator.CreateInstance(serializer, parameters);
            else
                throw new ArgumentException($"{serializer.FullName} must be a ISerializer or ISerializerCompiler", nameof(serializer));
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            var elementType = value.LocalType.GetElementType();
            var emptyArray = emiter.DefineLabel(nameof(ArrayWithScalarSerializer) + "EmptyArray" + Guid.NewGuid());
            var end = emiter.DefineLabel(nameof(ArrayWithScalarSerializer) + "End" + Guid.NewGuid());

            using (var length = emiter.DeclareLocal<int>("length"))
            {
                emiter.CallDeserializerForType(length.LocalType, length);

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

                var loop = emiter.DefineLabel(nameof(ArrayWithScalarSerializer) + "Loop" + Guid.NewGuid());
                var loopCheck = emiter.DefineLabel(nameof(ArrayWithScalarSerializer) + "LoopCheck" + Guid.NewGuid());

                // Little optimization for byte arrays
                if (elementType == typeof(byte))
                {
                    // value = reader.ReadBytes(length);
                    emiter.LoadArgument(1);
                    emiter.LoadLocal(length);
                    emiter.CallVirtual(typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadBytes)));
                    emiter.StoreLocal(value);
                    emiter.Branch(end);
                }
                else
                {
                    using (var element = emiter.DeclareLocal(elementType, "element"))
                    using (var i = emiter.DeclareLocal<int>("i"))
                    {
                        emiter.MarkLabel(loop);

                        if (_compiler != null)
                            _compiler.EmitDeserialize(emiter, element);
                        else if (_serializer != null)
                            emiter.CallDeserializer(_serializer, element);
                        else
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

        public void EmitSerialize(Emit emiter, Local value)
        {
            var elementType = value.LocalType.GetElementType();
            using (var length = emiter.DeclareLocal<int>("length"))
            {
                // length = value.Length
                emiter.LoadLocal(value);
                emiter.Call(value.LocalType.GetProperty(nameof(Array.Length)).GetMethod);
                emiter.StoreLocal(length);

                emiter.CallSerializerForType(length.LocalType, length);

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

                    if (_compiler != null)
                        _compiler.EmitSerialize(emiter, element);
                    else if (_serializer != null)
                        emiter.CallSerializer(_serializer, element);
                    else
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
