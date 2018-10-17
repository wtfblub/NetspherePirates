using System.Collections.Generic;
using BlubLib.IO;
using BlubLib.Serialization;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using ProudNet.Serialization.Messages.Core;
using ReadOnlyByteBufferStream = BlubLib.DotNetty.ReadOnlyByteBufferStream;

namespace ProudNet.DotNetty.Codecs
{
    internal class CoreMessageDecoder : MessageToMessageDecoderEx<MessageContext>
    {
        private readonly BlubSerializer _serializer;

        public CoreMessageDecoder(BlubSerializer serializer)
        {
            _serializer = serializer;
        }

        protected override void Decode(IChannelHandlerContext context, MessageContext message, List<object> output)
        {
            var buffer = message.Message as IByteBuffer;
            try
            {
                if (buffer == null)
                    throw new ProudException($"{nameof(CoreMessageDecoder)} can only handle {nameof(IByteBuffer)}");

                message.Message = Decode(_serializer, buffer);
                output.Add(message);
            }
            finally
            {
                buffer?.Release();
            }
        }

        public static ICoreMessage Decode(BlubSerializer serializer, IByteBuffer buffer)
        {
            using (var r = new ReadOnlyByteBufferStream(buffer, false).ToBinaryReader(false))
            {
                var opCode = r.ReadEnum<ProudCoreOpCode>();
                return CoreMessageFactory.Default.GetMessage(serializer, opCode, r);
            }
        }
    }
}
