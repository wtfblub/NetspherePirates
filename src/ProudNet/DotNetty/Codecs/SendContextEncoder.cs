using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlubLib.Serialization;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.DotNetty.Codecs
{
    internal class SendContextEncoder : MessageToMessageEncoder<SendContext>
    {
        private readonly BlubSerializer _serializer;

        public SendContextEncoder(BlubSerializer serializer)
        {
            _serializer = serializer;
        }

        protected override void Encode(IChannelHandlerContext context, SendContext message, List<object> output)
        {
            if (!(message.Message is IByteBuffer buffer))
                throw new ProudException($"{nameof(SendContextEncoder)} can only handle {nameof(IByteBuffer)}");

            try
            {
                var data = buffer.GetIoBuffer().ToArray();
                ICoreMessage coreMessage = new RmiMessage(data);

                if (message.SendOptions.RelayFrom > 0)
                {
                    data = CoreMessageEncoder.Encode(_serializer, coreMessage);
                    coreMessage = new UnreliableRelay2Message(message.SendOptions.RelayFrom, data);
                }
                else
                {
                    if (message.SendOptions.Compress)
                    {
                        data = CoreMessageEncoder.Encode(_serializer, coreMessage);
                        coreMessage = new CompressedMessage(data.Length, data.CompressZLib());
                    }

                    if (message.SendOptions.Encrypt)
                    {
                        data = CoreMessageEncoder.Encode(_serializer, coreMessage);
                        var session = context.Channel.GetAttribute(ChannelAttributes.Session).Get();
                        using (var src = new MemoryStream(data))
                        using (var dst = new MemoryStream())
                        {
                            session.Crypt.Encrypt(context.Allocator, EncryptMode.Secure, src, dst, true);
                            data = dst.ToArray();
                        }

                        coreMessage = new EncryptedReliableMessage(data, EncryptMode.Secure);
                    }
                }

                output.Add(coreMessage);
            }
            finally
            {
                buffer.Release();
            }
        }
    }
}
