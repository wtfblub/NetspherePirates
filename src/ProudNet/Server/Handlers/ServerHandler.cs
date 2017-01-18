using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using ProudNet.Handlers;
using ProudNet.Serialization.Messages;

namespace ProudNet.Server.Handlers
{
    internal class ServerHandler : ProudMessageHandler
    {
        [MessageHandler(typeof(ReliablePingMessage))]
        public Task ReliablePing(ProudSession session)
        {
            return session.SendAsync(new ReliablePongMessage());
        }

        [MessageHandler(typeof(P2PGroup_MemberJoin_AckMessage))]
        public async Task P2PGroupMemberJoinAck(ProudSession session, P2PGroup_MemberJoin_AckMessage message)
        {
            if (session.P2PGroup == null || session.HostId == message.AddedMemberHostId)
                return;

            var remotePeer = (RemotePeer)session.P2PGroup.Members[session.HostId];
            var connectionState = remotePeer.ConnectionStates.GetValueOrDefault(message.AddedMemberHostId);

            if (connectionState.EventId != message.EventId)
                return;

            connectionState.IsJoined = true;
            var connectionStateB = ((RemotePeer)connectionState.RemotePeer).ConnectionStates[session.HostId];
            if (connectionStateB.IsJoined)
            {
                await remotePeer.SendAsync(new P2PRecycleCompleteMessage(connectionState.RemotePeer.HostId));
                await ((RemotePeer)connectionState.RemotePeer).SendAsync(new P2PRecycleCompleteMessage(session.HostId));
            }
        }

        [MessageHandler(typeof(NotifyP2PHolepunchSuccessMessage))]
        public async Task NotifyP2PHolepunchSuccess(ProudSession session, NotifyP2PHolepunchSuccessMessage message)
        {
            var group = session.P2PGroup;
            if (group == null || (session.HostId != message.A && session.HostId != message.B))
                return;

            var remotePeerA = (RemotePeer)group.Members.GetValueOrDefault(message.A);
            var remotePeerB = (RemotePeer)group.Members.GetValueOrDefault(message.B);

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

                await remotePeerA.SendAsync(notify);
                await remotePeerB.SendAsync(notify);
            }
        }

        [MessageHandler(typeof(ShutdownTcpMessage))]
        public Task ShutdownTcp(ProudSession session)
        {
            return session.CloseAsync();
        }

        [MessageHandler(typeof(NotifyLogMessage))]
        public void NotifyLog(NotifyLogMessage message)
        {
            //Logger<>.Debug($"{message.TraceId} - {message.Message}");
        }

        [MessageHandler(typeof(NotifyJitDirectP2PTriggeredMessage))]
        public async Task NotifyJitDirectP2PTriggered(ProudSession session, NotifyJitDirectP2PTriggeredMessage message)
        {
            var group = session.P2PGroup;

            if (group == null)
                return;

            var remotePeerA = (RemotePeer)group.Members.GetValueOrDefault(session.HostId);
            var remotePeerB = (RemotePeer)group.Members.GetValueOrDefault(message.HostId);

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
                await remotePeerA.SendAsync(new NewDirectP2PConnectionMessage(remotePeerB.HostId));
                await remotePeerB.SendAsync(new NewDirectP2PConnectionMessage(remotePeerA.HostId));
            }
        }

        [MessageHandler(typeof(NotifyNatDeviceNameDetectedMessage))]
        public void NotifyNatDeviceNameDetected()
        { }

        //[MessageHandler(typeof(C2S_RequestCreateUdpSocketMessage))]
        //public void C2S_RequestCreateUdpSocket(ProudSession session)
        //{
        //    if (session.P2PGroup == null || _filter.Config.UdpListener == null)
        //        return;

        //    Logger<>.Debug($"Client:{session.HostId} - Requesting UdpSocket");
        //    var endPoint = new IPEndPoint(_filter.Config.UdpAddress, _filter.Config.UdpListener.Port);
        //    session.Send(new S2C_RequestCreateUdpSocketMessage(endPoint));
        //}

        //[MessageHandler(typeof(C2S_CreateUdpSocketAckMessage))]
        //public void C2S_CreateUdpSocketAck(ProudSession session, C2S_CreateUdpSocketAckMessage message)
        //{
        //    if (session.P2PGroup == null || _filter.Config.UdpListener == null)
        //        return;

        //    Logger<>.Debug($"Client:{session.HostId} - Starting server holepunch");
        //    session.Send(new RequestStartServerHolepunchMessage(session.Guid));
        //}

        [MessageHandler(typeof(ReportC2SUdpMessageTrialCountMessage))]
        public void ReportC2SUdpMessageTrialCount()
        { }
    }
}
