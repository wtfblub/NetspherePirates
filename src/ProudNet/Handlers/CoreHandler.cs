using BlubLib.DotNetty.Handlers.MessageHandling;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using ProudNet.Codecs;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Handlers
{
    internal class CoreHandler : MessageHandler
    {
        [MessageHandler(typeof(RmiMessage))]
        public void RmiMessage(IChannelHandlerContext context, RmiMessage message)
        {
            var buffer = Unpooled.WrappedBuffer(message.Data);
            try
            {
                context.FireChannelRead(buffer);
            }
            finally
            {
                buffer.Release();
            }
        }

        [MessageHandler(typeof(CompressedMessage))]
        public void CompressedMessage(IChannelHandlerContext context, CompressedMessage message)
        {
            var decompressed = message.Data.DecompressZLib();
            var buffer = Unpooled.WrappedBuffer(decompressed);
            try
            {
                context.Channel.Pipeline.Context<CoreMessageDecoder>().FireChannelRead(buffer);
            }
            finally
            {
                buffer.Release();
            }
        }
    }
}
