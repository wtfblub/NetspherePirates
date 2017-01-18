using System.Collections.Generic;
using System.IO;
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
            using (var r = new ReadOnlyByteBufferStream(message, false).ToBinaryReader(false))
            {
                var opCode = r.ReadEnum<ProudCoreOpCode>();
                output.Add(CoreMessageFactory.Default.GetMessage(opCode, r));
            }
        }
    }
}
