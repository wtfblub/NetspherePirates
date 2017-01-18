using BlubLib.DotNetty.Codecs;
using DotNetty.Buffers;

namespace ProudNet.Codecs
{
    internal class ProudFrameDecoder : LengthFieldBasedFrameDecoder
    {
        public ProudFrameDecoder(int maxFrameLength)
            : base(maxFrameLength, 2, 1)
        { }

        protected override long GetUnadjustedFrameLength(IByteBuffer buffer, int offset, int length, ByteOrder order)
        {
            var scalarPrefix = buffer.GetByte(offset++);
            switch (scalarPrefix)
            {
                case 1:
                    return buffer.ReadableBytes < 1 ? 1 : buffer.GetByte(offset);

                case 2:
                    return buffer.ReadableBytes < 2 ? 2 : buffer.GetShort(offset);

                case 4:
                    return buffer.ReadableBytes < 4 ? 4 : buffer.GetInt(offset);

                default:
                    throw new ProudException("Invalid scalar prefix " + scalarPrefix);
            }
        }
    }
}
