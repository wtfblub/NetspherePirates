using System;
using System.Threading.Tasks;
using BlubLib;
using BlubLib.IO;
using BlubLib.Serialization;
using Logging;
using Netsphere.Network;
using Netsphere.Network.Message.Event;
using Netsphere.Network.Message.P2P;
using ProudNet;

namespace Netsphere.Server.Relay.Handlers
{
    internal class EventHandler : IHandle<PacketMessage>
    {
        private readonly ILogger<EventHandler> _logger;
        private readonly BlubSerializer _serializer;

        public EventHandler(ILogger<EventHandler> logger, BlubSerializer serializer)
        {
            _logger = logger;
            _serializer = serializer;
        }

        [Inline]
        public Task<bool> OnHandle(MessageContext context, PacketMessage message)
        {
            return Task.FromResult(true);
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
                        _logger.Error(ex, "Failed to deserialize P2PMessage");
                        break;
                    }
                }
            }

            return Task.FromResult(true);
        }
    }
}
