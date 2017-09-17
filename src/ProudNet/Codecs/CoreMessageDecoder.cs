using System.Collections.Generic;
using BlubLib.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProudNet.Serialization.Messages.Core;
using ReadOnlyByteBufferStream = BlubLib.DotNetty.ReadOnlyByteBufferStream;

namespace ProudNet.Codecs
{
    internal class CoreMessageDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            output.Add(Decode(message));
        }

        public static ICoreMessage Decode(IByteBuffer buffer)
        {
            using (var r = new ReadOnlyByteBufferStream(buffer, false).ToBinaryReader(false))
            {
                var opCode = r.ReadEnum<ProudCoreOpCode>();
                return CoreMessageFactory.Default.GetMessage(opCode, r);
            }
        }
    }
}
