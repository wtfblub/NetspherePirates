using System.Collections.Generic;
using System.IO;
using BlubLib.DotNetty;
using BlubLib.IO;
using BlubLib.Serialization;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.DotNetty.Codecs
{
    internal class CoreMessageEncoder : MessageToMessageEncoder<ICoreMessage>
    {
        private readonly BlubSerializer _serializer;

        public CoreMessageEncoder(BlubSerializer serializer)
        {
            _serializer = serializer;
        }

        protected override void Encode(IChannelHandlerContext context, ICoreMessage message, List<object> output)
        {
            var buffer = context.Allocator.Buffer(sizeof(ProudCoreOpCode));
            Encode(_serializer, message, buffer);
            output.Add(buffer);
        }

        public static void Encode(BlubSerializer serializer, ICoreMessage message, IByteBuffer buffer)
        {
            var opCode = CoreMessageFactory.Default.GetOpCode(message.GetType());
            using (var w = new WriteOnlyByteBufferStream(buffer, false).ToBinaryWriter(false))
            {
                w.WriteEnum(opCode);
                serializer.Serialize(w, (object)message);
            }
        }

        public static byte[] Encode(BlubSerializer serializer, ICoreMessage message)
        {
            var opCode = CoreMessageFactory.Default.GetOpCode(message.GetType());
            using (var w = new MemoryStream().ToBinaryWriter(false))
            {
                w.WriteEnum(opCode);
                serializer.Serialize(w, (object)message);
                return w.ToArray();
            }
        }
    }
}
