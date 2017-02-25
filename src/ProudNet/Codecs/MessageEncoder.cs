using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlubLib.DotNetty;
using BlubLib.Serialization;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProudNet.Serialization;
using ProudNet.Serialization.Messages;

namespace ProudNet.Codecs
{
    internal class MessageEncoder : MessageToMessageEncoder<SendContext>
    {
        private readonly MessageFactory[] _userMessageFactories;

        public MessageEncoder(MessageFactory[] userMessageFactories)
        {
            _userMessageFactories = userMessageFactories;
        }

        protected override void Encode(IChannelHandlerContext context, SendContext message, List<object> output)
        {
            var type = message.Message.GetType();
            var isInternal = RmiMessageFactory.Default.ContainsType(type);
            var factory = isInternal
                ? RmiMessageFactory.Default
                : _userMessageFactories.FirstOrDefault(userFactory => userFactory.ContainsType(type));

            if (factory == null)
                throw new ProudException($"No {nameof(MessageFactory)} found for message {type.FullName}");

            var opCode = factory.GetOpCode(type);
            var buffer = context.Allocator.Buffer(2);
            using (var w = new WriteOnlyByteBufferStream(buffer, false).ToBinaryWriter(false))
            {
                w.Write(opCode);
                Serializer.Serialize(w, message.Message);
            }
            message.Message = buffer;
            output.Add(message);
        }
    }
}
