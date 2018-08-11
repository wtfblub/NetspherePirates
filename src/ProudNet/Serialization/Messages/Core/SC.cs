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
        [BlubMember(0)]
        [BlubSerializer(typeof(ReadToEndSerializer))]
        public byte[] Data { get; set; }

        public RmiMessage()
        {
        }

        public RmiMessage(byte[] data)
        {
            Data = data;
        }
    }

    [BlubContract]
    internal class EncryptedReliableMessage : ICoreMessage
    {
        [BlubMember(0)]
        public EncryptMode EncryptMode { get; set; }

        [BlubMember(1)]
        public byte[] Data { get; set; }

        public EncryptedReliableMessage()
        {
        }

        public EncryptedReliableMessage(byte[] data, EncryptMode encryptMode)
        {
            Data = data;
            EncryptMode = encryptMode;
        }
    }

    [BlubContract]
    internal class Encrypted_UnReliableMessage : ICoreMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }

        [BlubMember(1)]
        public byte[] Data { get; set; }

        public Encrypted_UnReliableMessage()
        {
        }

        public Encrypted_UnReliableMessage(byte[] data)
        {
            Data = data;
        }
    }

    [BlubContract]
    [BlubSerializer(typeof(Serializer))]
    internal class CompressedMessage : ICoreMessage
    {
        public int DecompressedLength { get; set; }
        public byte[] Data { get; set; }

        public CompressedMessage()
        {
        }

        public CompressedMessage(int decompressedLength, byte[] data)
        {
            DecompressedLength = decompressedLength;
            Data = data;
        }

        internal class Serializer : ISerializer<CompressedMessage>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool CanHandle(Type type)
            {
                return type == typeof(CompressedMessage);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Serialize(BlubSerializer serializer, BinaryWriter writer, CompressedMessage value)
            {
                writer.WriteScalar(value.Data.Length);
                writer.WriteScalar(value.DecompressedLength);
                writer.Write(value.Data);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CompressedMessage Deserialize(BlubSerializer serializer, BinaryReader reader)
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

        [BlubMember(1)]
        public byte[] Data { get; set; }
    }
}
