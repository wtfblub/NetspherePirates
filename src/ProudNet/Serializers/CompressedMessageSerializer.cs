using System;
using System.IO;
using BlubLib.Serialization;
using ProudNet.Message.Core;
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

                // value.Data = reader.ReadBytes(compressedLength)
                emiter.LoadLocal(value);
                emiter.LoadArgument(1);
                emiter.LoadLocal(compressedLength);
                emiter.CallVirtual(typeof (BinaryReader).GetMethod(nameof(BinaryReader.ReadBytes)));
                emiter.Call(typeof (CompressedMessage).GetProperty(nameof(CompressedMessage.Data)).SetMethod);
            }
        }

        public void EmitSerialize(Emit<Action<BinaryWriter, object>> emiter, Local value)
        {
            using (var data = emiter.DeclareLocal<byte[]>("data"))
            {
                // data = value.Data
                emiter.LoadLocal(value);
                emiter.Call(typeof(CompressedMessage).GetProperty(nameof(CompressedMessage.Data)).GetMethod);
                emiter.StoreLocal(data);

                // ProudNetBinaryWriterExtensions.WriteScalar(writer, data.Length)
                emiter.LoadArgument(1);
                emiter.LoadLocal(data);
                emiter.CallVirtual(typeof(Array).GetProperty(nameof(Array.Length)).GetMethod);
                emiter.Call(typeof(ProudNetBinaryWriterExtensions).GetMethod(nameof(ProudNetBinaryWriterExtensions.WriteScalar)));

                // ProudNetBinaryWriterExtensions.WriteScalar(writer, value.DecompressedLength)
                emiter.LoadArgument(1);
                emiter.LoadLocal(value);
                emiter.Call(typeof(CompressedMessage).GetProperty(nameof(CompressedMessage.DecompressedLength)).GetMethod);
                emiter.Call(typeof(ProudNetBinaryWriterExtensions).GetMethod(nameof(ProudNetBinaryWriterExtensions.WriteScalar)));

                // writer.Write(data)
                emiter.LoadArgument(1);
                emiter.LoadLocal(data);
                emiter.CallVirtual(typeof (BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] {typeof (byte[])}));
            }
        }
    }
}
