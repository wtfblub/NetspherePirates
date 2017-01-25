using System;
using System.Collections.Generic;
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
                buffer.WriteUnsignedShort(message.Flag)
                    .WriteUnsignedShort(message.SessionId)
                    .WriteInt(message.Length)
                    .WriteUnsignedInt(message.Id)
                    .WriteUnsignedInt(message.FragId)
                    .WriteShort(Constants.NetMagic)
                    .WriteStruct(message.Content);
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
