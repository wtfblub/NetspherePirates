using System;
using System.Collections.Generic;
using BlubLib;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace ProudNet.DotNetty.Codecs
{
    internal class UdpFrameEncoder : MessageToMessageEncoder<UdpMessage>
    {
        protected override void Encode(IChannelHandlerContext context, UdpMessage message, List<object> output)
        {
            var buffer = context.Allocator.Buffer();
            try
            {
                buffer.WriteShortLE(message.Flag)
                    .WriteShortLE(message.SessionId)
                    .WriteIntLE(0)
                    .WriteIntLE((int)message.Id)
                    .WriteIntLE((int)message.FragId);

                var headerLength = buffer.ReadableBytes;
                buffer.WriteShortLE(Constants.NetMagic)
                    .WriteStruct(message.Content)
                    .SetIntLE(4, buffer.ReadableBytes - headerLength);

                output.Add(new DatagramPacket(buffer, message.EndPoint));
            }
            catch (Exception ex)
            {
                buffer.Release();
                ex.Rethrow();
            }
            finally
            {
                message.Content.Release();
            }
        }
    }
}
