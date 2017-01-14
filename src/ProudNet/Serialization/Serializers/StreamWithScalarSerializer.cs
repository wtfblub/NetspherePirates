using System;
using System.IO;
using BlubLib.IO;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace ProudNet.Serialization.Serializers
{
    public class StreamWithScalarSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            throw new NotImplementedException();
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            // writer.WriteScalar(value.Length)
            emiter
                .LoadArgument(1)
                .LoadLocal(value)
                .CallVirtual(typeof(Stream).GetProperty(nameof(Stream.Length)).GetMethod)
                .Convert<int>()
                .Call(ReflectionHelper.GetMethod((BinaryWriter x) => x.WriteScalar(default(int))));

            // value.CopyTo(writer.BaseStream)
            emiter
                .LoadLocal(value)
                .LoadArgument(1)
                .CallVirtual(typeof(BinaryWriter).GetProperty(nameof(BinaryWriter.BaseStream)).GetMethod)
                .Call(ReflectionHelper.GetMethod((Stream x) => x.CopyTo(default(Stream))));
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            using (var length = emiter.DeclareLocal<int>())
            using (var stream = emiter.DeclareLocal<Stream>())
            {
                // var length = reader.ReadScalar
                emiter
                    .LoadArgument(1)
                    .Call(ReflectionHelper.GetMethod((BinaryReader x) => x.ReadScalar()))
                    .StoreLocal(length);

                // var stream = reader.BaseStream
                emiter
                    .LoadArgument(1)
                    .CallVirtual(typeof(BinaryReader).GetProperty(nameof(BinaryReader.BaseStream)).GetMethod)
                    .StoreLocal(stream)
                    .LoadLocal(stream);

                // value = new LimitedStream(stream, stream.Position, length)
                emiter
                    .LoadLocal(stream)
                    .CallVirtual(typeof(Stream).GetProperty(nameof(Stream.Position)).GetMethod)

                    .LoadLocal(length)
                    .Convert<long>()

                    .NewObject(typeof(LimitedStream), typeof(Stream), typeof(long), typeof(long))
                    .StoreLocal(value);
            }
        }
    }
}
