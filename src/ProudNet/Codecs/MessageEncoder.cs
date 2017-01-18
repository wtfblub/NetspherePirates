using System.Collections.Generic;
using System.IO;
using BlubLib.DotNetty;
using BlubLib.Serialization;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProudNet.Serialization;
using ProudNet.Serialization.Messages;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Codecs
{
    internal class MessageEncoder : MessageToMessageEncoder<object>
    {
        private readonly MessageFactory _userMessageFactory;

        public MessageEncoder(MessageFactory userMessageFactory)
        {
            _userMessageFactory = userMessageFactory;
        }

        protected override void Encode(IChannelHandlerContext context, object message, List<object> output)
        {
            var type = message.GetType();
            var isInternal = RmiMessageFactory.Default.ContainsType(type);
            var factory = isInternal ? RmiMessageFactory.Default : _userMessageFactory;

            // TODO Encryption/Compression etc

            var opCode = factory.GetOpCode(type);
            var buffer = context.Allocator.Buffer(2);
            using (var w = new WriteOnlyByteBufferStream(buffer, true).ToBinaryWriter(false))
            {
                w.Write(opCode);
                Serializer.Serialize(w, message);
                output.Add(new RmiMessage { Data = buffer.ToArray() });
            }
        }
    }
}
