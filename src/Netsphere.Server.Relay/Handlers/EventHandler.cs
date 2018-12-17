using System;
using System.Threading.Tasks;
using BlubLib;
using BlubLib.IO;
using BlubLib.Serialization;
using Netsphere.Network;
using Netsphere.Network.Message.Event;
using Netsphere.Network.Message.P2P;
using ProudNet;

namespace Netsphere.Server.Relay.Handlers
{
    internal class EventHandler : IHandle<PacketMessage>
    {
        private readonly BlubSerializer _serializer;

        public EventHandler(BlubSerializer serializer)
        {
            _serializer = serializer;
        }

        public Task<bool> OnHandle(MessageContext context, PacketMessage message)
        {
            var data = message.IsCompressed ? message.Data.DecompressLZO(2048) : message.Data;

            using (var r = data.ToBinaryReader())
            {
                while (r.BaseStream.Position != r.BaseStream.Length)
                {
                    try
                    {
                        var opCode = r.ReadEnum<P2POpCode>();
                        var m = new P2PMessageFactory().GetMessage(_serializer, opCode, r);
                        context.ChannelHandlerContext.Handler.ChannelRead(context.ChannelHandlerContext, new MessageContext
                        {
                            ChannelHandlerContext = context.ChannelHandlerContext,
                            Session = context.Session,
                            Message = m
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        break;
                    }
                }
            }

            return Task.FromResult(true);
        }
    }
}
