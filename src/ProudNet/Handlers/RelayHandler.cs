using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProudNet.Firewall;
using ProudNet.Serialization;
using ProudNet.Serialization.Messages;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Handlers
{
    internal class RelayHandler
        : IHandle<ReliableRelay1Message>,
          IHandle<UnreliableRelay1Message>,
          IHandle<C2S_RequestCreateUdpSocketMessage>,
          IHandle<C2S_CreateUdpSocketAckMessage>
    {
        private readonly IInternalSessionManager<Guid> _magicNumberSessionManager;
        private readonly IInternalSessionManager<uint> _sessionManager;
        private readonly UdpSocketManager _udpSocketManager;

        public RelayHandler(ISessionManagerFactory sessionManagerFactory, UdpSocketManager udpSocketManager)
        {
            _magicNumberSessionManager = sessionManagerFactory.GetSessionManager<Guid>(SessionManagerType.MagicNumber);
            _sessionManager = sessionManagerFactory.GetSessionManager<uint>(SessionManagerType.HostId);
            _udpSocketManager = udpSocketManager;
        }

        [Firewall(typeof(MustBeInP2PGroup))]
        public async Task<bool> OnHandle(MessageContext context, ReliableRelay1Message message)
        {
            var session = context.Session;

            foreach (var destination in message.Destination.Where(x => x.HostId != session.HostId))
            {
                // TODO Hack for race condition
                if (session.P2PGroup?.GetMember(destination.HostId) == null)
                    return true;

                var target = _sessionManager.GetSession(destination.HostId);
                var _ = target?.SendAsync(new ReliableRelay2Message(
                    new RelayDestinationDto(session.HostId, destination.FrameNumber), message.Data));
            }

            return true;
        }

        [Firewall(typeof(MustBeInP2PGroup))]
        public async Task<bool> OnHandle(MessageContext context, UnreliableRelay1Message message)
        {
            var session = context.Session;

            foreach (var destination in message.Destination.Where(id => id != session.HostId))
            {
                // TODO Hack for race condition
                if (session.P2PGroup?.GetMember(destination) == null)
                    return true;

                var target = _sessionManager.GetSession(destination);
                var _ = target?.SendUdpIfAvailableAsync(new UnreliableRelay2Message(session.HostId, message.Data));
            }

            return true;
        }

        [Firewall(typeof(MustBeInP2PGroup))]
        [Firewall(typeof(MustBeUdpRelay), Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, C2S_RequestCreateUdpSocketMessage message)
        {
            var session = context.Session;

            session.Logger.LogDebug("C2S_RequestCreateUdpSocketMessage {@Message}", message);
            if (!_udpSocketManager.IsRunning)
                return true;

            _magicNumberSessionManager.RemoveSession(session.HolepunchMagicNumber);

            var socket = _udpSocketManager.NextSocket();
            session.UdpSocket = socket;
            session.HolepunchMagicNumber = Guid.NewGuid();
            _magicNumberSessionManager.AddSession(session.HolepunchMagicNumber, session);
            await session.SendAsync(new S2C_RequestCreateUdpSocketMessage(
                new IPEndPoint(_udpSocketManager.Address, ((IPEndPoint)socket.Channel.LocalAddress).Port)));
            return true;
        }

        [Firewall(typeof(MustBeInP2PGroup))]
        [Firewall(typeof(MustBeUdpRelay), Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, C2S_CreateUdpSocketAckMessage message)
        {
            var session = context.Session;

            session.Logger.LogDebug("C2S_CreateUdpSocketAckMessage {@Message}", message);
            if (!_udpSocketManager.IsRunning)
                return true;

            await session.SendAsync(new RequestStartServerHolepunchMessage(session.HolepunchMagicNumber));
            return true;
        }
    }
}
