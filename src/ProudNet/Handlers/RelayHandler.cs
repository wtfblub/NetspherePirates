using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BlubLib.Serialization;
using DotNetty.Buffers;
using ProudNet.DotNetty.Codecs;
using ProudNet.Firewall;
using ProudNet.Serialization;
using ProudNet.Serialization.Messages;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Handlers
{
    internal class RelayHandler
        : IHandle<ReliableUdp_FrameMessage>,
          IHandle<ReliableRelay1Message>,
          IHandle<UnreliableRelay1Message>,
          IHandle<UnreliableRelay1_RelayDestListCompressedMessage>,
          IHandle<C2S_RequestCreateUdpSocketMessage>,
          IHandle<C2S_CreateUdpSocketAckMessage>
    {
        private readonly IInternalSessionManager<Guid> _magicNumberSessionManager;
        private readonly UdpSocketManager _udpSocketManager;
        private readonly BlubSerializer _serializer;

        public RelayHandler(ISessionManagerFactory sessionManagerFactory, UdpSocketManager udpSocketManager,
            BlubSerializer serializer)
        {
            _magicNumberSessionManager = sessionManagerFactory.GetSessionManager<Guid>(SessionManagerType.MagicNumber);
            _udpSocketManager = udpSocketManager;
            _serializer = serializer;
        }

        [Firewall(typeof(MustBeInP2PGroup))]
        public Task<bool> OnHandle(MessageContext context, ReliableUdp_FrameMessage message)
        {
            var decodedMessage = CoreMessageDecoder.Decode(_serializer, Unpooled.WrappedBuffer(message.Data));
            context.ChannelHandlerContext.Handler.ChannelRead(context.ChannelHandlerContext, new MessageContext
            {
                ChannelHandlerContext = context.ChannelHandlerContext,
                Session = context.Session,
                Message = decodedMessage,
                UdpEndPoint = context.UdpEndPoint
            });
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInP2PGroup))]
        public Task<bool> OnHandle(MessageContext context, ReliableRelay1Message message)
        {
            var session = context.Session;

            foreach (var destination in message.Destination.Where(x => x.HostId != session.HostId))
            {
                if (destination.HostId == Constants.HostIdServerHack)
                {
                    var decodedMessage = CoreMessageDecoder.Decode(_serializer, Unpooled.WrappedBuffer(message.Data));
                    context.ChannelHandlerContext.Handler.ChannelRead(context.ChannelHandlerContext, new MessageContext
                    {
                        ChannelHandlerContext = context.ChannelHandlerContext,
                        Session = context.Session,
                        Message = decodedMessage,
                        UdpEndPoint = context.UdpEndPoint
                    });
                    continue;
                }

                var target = session.P2PGroup?.GetMemberInternal(destination.HostId);
                target?.Send(new ReliableRelay2Message(new RelayDestinationDto(session.HostId, destination.FrameNumber),
                    message.Data));
            }

            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInP2PGroup))]
        public Task<bool> OnHandle(MessageContext context, UnreliableRelay1Message message)
        {
            var session = context.Session;

            foreach (var destination in message.Destination.Where(id => id != session.HostId))
            {
                if (destination == Constants.HostIdServerHack)
                {
                    var decodedMessage = CoreMessageDecoder.Decode(_serializer, Unpooled.WrappedBuffer(message.Data));
                    context.ChannelHandlerContext.Handler.ChannelRead(context.ChannelHandlerContext, new MessageContext
                    {
                        ChannelHandlerContext = context.ChannelHandlerContext,
                        Session = context.Session,
                        Message = decodedMessage,
                        UdpEndPoint = context.UdpEndPoint
                    });
                    continue;
                }

                var target = session.P2PGroup?.GetMemberInternal(destination);
                target?.Send(new UnreliableRelay2Message(session.HostId, message.Data), true);
            }

            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInP2PGroup))]
        public Task<bool> OnHandle(MessageContext context, UnreliableRelay1_RelayDestListCompressedMessage message)
        {
            var session = context.Session;

            session.Logger.Debug("UnreliableRelay1_RelayDestListCompressedMessage: {@Message}", message);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInP2PGroup))]
        [Firewall(typeof(MustBeUdpRelay), Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, C2S_RequestCreateUdpSocketMessage message)
        {
            var session = context.Session;

            session.Logger.Debug("C2S_RequestCreateUdpSocketMessage {@Message}", message);
            if (!_udpSocketManager.IsRunning)
                return true;

            _magicNumberSessionManager.RemoveSession(session.HolepunchMagicNumber);

            var socket = _udpSocketManager.NextSocket();
            session.UdpSocket = socket;
            session.HolepunchMagicNumber = Guid.NewGuid();
            _magicNumberSessionManager.AddSession(session.HolepunchMagicNumber, session);
            session.Send(new S2C_RequestCreateUdpSocketMessage(
                new IPEndPoint(_udpSocketManager.Address, ((IPEndPoint)socket.Channel.LocalAddress).Port))
            );
            return true;
        }

        [Firewall(typeof(MustBeInP2PGroup))]
        [Firewall(typeof(MustBeUdpRelay), Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, C2S_CreateUdpSocketAckMessage message)
        {
            var session = context.Session;

            session.Logger.Debug("C2S_CreateUdpSocketAckMessage {@Message}", message);
            if (!_udpSocketManager.IsRunning)
                return true;

            session.Send(new RequestStartServerHolepunchMessage(session.HolepunchMagicNumber));
            return true;
        }
    }
}
