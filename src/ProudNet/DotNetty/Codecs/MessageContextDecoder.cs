using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace ProudNet.DotNetty.Codecs
{
    public class MessageContextDecoder : MessageToMessageDecoderEx<IByteBuffer>
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            output.Add(new MessageContext
            {
                Session = context.GetAttribute(ChannelAttributes.Session).Get(),
                Message = message.Retain()
            });
        }
    }
}
