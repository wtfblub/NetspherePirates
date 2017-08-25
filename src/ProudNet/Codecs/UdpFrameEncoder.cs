using System;
using System.Collections.Generic;
using BlubLib;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace ProudNet.Codecs
{
    internal class UdpFrameEncoder : MessageToMessageEncoder<UdpMessage>
    {
        protected override void Encode(IChannelHandlerContext context, UdpMessage message, List<object> output)
        {
            var buffer = context.Allocator.Buffer().WithOrder(ByteOrder.LittleEndian);
            try
            {
                buffer.WriteShort(message.Flag)
                    .WriteShort(message.SessionId)
                    .WriteInt(0)
                    .WriteInt((int)message.Id)
                    .WriteInt((int)message.FragId);

                var headerLength = buffer.ReadableBytes;
                buffer.WriteShort(Constants.NetMagic)
                    .WriteStruct(message.Content)
                    .SetInt(4, buffer.ReadableBytes - headerLength);

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
