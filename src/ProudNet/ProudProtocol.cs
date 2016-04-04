using System.IO;
using BlubLib.IO;
using BlubLib.Network;
using BlubLib.Network.Message;
using BlubLib.Network.Pipes;
using ProudNet.Message.Core;

namespace ProudNet
{
    public class ProudProtocol : IProtocol
    {
        private const short Magic = 0x5713;

        public IMessage Serialize(ISession session, IMessage message)
        {
            return new ProudNetSerializeMessage(session, message);
        }

        public bool CanDeserialize(ISession session, Stream stream)
        {
            if (stream.Length < 3) // magic + scalar prefix
                return false;

            // skip magic
            stream.Seek(2, SeekOrigin.Current);

            var scalarPrefix = stream.ReadByte();

            // magic + scalar
            if (stream.Length < (3 + scalarPrefix))
                return false;

            int length;
            switch (scalarPrefix)
            {
                case 1:
                    length = stream.ReadByte();
                    break;

                case 2:
                    length = stream.ReadByte() | stream.ReadByte() << 8;
                    break;

                case 4:
                    length = stream.ReadByte() | stream.ReadByte() << 8 | stream.ReadByte() << 16 | stream.ReadByte() << 24;
                    break;

                default:
                    throw new ProudException("Invalid scalar prefix " + scalarPrefix);
            }

            // (magic + scalar) + length
            var messageLength = (3 + scalarPrefix) + length;
            return stream.Length >= messageLength;
        }

        public IMessage Deserialize(ISession session, Stream stream)
        {
            using (var r = stream.ToBinaryReader(true))
            {
                var magic = r.ReadInt16();
                if (magic != Magic)
                    throw new ProudException("Invalid magic: " + magic);

                var length = r.ReadScalar() - 1; // Exclude opCode
                var opCode = r.ReadEnum<ProudCoreOpCode>();

                var position = (int)stream.Position;
                stream.Seek(length, SeekOrigin.Current);

                using (var r2 = new BinaryReader(new LimitedStream(stream, position, length)))
                {
                    var message = CoreMapper.GetMessage(opCode, r2);
                    if (!r2.IsEOF())
#if DEBUG
                    {
                        r2.BaseStream.Position = 0;
                        throw new ProudBadFormatException(message.GetType(), r2.ReadToEnd());
                    }
#else
                        throw new ProudBadFormatException(message.GetType());
#endif
                    return message;
                }
            }
        }

        private class ProudNetSerializeMessage : IMessage
        {
            private readonly ISession _session;
            private readonly IMessage _message;

            public ProudNetSerializeMessage(ISession session, IMessage message)
            {
                _session = session;
                _message = message;
            }

            public void Serialize(Stream stream)
            {
                using (var ms = new PooledMemoryStream(_session.Service.ArrayPool))
                {
                    _message.Serialize(ms);
                    var segment = ms.ToSegment();

                    using (var w = stream.ToBinaryWriter(true))
                    {
                        w.Write(Magic);
                        w.WriteScalar(segment.Count);
                        w.Write(segment.Array, segment.Offset, segment.Count);
                    }
                }
            }
        }
    }
}
