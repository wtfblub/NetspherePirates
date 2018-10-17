using System;
using System.Threading.Tasks;
using BlubLib.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using ProudNet.Serialization.Messages;

namespace ProudNet.Handlers
{
    internal class P2PGroupHandler
        : IHandle<P2PGroup_MemberJoin_AckMessage>
    {
        public async Task<bool> OnHandle(MessageContext context, P2PGroup_MemberJoin_AckMessage message)
        {
            var session = context.Session;

            // TODO Use firewall
            session.Logger.LogDebug("P2PGroupMemberJoinAck {@Message}", message);
            if (session.P2PGroup == null || session.HostId == message.AddedMemberHostId)
                return true;

            var remotePeer = session.P2PGroup?.GetMemberInternal(session.HostId);
            var stateA = remotePeer?.ConnectionStates.GetValueOrDefault(message.AddedMemberHostId);
            if (stateA?.EventId != message.EventId)
                return true;

            stateA.IsJoined = true;
            var stateB = stateA.RemotePeer.ConnectionStates.GetValueOrDefault(session.HostId);
            if (stateB?.IsJoined == true)
            {
                if (!session.P2PGroup.AllowDirectP2P)
                    return true;

                // Do not try p2p when the udp relay is not used by one of the clients
                if (!(stateA.RemotePeer is ProudSession sessionA) || !sessionA.UdpEnabled ||
                    !(stateB.RemotePeer is ProudSession sessionB) || !sessionB.UdpEnabled)
                    return true;

                session.Logger.LogDebug("Initialize P2P with {TargetHostId}", stateA.RemotePeer.HostId);
                sessionA.Logger.LogDebug("Initialize P2P with {TargetHostId}", session.HostId);
                stateA.LastHolepunch = stateB.LastHolepunch = DateTimeOffset.Now;
                stateA.IsInitialized = stateB.IsInitialized = true;
                await remotePeer.SendAsync(new P2PRecycleCompleteMessage(stateA.RemotePeer.HostId));
                await stateA.RemotePeer.SendAsync(new P2PRecycleCompleteMessage(session.HostId));
            }

            return true;
        }
    }
}
