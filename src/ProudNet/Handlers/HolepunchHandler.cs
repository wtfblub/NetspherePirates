using System;
using System.Threading.Tasks;
using BlubLib.Collections.Concurrent;
using Microsoft.Extensions.Logging;
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

        public async Task<bool> OnHandle(MessageContext context, ServerHolepunchMessage message)
        {
            var session = context.Session;

            // TODO Use firewall
            session.Logger.LogDebug("ServerHolepunch={@Message}", message);
            if (session.P2PGroup == null || !_udpSocketManager.IsRunning || session.HolepunchMagicNumber != message.MagicNumber)
                return true;

            await session.SendUdpAsync(new ServerHolepunchAckMessage(session.HolepunchMagicNumber, session.UdpEndPoint));
            return true;
        }

        public async Task<bool> OnHandle(MessageContext context, NotifyHolepunchSuccessMessage message)
        {
            var session = context.Session;

            // TODO Use firewall
            session.Logger.LogDebug("NotifyHolepunchSuccess={@Message}", message);
            if (session.P2PGroup == null || !_udpSocketManager.IsRunning || session.HolepunchMagicNumber != message.MagicNumber)
                return true;

            session.LastUdpPing = DateTimeOffset.Now;
            session.UdpEnabled = true;
            session.UdpLocalEndPoint = message.LocalEndPoint;
            await session.SendUdpAsync(new NotifyClientServerUdpMatchedMessage(message.MagicNumber));
            return true;
        }

        public async Task<bool> OnHandle(MessageContext context, PeerUdp_ServerHolepunchMessage message)
        {
            var session = context.Session;

            // TODO Use firewall
            session.Logger.LogDebug("PeerUdp_ServerHolepunch={@Message}", message);
            if (!session.UdpEnabled || !_udpSocketManager.IsRunning)
                return true;

            // TODO Use firewall
            if (!(session.P2PGroup.GetMember(message.HostId) is ProudSession target) || !target.UdpEnabled)
                return true;

            await session.SendUdpAsync(
                new PeerUdp_ServerHolepunchAckMessage(message.MagicNumber, target.UdpEndPoint, target.HostId));
            return true;
        }

        public async Task<bool> OnHandle(MessageContext context, PeerUdp_NotifyHolepunchSuccessMessage message)
        {
            var session = context.Session;

            // TODO Use firewall
            session.Logger.LogDebug("PeerUdp_NotifyHolepunchSuccess={@Message}", message);
            if (!session.UdpEnabled || !_udpSocketManager.IsRunning)
                return true;

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
                    connectionState.EndPoint));

                await connectionState.RemotePeer.SendAsync(new RequestP2PHolepunchMessage(
                    session.HostId,
                    connectionState.LocalEndPoint,
                    connectionStateB.EndPoint));
            }

            return true;
        }

        public async Task<bool> OnHandle(MessageContext context, NotifyP2PHolepunchSuccessMessage message)
        {
            var session = context.Session;

            session.Logger.LogDebug("NotifyP2PHolepunchSuccess {@Message}", message);
            var group = session.P2PGroup;
            if (group == null || (session.HostId != message.A && session.HostId != message.B))
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

        public async Task<bool> OnHandle(MessageContext context, NotifyJitDirectP2PTriggeredMessage message)
        {
            var session = context.Session;

            session.Logger.LogDebug("NotifyJitDirectP2PTriggered {@Message}", message);
            var group = session.P2PGroup;
            if (group == null)
                return true;

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
