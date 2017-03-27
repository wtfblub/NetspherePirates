using System.Collections.Generic;
using System.IO;
using BlubLib.DotNetty;
using BlubLib.Serialization;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Codecs
{
    internal class CoreMessageEncoder : MessageToMessageEncoder<ICoreMessage>
    {
        protected override void Encode(IChannelHandlerContext context, ICoreMessage message, List<object> output)
        {
            var buffer = context.Allocator.Buffer(sizeof(ProudCoreOpCode));
            Encode(message, buffer);
            output.Add(buffer);
        }

        public static void Encode(ICoreMessage message, IByteBuffer buffer)
        {
            var opCode = CoreMessageFactory.Default.GetOpCode(message.GetType());
            using (var w = new WriteOnlyByteBufferStream(buffer, false).ToBinaryWriter(false))
            {
                w.WriteEnum(opCode);
                Serializer.Serialize(w, (object)message);
            }
        }

        public static byte[] Encode(ICoreMessage message)
        {
            var opCode = CoreMessageFactory.Default.GetOpCode(message.GetType());
            using (var w = new MemoryStream().ToBinaryWriter(false))
            {
                w.WriteEnum(opCode);
                Serializer.Serialize(w, (object)message);
                return w.ToArray();
            }
        }
    }
}
