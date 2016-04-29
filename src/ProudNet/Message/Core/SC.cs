using System;
using System.IO;
using BlubLib.Serialization;
using ProudNet.Serializers;
using Sigil;
using Sigil.NonGeneric;

namespace ProudNet.Message.Core
{
    [BlubContract]
    internal class RmiMessage : CoreMessage
    {
        [BlubMember(0, typeof(ReadToEndSerializer))]
        public byte[] Data { get; set; }

        public RmiMessage()
        { }

        public RmiMessage(byte[] data)
        {
            Data = data;
        }
    }

    [BlubContract]
    internal class EncryptedReliableMessage : CoreMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }

        [BlubMember(1, typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; }

        public EncryptedReliableMessage()
        { }

        public EncryptedReliableMessage(byte[] data)
        {
            Data = data;
        }
    }

    [BlubContract(typeof(Serializer))]
    internal class CompressedMessage : CoreMessage
    {
        public int DecompressedLength { get; set; }
        public byte[] Data { get; set; }

        public CompressedMessage()
        {
            Data = Array.Empty<byte>();
        }

        public CompressedMessage(int decompressedLength, byte[] data)
        {
            DecompressedLength = decompressedLength;
            Data = data;
        }

        internal class Serializer : ISerializerCompiler
        {
            public bool CanHandle(Type type) => type == typeof(CompressedMessage);

            public void EmitDeserialize(Emit emiter, Local value)
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
                    emiter.CallVirtual(typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadBytes)));
                    emiter.Call(typeof(CompressedMessage).GetProperty(nameof(CompressedMessage.Data)).SetMethod);
                }
            }

            public void EmitSerialize(Emit emiter, Local value)
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
                    emiter.CallVirtual(typeof(BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] { typeof(byte[]) }));
                }
            }
        }
    }

    [BlubContract]
    internal class ReliableUdp_FrameMessage : CoreMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }

        [BlubMember(1, typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; }
    }
}
