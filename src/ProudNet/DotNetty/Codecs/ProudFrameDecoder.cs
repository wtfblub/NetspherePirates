using BlubLib.DotNetty.Codecs;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Options;
using ProudNet.Configuration;

namespace ProudNet.DotNetty.Codecs
{
    internal class ProudFrameDecoder : LengthFieldBasedFrameDecoder
    {
        public ProudFrameDecoder(IOptions<NetworkOptions> options)
            : base(ByteOrder.LittleEndian, (int)options.Value.MessageMaxLength, 2, 1, 0, 0, true)
        {
        }

        protected override long GetUnadjustedFrameLength(IByteBuffer buffer, int offset, int length, ByteOrder order)
        {
            var scalarPrefix = buffer.GetByte(offset++);
            if (buffer.ReadableBytes - (offset - buffer.ReaderIndex) < scalarPrefix)
                return scalarPrefix;

            switch (scalarPrefix)
            {
                case 1:
                    return buffer.GetByte(offset) + scalarPrefix;

                case 2:
                    return buffer.GetShortLE(offset) + scalarPrefix;

                case 4:
                    return buffer.GetIntLE(offset) + scalarPrefix;

                default:
                    throw new ProudException("Invalid scalar prefix " + scalarPrefix);
            }
        }

        protected override IByteBuffer ExtractFrame(IChannelHandlerContext context, IByteBuffer buffer, int index,
            int length)
        {
            var bytesToSkip = 2; // magic
            var scalarPrefix = buffer.GetByte(index + bytesToSkip);
            bytesToSkip += 1 + scalarPrefix;
            var frame = buffer.Slice(index + bytesToSkip, length - bytesToSkip);
            frame.Retain();
            return frame;
        }
    }
}
