using BlubLib.DotNetty.Codecs;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace ProudNet.Codecs
{
    internal class ProudFrameDecoder : LengthFieldBasedFrameDecoder
    {
        public ProudFrameDecoder(int maxFrameLength)
            : base(maxFrameLength, 2, 1)
        { }

        protected override long GetUnadjustedFrameLength(IByteBuffer buffer, int offset, int length, ByteOrder order)
        {
            buffer = buffer.WithOrder(ByteOrder.LittleEndian);
            var scalarPrefix = buffer.GetByte(offset++);
            switch (scalarPrefix)
            {
                case 1:
                    return buffer.ReadableBytes < 1 ? 1 : buffer.GetByte(offset) + 1;

                case 2:
                    return buffer.ReadableBytes < 2 ? 2 : buffer.GetShort(offset) + 2;

                case 4:
                    return buffer.ReadableBytes < 4 ? 4 : buffer.GetInt(offset) + 4;

                default:
                    throw new ProudException("Invalid scalar prefix " + scalarPrefix);
            }
        }

        protected override IByteBuffer ExtractFrame(IChannelHandlerContext context, IByteBuffer buffer, int index, int length)
        {
            var bytesToSkip = 2; // magic
            var scalarPrefix = buffer.GetByte(index + bytesToSkip);
            ++bytesToSkip;

            switch (scalarPrefix)
            {
                case 1:
                    ++bytesToSkip;
                    break;

                case 2:
                    bytesToSkip += 2;
                    break;

                case 4:
                    bytesToSkip += 4;
                    break;
            }

            var frame = buffer.Slice(index + bytesToSkip, length - bytesToSkip);
            frame.Retain();
            return frame;
        }
    }
}
