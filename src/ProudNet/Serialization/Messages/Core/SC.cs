using System;
using System.IO;
using System.Runtime.CompilerServices;
using BlubLib.IO;
using BlubLib.Serialization;
using ProudNet.Serialization.Serializers;

namespace ProudNet.Serialization.Messages.Core
{
    [BlubContract]
    internal class RmiMessage
    {
        [BlubMember(0, typeof(StreamSerializer))]
        public Stream Data { get; set; }

        public RmiMessage()
        { }

        public RmiMessage(Stream data)
        {
            Data = data;
        }
    }

    [BlubContract]
    internal class EncryptedReliableMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }

        [BlubMember(1, typeof(StreamWithScalarSerializer))]
        public Stream Data { get; set; }

        public EncryptedReliableMessage()
        { }

        public EncryptedReliableMessage(Stream data)
        {
            Data = data;
        }
    }

    [BlubContract(typeof(Serializer))]
    internal class CompressedMessage
    {
        public int DecompressedLength { get; set; }
        public Stream Data { get; set; }

        public CompressedMessage()
        { }

        public CompressedMessage(int decompressedLength, Stream data)
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
                value.Data.CopyTo(writer.BaseStream);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CompressedMessage Deserialize(BinaryReader reader)
            {
                var length = reader.ReadScalar();
                return new CompressedMessage(reader.ReadScalar(), new LimitedStream(reader.BaseStream, reader.BaseStream.Position, length));
            }
        }
    }

    [BlubContract]
    internal class ReliableUdp_FrameMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }

        [BlubMember(1, typeof(StreamWithScalarSerializer))]
        public Stream Data { get; set; }
    }
}
