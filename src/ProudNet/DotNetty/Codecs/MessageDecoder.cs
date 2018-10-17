using System.Collections.Generic;
using System.Linq;
using BlubLib.IO;
using BlubLib.Serialization;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProudNet.Serialization;
using ProudNet.Serialization.Messages;
using ReadOnlyByteBufferStream = BlubLib.DotNetty.ReadOnlyByteBufferStream;

namespace ProudNet.DotNetty.Codecs
{
    internal class MessageDecoder : MessageToMessageDecoder<MessageContext>
    {
        private readonly BlubSerializer _serializer;
        private readonly MessageFactory[] _userMessageFactories;

        public MessageDecoder(BlubSerializer serializer, MessageFactory[] userMessageFactories)
        {
            _serializer = serializer;
            _userMessageFactories = userMessageFactories;
        }

        protected override void Decode(IChannelHandlerContext context, MessageContext message, List<object> output)
        {
            var buffer = message.Message as IByteBuffer;
            try
            {
                // Drop core messages
                if (buffer == null)
                    return;

                using (var r = new ReadOnlyByteBufferStream(buffer, false).ToBinaryReader(false))
                {
                    var opCode = r.ReadUInt16();
                    var isInternal = opCode >= 64000;
                    var factory = isInternal
                        ? RmiMessageFactory.Default
                        : _userMessageFactories.FirstOrDefault(userFactory => userFactory.ContainsOpCode(opCode));

                    if (factory == null)
                    {
#if DEBUG
                        throw new ProudBadOpCodeException(opCode, buffer.GetIoBuffer());
#else
                        throw new ProudException($"No {nameof(MessageFactory)} found for opcode {opCode}");
#endif
                    }

                    message.Message = factory.GetMessage(_serializer, opCode, r);
                    output.Add(message);
                }
            }
            finally
            {
                buffer?.Release();
            }
        }
    }
}
