using System;
using System.Net;
using System.Threading.Tasks;
using BlubLib.Collections.Concurrent;
using BlubLib.Collections.Generic;
using BlubLib.DotNetty.Handlers.MessageHandling;
using ProudNet.Serialization.Messages;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Handlers
{
    internal class ServerHandler : ProudMessageHandler
    {
        [MessageHandler(typeof(ReliablePingMessage))]
        public Task ReliablePing(ProudSession session)
        {
            return session.SendAsync(new ReliablePongMessage());
        }

        [MessageHandler(typeof(P2P_NotifyDirectP2PDisconnectedMessage))]
        public void P2P_NotifyDirectP2PDisconnected(ProudSession session, P2P_NotifyDirectP2PDisconnectedMessage message)
        {
            if (session.P2PGroup == null)
                return;

            session.Logger?.Debug("P2P_NotifyDirectP2PDisconnected {@Message}", message);
            var remotePeer = session.P2PGroup.Members.GetValueOrDefault(session.HostId);
            var stateA = remotePeer?.ConnectionStates.GetValueOrDefault(message.RemotePeerHostId);
            var stateB = stateA?.RemotePeer.ConnectionStates.GetValueOrDefault(session.HostId);
            if (stateA?.HolepunchSuccess == true)
            {
                session.Logger?.Information("P2P to {TargetHostId} disconnected with {Reason}", message.RemotePeerHostId, message.Reason);
                stateA.HolepunchSuccess = false;
                stateA.RemotePeer.SendAsync(
                    new P2P_NotifyDirectP2PDisconnected2Message(session.HostId, message.Reason));
            }

            if (stateB?.HolepunchSuccess == true)
                stateB.HolepunchSuccess = false;
        }

        [MessageHandler(typeof(NotifyUdpToTcpFallbackByClientMessage))]
        public void NotifyUdpToTcpFallbackByClient(ProudServer server, ProudSession session)
        {
            session.Logger?.Information("Fallback to tcp relay by client");
            session.UdpEnabled = false;
            server.SessionsByUdpId.Remove(session.UdpSessionId);
        }

        [MessageHandler(typeof(P2PGroup_MemberJoin_AckMessage))]
        public void P2PGroupMemberJoinAck(ProudSession session, P2PGroup_MemberJoin_AckMessage message)
        {
            session.Logger?.Debug("P2PGroupMemberJoinAck {@Message}", message);
            if (session.P2PGroup == null || session.HostId == message.AddedMemberHostId)
                return;

            var remotePeer = session.P2PGroup?.Members[session.HostId];
            var stateA = remotePeer?.ConnectionStates.GetValueOrDefault(message.AddedMemberHostId);
            if (stateA?.EventId != message.EventId)
                return;

            stateA.IsJoined = true;
            var stateB = stateA.RemotePeer.ConnectionStates.GetValueOrDefault(session.HostId);
            if (stateB?.IsJoined == true)
            {
                // Do not try p2p when the udp relay is not used by one of the clients
                if (!stateA.RemotePeer.Session.UdpEnabled || !stateB.RemotePeer.Session.UdpEnabled)
                    return;

                session.Logger?.Debug("Initialize P2P with {TargetHostId}", stateA.RemotePeer.HostId);
                stateA.RemotePeer.Session.Logger?.Debug("Initialize P2P with {TargetHostId}", session.HostId);
                stateA.LastHolepunch = stateB.LastHolepunch = DateTimeOffset.Now;
                stateA.IsInitialized = stateB.IsInitialized = true;
                remotePeer.SendAsync(new P2PRecycleCompleteMessage(stateA.RemotePeer.HostId));
                stateA.RemotePeer.SendAsync(new P2PRecycleCompleteMessage(session.HostId));
            }
        }

        [MessageHandler(typeof(NotifyP2PHolepunchSuccessMessage))]
        public void NotifyP2PHolepunchSuccess(ProudSession session, NotifyP2PHolepunchSuccessMessage message)
        {
            session.Logger?.Debug("NotifyP2PHolepunchSuccess {@Message}", message);
            var group = session.P2PGroup;
            if (group == null || (session.HostId != message.A && session.HostId != message.B))
                return;

            var remotePeerA = group.Members.GetValueOrDefault(message.A);
            var remotePeerB = group.Members.GetValueOrDefault(message.B);
            if (remotePeerA == null || remotePeerB == null)
                return;

            var stateA = remotePeerA.ConnectionStates.GetValueOrDefault(remotePeerB.HostId);
            var stateB = remotePeerB.ConnectionStates.GetValueOrDefault(remotePeerA.HostId);
            if (stateA == null || stateB == null)
                return;

            if (session.HostId == remotePeerA.HostId)
                stateA.HolepunchSuccess = true;
            else if (session.HostId == remotePeerB.HostId)
                stateB.HolepunchSuccess = true;

            if (stateA.HolepunchSuccess && stateB.HolepunchSuccess)
            {
                var notify = new NotifyDirectP2PEstablishMessage(message.A, message.B, message.ABSendAddr, message.ABRecvAddr,
                    message.BASendAddr, message.BARecvAddr);

                remotePeerA.SendAsync(notify);
                remotePeerB.SendAsync(notify);
            }
        }

        [MessageHandler(typeof(ShutdownTcpMessage))]
        public void ShutdownTcp(ProudSession session)
        {
            session.CloseAsync();
        }

        [MessageHandler(typeof(NotifyLogMessage))]
        public void NotifyLog(ProudServer server, NotifyLogMessage message)
        {
            server.Configuration.Logger?.Debug("NotifyLog {@Message}", message);
        }

        [MessageHandler(typeof(NotifyJitDirectP2PTriggeredMessage))]
        public void NotifyJitDirectP2PTriggered(ProudSession session, NotifyJitDirectP2PTriggeredMessage message)
        {
            session.Logger?.Debug("NotifyJitDirectP2PTriggered {@Message}", message);
            var group = session.P2PGroup;
            if (group == null)
                return;

            var remotePeerA = group.Members.GetValueOrDefault(session.HostId);
            var remotePeerB = group.Members.GetValueOrDefault(message.HostId);
            if (remotePeerA == null || remotePeerB == null)
                return;

            var stateA = remotePeerA.ConnectionStates.GetValueOrDefault(remotePeerB.HostId);
            var stateB = remotePeerB.ConnectionStates.GetValueOrDefault(remotePeerA.HostId);
            if (stateA == null || stateB == null)
                return;

            if (session.HostId == remotePeerA.HostId)
                stateA.JitTriggered = true;
            else if (session.HostId == remotePeerB.HostId)
                stateB.JitTriggered = true;

            if (stateA.JitTriggered && stateB.JitTriggered)
            {
                remotePeerA.SendAsync(new NewDirectP2PConnectionMessage(remotePeerB.HostId));
                remotePeerB.SendAsync(new NewDirectP2PConnectionMessage(remotePeerA.HostId));
            }
        }

        [MessageHandler(typeof(NotifyNatDeviceNameDetectedMessage))]
        public void NotifyNatDeviceNameDetected()
        { }

        [MessageHandler(typeof(C2S_RequestCreateUdpSocketMessage))]
        public void C2S_RequestCreateUdpSocket(ProudServer server, ProudSession session)
        {
            session.Logger?.Debug("C2S_RequestCreateUdpSocket");
            if (session.P2PGroup == null || session.UdpEnabled || !server.UdpSocketManager.IsRunning)
                return;

            var socket = server.UdpSocketManager.NextSocket();
            session.UdpSocket = socket;
            session.HolepunchMagicNumber = Guid.NewGuid();
            session.SendAsync(new S2C_RequestCreateUdpSocketMessage(new IPEndPoint(server.UdpSocketManager.Address, ((IPEndPoint)socket.Channel.LocalAddress).Port)));
        }

        [MessageHandler(typeof(C2S_CreateUdpSocketAckMessage))]
        public void C2S_CreateUdpSocketAck(ProudServer server, ProudSession session, C2S_CreateUdpSocketAckMessage message)
        {
            session.Logger?.Debug("{@Message}", message);
            if (session.P2PGroup == null || session.UdpSocket == null || session.UdpEnabled ||
                !server.UdpSocketManager.IsRunning)
                return;

            session.SendAsync(new RequestStartServerHolepunchMessage(session.HolepunchMagicNumber));
        }

        [MessageHandler(typeof(ReportC2SUdpMessageTrialCountMessage))]
        public void ReportC2SUdpMessageTrialCount()
        { }
    }
}
