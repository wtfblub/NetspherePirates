using System.Collections.Generic;
using System.IO;
using BlubLib.DotNetty;
using BlubLib.Serialization;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Codecs
{
    internal class CoreMessageEncoder : MessageToMessageEncoder<ICoreMessage>
    {
        protected override void Encode(IChannelHandlerContext context, ICoreMessage message, List<object> output)
        {
            var opCode = CoreMessageFactory.Default.GetOpCode(message.GetType());
            var buffer = context.Allocator.Buffer(sizeof(ProudCoreOpCode));
            using (var w = new WriteOnlyByteBufferStream(buffer, false).ToBinaryWriter(false))
            {
                w.WriteEnum(opCode);
                Serializer.Serialize(w, (object)message);
            }
            output.Add(buffer);
        }
    }
}
