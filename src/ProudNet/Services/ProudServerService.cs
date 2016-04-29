using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using BlubLib.Network;
using BlubLib.Network.Pipes;
using NLog;
using NLog.Fluent;
using ProudNet.Message;
using ProudNet.Message.Core;

namespace ProudNet.Services
{
    internal class ProudServerService : MessageHandler
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ProudServerPipe _filter;

        public ProudServerService(ProudServerPipe filter)
        {
            _filter = filter;
        }

        [MessageHandler(typeof(ReliablePingMessage))]
        public void ReliablePing(ISession session)
        {
            session.Send(new ReliablePongMessage());
        }

        [MessageHandler(typeof(P2PGroup_MemberJoin_AckMessage))]
        public void P2PGroupMemberJoinAck(ProudSession session, P2PGroup_MemberJoin_AckMessage message)
        {
            if (session.P2PGroup == null || session.HostId == message.AddedMemberHostId)
                return;

            var remotePeer = (ServerRemotePeer)session.P2PGroup.Members[session.HostId];
            var connectionState = remotePeer.ConnectionStates.GetValueOrDefault(message.AddedMemberHostId);

            if (connectionState.EventId != message.EventId)
                return;

            connectionState.IsJoined = true;
            var connectionStateB = connectionState.RemotePeer.ConnectionStates[session.HostId];
            if (connectionStateB.IsJoined)
            {
                remotePeer.Send(new P2PRecycleCompleteMessage(connectionState.RemotePeer.HostId));
                connectionState.RemotePeer.Send(new P2PRecycleCompleteMessage(session.HostId));
            }
        }

        [MessageHandler(typeof(NotifyP2PHolepunchSuccessMessage))]
        public void NotifyP2PHolepunchSuccess(ProudSession session, NotifyP2PHolepunchSuccessMessage message)
        {
            var group = session.P2PGroup;
            if (group == null || (session.HostId != message.A && session.HostId != message.B))
                return;

            var remotePeerA = (ServerRemotePeer)group.Members.GetValueOrDefault(message.A);
            var remotePeerB = (ServerRemotePeer)group.Members.GetValueOrDefault(message.B);

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

                remotePeerA.Send(notify);
                remotePeerB.Send(notify);
            }
        }

        [MessageHandler(typeof(ShutdownTcpMessage))]
        public void ShutdownTcp(ISession session)
        {
            session.Close();
        }

        [MessageHandler(typeof(NotifyLogMessage))]
        public void NotifyLog(NotifyLogMessage message)
        {
            Logger.Debug()
                .Message("{0} - {1}", message.TraceId, message.Message)
                .Write();
        }

        [MessageHandler(typeof(NotifyJitDirectP2PTriggeredMessage))]
        public void NotifyJitDirectP2PTriggered(ProudSession session, NotifyJitDirectP2PTriggeredMessage message)
        {
            var group = session.P2PGroup;

            if (group == null)
                return;

            var remotePeerA = (ServerRemotePeer)group.Members.GetValueOrDefault(session.HostId);
            var remotePeerB = (ServerRemotePeer)group.Members.GetValueOrDefault(message.HostId);

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
                remotePeerA.Send(new NewDirectP2PConnectionMessage(remotePeerB.HostId));
                remotePeerB.Send(new NewDirectP2PConnectionMessage(remotePeerA.HostId));
            }
        }

        [MessageHandler(typeof(NotifyNatDeviceNameDetectedMessage))]
        public void NotifyNatDeviceNameDetected()
        { }

        [MessageHandler(typeof(C2S_RequestCreateUdpSocketMessage))]
        public void C2S_RequestCreateUdpSocket(ProudSession session)
        {
            if (session.P2PGroup == null || _filter.Config.UdpListener == null)
                return;

            Logger.Debug()
                .Message("Client:{0} - Requesting UdpSocket", session.HostId)
                .Write();

            var endPoint = new IPEndPoint(_filter.Config.UdpAddress, _filter.Config.UdpListener.Port);
            session.Send(new S2C_RequestCreateUdpSocketMessage(endPoint));
        }

        [MessageHandler(typeof(C2S_CreateUdpSocketAckMessage))]
        public void C2S_CreateUdpSocketAck(ProudSession session, C2S_CreateUdpSocketAckMessage message)
        {
            if (session.P2PGroup == null || _filter.Config.UdpListener == null)
                return;

            Logger.Debug()
                .Message("Client:{0} - Starting server holepunch", session.HostId)
                .Write();

            session.Send(new RequestStartServerHolepunchMessage(session.Guid));
        }

        [MessageHandler(typeof(ReportC2SUdpMessageTrialCountMessage))]
        public void ReportC2SUdpMessageTrialCount()
        { }
    }
}
