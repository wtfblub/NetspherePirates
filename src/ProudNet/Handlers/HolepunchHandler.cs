using System;
using System.Threading.Tasks;
using BlubLib.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using ProudNet.Firewall;
using ProudNet.Serialization.Messages;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Handlers
{
    internal class HolepunchHandler
        : IHandle<ServerHolepunchMessage>,
          IHandle<NotifyHolepunchSuccessMessage>,
          IHandle<PeerUdp_ServerHolepunchMessage>,
          IHandle<PeerUdp_NotifyHolepunchSuccessMessage>,
          IHandle<NotifyP2PHolepunchSuccessMessage>,
          IHandle<NotifyJitDirectP2PTriggeredMessage>
    {
        private readonly UdpSocketManager _udpSocketManager;

        public HolepunchHandler(UdpSocketManager udpSocketManager)
        {
            _udpSocketManager = udpSocketManager;
        }

        [Firewall(typeof(MustBeInP2PGroup))]
        public async Task<bool> OnHandle(MessageContext context, ServerHolepunchMessage message)
        {
            var session = context.Session;

            session.Logger.LogDebug("ServerHolepunch={@Message}", message);
            if (!_udpSocketManager.IsRunning || session.HolepunchMagicNumber != message.MagicNumber)
                return true;

            await session.SendUdpAsync(new ServerHolepunchAckMessage(session.HolepunchMagicNumber, session.UdpEndPoint));
            return true;
        }

        [Firewall(typeof(MustBeInP2PGroup))]
        public async Task<bool> OnHandle(MessageContext context, NotifyHolepunchSuccessMessage message)
        {
            var session = context.Session;

            session.Logger.LogDebug("NotifyHolepunchSuccess={@Message}", message);
            if (!_udpSocketManager.IsRunning || session.HolepunchMagicNumber != message.MagicNumber)
                return true;

            session.LastUdpPing = DateTimeOffset.Now;
            session.UdpEnabled = true;
            session.UdpLocalEndPoint = message.LocalEndPoint;
            await session.SendUdpAsync(new NotifyClientServerUdpMatchedMessage(message.MagicNumber));
            return true;
        }

        [Firewall(typeof(MustBeInP2PGroup))]
        [Firewall(typeof(MustBeUdpRelay))]
        public async Task<bool> OnHandle(MessageContext context, PeerUdp_ServerHolepunchMessage message)
        {
            var session = context.Session;

            session.Logger.LogDebug("PeerUdp_ServerHolepunch={@Message}", message);
            if (!(session.P2PGroup.GetMember(message.HostId) is ProudSession target) || !target.UdpEnabled)
                return true;

            await session.SendUdpAsync(
                new PeerUdp_ServerHolepunchAckMessage(message.MagicNumber, target.UdpEndPoint, target.HostId));
            return true;
        }

        [Firewall(typeof(MustBeInP2PGroup))]
        [Firewall(typeof(MustBeUdpRelay))]
        public async Task<bool> OnHandle(MessageContext context, PeerUdp_NotifyHolepunchSuccessMessage message)
        {
            var session = context.Session;

            session.Logger.LogDebug("PeerUdp_NotifyHolepunchSuccess={@Message}", message);
            var remotePeer = session.P2PGroup.GetMemberInternal(session.HostId);
            var connectionState = remotePeer.ConnectionStates.GetValueOrDefault(message.HostId);

            connectionState.PeerUdpHolepunchSuccess = true;
            connectionState.LocalEndPoint = message.LocalEndPoint;
            connectionState.EndPoint = message.EndPoint;
            var connectionStateB = connectionState.RemotePeer.ConnectionStates[session.HostId];
            if (connectionStateB.PeerUdpHolepunchSuccess)
            {
                await remotePeer.SendAsync(new RequestP2PHolepunchMessage(
                    message.HostId,
                    connectionStateB.LocalEndPoint,
                    connectionStateB.EndPoint));

                await connectionState.RemotePeer.SendAsync(new RequestP2PHolepunchMessage(
                    session.HostId,
                    connectionState.LocalEndPoint,
                    connectionState.EndPoint));
            }

            return true;
        }

        [Firewall(typeof(MustBeInP2PGroup))]
        [Firewall(typeof(MustBeUdpRelay))]
        public async Task<bool> OnHandle(MessageContext context, NotifyP2PHolepunchSuccessMessage message)
        {
            var session = context.Session;

            session.Logger.LogDebug("NotifyP2PHolepunchSuccess {@Message}", message);
            var group = session.P2PGroup;
            if (session.HostId != message.A && session.HostId != message.B)
                return true;

            var remotePeerA = group.GetMemberInternal(message.A);
            var remotePeerB = group.GetMemberInternal(message.B);
            if (remotePeerA == null || remotePeerB == null)
                return true;

            var stateA = remotePeerA.ConnectionStates.GetValueOrDefault(remotePeerB.HostId);
            var stateB = remotePeerB.ConnectionStates.GetValueOrDefault(remotePeerA.HostId);
            if (stateA == null || stateB == null)
                return true;

            if (session.HostId == remotePeerA.HostId)
                stateA.HolepunchSuccess = true;
            else if (session.HostId == remotePeerB.HostId)
                stateB.HolepunchSuccess = true;

            if (stateA.HolepunchSuccess && stateB.HolepunchSuccess)
            {
                var notify = new NotifyDirectP2PEstablishMessage(message.A, message.B, message.ABSendAddr,
                    message.ABRecvAddr,
                    message.BASendAddr, message.BARecvAddr);

                await remotePeerA.SendAsync(notify);
                await remotePeerB.SendAsync(notify);
            }

            return true;
        }

        [Firewall(typeof(MustBeInP2PGroup))]
        [Firewall(typeof(MustBeUdpRelay))]
        public async Task<bool> OnHandle(MessageContext context, NotifyJitDirectP2PTriggeredMessage message)
        {
            var session = context.Session;

            session.Logger.LogDebug("NotifyJitDirectP2PTriggered {@Message}", message);
            var group = session.P2PGroup;
            var remotePeerA = group.GetMemberInternal(session.HostId);
            var remotePeerB = group.GetMemberInternal(message.HostId);

            if (remotePeerA == null || remotePeerB == null)
                return true;

            var stateA = remotePeerA.ConnectionStates.GetValueOrDefault(remotePeerB.HostId);
            var stateB = remotePeerB.ConnectionStates.GetValueOrDefault(remotePeerA.HostId);
            if (stateA == null || stateB == null)
                return true;

            if (session.HostId == remotePeerA.HostId)
                stateA.JitTriggered = true;
            else if (session.HostId == remotePeerB.HostId)
                stateB.JitTriggered = true;

            if (stateA.JitTriggered && stateB.JitTriggered)
            {
                await remotePeerA.SendAsync(new NewDirectP2PConnectionMessage(remotePeerB.HostId));
                await remotePeerB.SendAsync(new NewDirectP2PConnectionMessage(remotePeerA.HostId));
            }

            return true;
        }
    }
}
