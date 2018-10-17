using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace ProudNet.DotNetty.Codecs
{
    internal class ProudFrameEncoder : MessageToMessageEncoder<IByteBuffer>
    {
        protected override void Encode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            var buffer = context.Allocator
                .Buffer(2 + 1 + message.ReadableBytes)
                .WriteShortLE(Constants.NetMagic)
                .WriteScalar(message.ReadableBytes);

            output.Add(buffer);
            output.Add(message.Retain());
        }
    }
}
