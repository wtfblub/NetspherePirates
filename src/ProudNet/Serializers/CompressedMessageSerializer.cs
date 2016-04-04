using System;
using System.IO;
using BlubLib.Serialization;
using ProudNet.Message.Core;
using BlubLib.IO;
using Sigil;

namespace ProudNet.Serializers
{
    internal class CompressedMessageSerializer : ISerializerCompiler
    {
        public Type HandlesType => typeof(CompressedMessage);

        public void EmitDeserialize(Emit<Func<BinaryReader, object>> emiter, Local value)
        {
            // value = new CompressedMessage()
            emiter.NewObject(typeof(CompressedMessage));
            emiter.StoreLocal(value);

            using (var compressedLength = emiter.DeclareLocal<int>("compressedLength"))
            {
                // compressedLength = ProudNetBinaryReaderExtensions.ReadScalar(reader)
                emiter.LoadArgument(1);
                emiter.Call(typeof(ProudNetBinaryReaderExtensions).GetMethod(nameof(ProudNetBinaryReaderExtensions.ReadScalar)));
                emiter.StoreLocal(compressedLength);

                // value.DecompressedLength = ProudNetBinaryReaderExtensions.ReadScalar(reader)
                emiter.LoadLocal(value);
                emiter.LoadArgument(1);
                emiter.Call(typeof(ProudNetBinaryReaderExtensions).GetMethod(nameof(ProudNetBinaryReaderExtensions.ReadScalar)));
                emiter.Call(typeof(CompressedMessage).GetProperty(nameof(CompressedMessage.DecompressedLength)).SetMethod);

                using (var limitedStream = emiter.DeclareLocal<Stream>("limitedStream"))
                {
                    using (var baseStream = emiter.DeclareLocal<Stream>("baseStream"))
                    {
                        // baseStream = reader.BaseStream
                        emiter.LoadArgument(1);
                        emiter.CallVirtual(typeof(BinaryReader).GetProperty(nameof(BinaryReader.BaseStream)).GetMethod);
                        emiter.StoreLocal(baseStream);

                        // limitedStream = new LimitedStream(baseStream, baseStream.Position, compressedLength)
                        emiter.LoadLocal(baseStream);
                        emiter.LoadLocal(baseStream);
                        emiter.CallVirtual(typeof(Stream).GetProperty(nameof(Stream.Position)).GetMethod);
                        emiter.Convert<int>();
                        emiter.LoadLocal(compressedLength);
                        emiter.NewObject(typeof(LimitedStream), new Type[] { typeof(Stream), typeof(int), typeof(int) });
                        emiter.StoreLocal(limitedStream);

                        // baseStream.Seek(compressedLength, SeekOrigin.Current)
                        emiter.LoadLocal(baseStream);
                        emiter.LoadLocal(compressedLength);
                        emiter.Convert<long>();
                        emiter.LoadConstant((int)SeekOrigin.Current);
                        emiter.CallVirtual(typeof(Stream).GetMethod(nameof(Stream.Seek)));
                        emiter.Pop();
                    }

                    // value.Data = limitedStream
                    emiter.LoadLocal(value);
                    emiter.LoadLocal(limitedStream);
                    emiter.Call(typeof(CompressedMessage).GetProperty(nameof(CompressedMessage.Data)).SetMethod);
                }
            }
        }

        public void EmitSerialize(Emit<Action<BinaryWriter, object>> emiter, Local value)
        {
            using (var stream = emiter.DeclareLocal<Stream>("stream"))
            {
                // stream = value.Data
                emiter.LoadLocal(value);
                emiter.Call(typeof(CompressedMessage).GetProperty(nameof(CompressedMessage.Data)).GetMethod);
                emiter.StoreLocal(stream);

                // ProudNetBinaryWriterExtensions.WriteScalar(writer, stream.Length)
                emiter.LoadArgument(1);
                emiter.LoadLocal(stream);
                emiter.CallVirtual(typeof(Stream).GetProperty(nameof(Stream.Length)).GetMethod);
                emiter.Convert<int>();
                emiter.Call(typeof(ProudNetBinaryWriterExtensions).GetMethod(nameof(ProudNetBinaryWriterExtensions.WriteScalar)));

                // ProudNetBinaryWriterExtensions.WriteScalar(writer, value.DecompressedLength)
                emiter.LoadArgument(1);
                emiter.LoadLocal(value);
                emiter.Call(typeof(CompressedMessage).GetProperty(nameof(CompressedMessage.DecompressedLength)).GetMethod);
                emiter.Call(typeof(ProudNetBinaryWriterExtensions).GetMethod(nameof(ProudNetBinaryWriterExtensions.WriteScalar)));

                // stream.CopyTo(writer.BaseStream)
                emiter.LoadLocal(stream);
                emiter.LoadArgument(1);
                emiter.CallVirtual(typeof(BinaryWriter).GetProperty(nameof(BinaryWriter.BaseStream)).GetMethod);
                emiter.CallVirtual(typeof(Stream).GetMethod(nameof(Stream.CopyTo), new Type[] { typeof(Stream) }));
            }
        }
    }
}
