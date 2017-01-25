using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace ProudNet.Codecs
{
    internal class UdpFrameDecoder : MessageToMessageDecoder<DatagramPacket>
    {
        private readonly int _maxFrameLength;

        public UdpFrameDecoder(int maxFrameLength)
        {
            _maxFrameLength = maxFrameLength;
        }

        protected override void Decode(IChannelHandlerContext context, DatagramPacket message, List<object> output)
        {
            var content = message.Content.WithOrder(ByteOrder.LittleEndian);
            var flag = content.ReadUnsignedShort();
            var sessionId = content.ReadUnsignedShort();
            var length = content.ReadInt();
            var id = content.ReadUnsignedInt();
            var fragId = content.ReadUnsignedInt();

            if (length > _maxFrameLength)
                throw new TooLongFrameException("Received message is too long");

            var buffer = content
                .SkipBytes(2)
                .ReadStruct();

            // ReadStruct uses a slice
            content.Retain();

            output.Add(new UdpMessage
            {
                Flag = flag,
                SessionId = sessionId,
                Length = length,
                Id = id,
                FragId = fragId,
                Content = buffer
            });
        }
    }
}
