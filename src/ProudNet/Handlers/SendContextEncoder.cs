using System.Collections.Generic;
using System.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProudNet.Codecs;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Handlers
{
    internal class SendContextEncoder : MessageToMessageEncoder<SendContext>
    {
        protected override void Encode(IChannelHandlerContext context, SendContext message, List<object> output)
        {
            var buffer = message.Message as IByteBuffer;
            if (buffer == null)
                throw new ProudException($"{nameof(SendContextEncoder)} can only handle {nameof(IByteBuffer)}");

            try
            {
                var data = buffer.ToArray();
                ICoreMessage coreMessage = new RmiMessage(data);

                if (message.SendOptions.Compress)
                {
                    data = CoreMessageEncoder.Encode(coreMessage);
                    coreMessage = new CompressedMessage(data.Length, data.CompressZLib());
                }

                if (message.SendOptions.Encrypt)
                {
                    data = CoreMessageEncoder.Encode(coreMessage);
                    var session = context.Channel.GetAttribute(ChannelAttributes.Session).Get();
                    using (var src = new MemoryStream(data))
                    using (var dst = new MemoryStream())
                    {
                        session.Crypt.Encrypt(context.Allocator, EncryptMode.Secure, src, dst, true);
                        data = dst.ToArray();
                    }
                    coreMessage = new EncryptedReliableMessage(data, EncryptMode.Secure);
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
