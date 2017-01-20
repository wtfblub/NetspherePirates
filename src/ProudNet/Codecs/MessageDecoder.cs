using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProudNet.Serialization;
using ProudNet.Serialization.Messages;
using ReadOnlyByteBufferStream = BlubLib.DotNetty.ReadOnlyByteBufferStream;

namespace ProudNet.Codecs
{
    internal class MessageDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        private readonly MessageFactory[] _userMessageFactories;

        public MessageDecoder(MessageFactory[] userMessageFactories)
        {
            _userMessageFactories = userMessageFactories;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            using (var r = new ReadOnlyByteBufferStream(message, false).ToBinaryReader(false))
            {
                var opCode = r.ReadUInt16();
                var isInternal = opCode >= 64000;
                var factory = isInternal
                    ? RmiMessageFactory.Default
                    : _userMessageFactories.FirstOrDefault(userFactory => userFactory.ContainsOpCode(opCode));

                if (factory == null)
                    throw new ProudException($"No {nameof(MessageFactory)} found for opcode {opCode}");

                output.Add(factory.GetMessage(opCode, r));
            }
        }
    }
}
