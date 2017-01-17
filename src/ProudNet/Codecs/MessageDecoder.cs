using System.Collections.Generic;
using System.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProudNet.Serialization;
using ProudNet.Serialization.Messages;

namespace ProudNet.Codecs
{
    internal class MessageDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        private readonly MessageFactory _userMessageFactory;

        public MessageDecoder(MessageFactory userMessageFactory)
        {
            _userMessageFactory = userMessageFactory;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            using (var r = new ReadOnlyByteBufferStream(message, false).ToBinaryReader(false))
            {
                var opCode = r.ReadUInt16();
                output.Add(opCode >= 64000
                    ? RmiMessageFactory.Default.GetMessage(opCode, r)
                    : _userMessageFactory.GetMessage(opCode, r));
            }
        }
    }
}
