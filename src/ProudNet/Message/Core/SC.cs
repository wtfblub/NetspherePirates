using System;
using BlubLib.Serialization;
using ProudNet.Serializers;

namespace ProudNet.Message.Core
{
    internal class RmiMessage : CoreMessage
    {
        [Serialize(0, typeof(ReadToEndSerializer))]
        public byte[] Data { get; set; }

        public RmiMessage()
        { }

        public RmiMessage(byte[] data)
        {
            Data = data;
        }
    }

    internal class EncryptedReliableMessage : CoreMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }

        [Serialize(1, typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; }

        public EncryptedReliableMessage()
        { }

        public EncryptedReliableMessage(byte[] data)
        {
            Data = data;
        }
    }

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
    }

    // ReSharper disable once InconsistentNaming
    internal class ReliableUdp_FrameMessage : CoreMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }

        [Serialize(1, typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; }
    }
}
