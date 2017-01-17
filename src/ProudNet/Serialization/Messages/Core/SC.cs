using System;
using System.IO;
using System.Runtime.CompilerServices;
using BlubLib.Serialization;
using ProudNet.Serialization.Serializers;

namespace ProudNet.Serialization.Messages.Core
{
    [BlubContract]
    internal class RmiMessage : ICoreMessage
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
    internal class EncryptedReliableMessage : ICoreMessage
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
    internal class CompressedMessage : ICoreMessage
    {
        public int DecompressedLength { get; set; }
        public byte[] Data { get; set; }

        public CompressedMessage()
        { }

        public CompressedMessage(int decompressedLength, byte[] data)
        {
            DecompressedLength = decompressedLength;
            Data = data;
        }

        internal class Serializer : ISerializer<CompressedMessage>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool CanHandle(Type type) => type == typeof(CompressedMessage);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Serialize(BinaryWriter writer, CompressedMessage value)
            {
                writer.WriteScalar((int)value.Data.Length);
                writer.WriteScalar(value.DecompressedLength);
                writer.Write(value.Data);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CompressedMessage Deserialize(BinaryReader reader)
            {
                var length = reader.ReadScalar();
                return new CompressedMessage(reader.ReadScalar(), reader.ReadBytes(length));
            }
        }
    }

    [BlubContract]
    internal class ReliableUdp_FrameMessage : ICoreMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }

        [BlubMember(1, typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; }
    }
}
