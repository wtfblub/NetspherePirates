using System.Threading.Tasks;
using BlubLib.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using ProudNet.Firewall;
using ProudNet.Serialization.Messages;

namespace ProudNet.Handlers
{
    internal class P2PStatusHandler
        : IHandle<P2P_NotifyDirectP2PDisconnectedMessage>,
          IHandle<NotifyUdpToTcpFallbackByClientMessage>
    {
        private readonly IInternalSessionManager<uint> _udpSessionManager;

        public P2PStatusHandler(ISessionManagerFactory sessionManagerFactory)
        {
            _udpSessionManager = sessionManagerFactory.GetSessionManager<uint>(SessionManagerType.UdpId);
        }

        [Firewall(typeof(MustBeInP2PGroup))]
        public async Task<bool> OnHandle(MessageContext context, P2P_NotifyDirectP2PDisconnectedMessage message)
        {
            var session = context.Session;

            session.Logger.LogDebug("P2P_NotifyDirectP2PDisconnected {@Message}", message);
            var remotePeer = session.P2PGroup.GetMemberInternal(session.HostId);
            var stateA = remotePeer?.ConnectionStates.GetValueOrDefault(message.RemotePeerHostId);
            var stateB = stateA?.RemotePeer.ConnectionStates.GetValueOrDefault(session.HostId);
            if (stateA?.HolepunchSuccess == true)
            {
                session.Logger.LogInformation("P2P to {TargetHostId} disconnected with {Reason}",
                    message.RemotePeerHostId, message.Reason);
                stateA.HolepunchSuccess = false;
                stateA.RemotePeer.Send(new P2P_NotifyDirectP2PDisconnected2Message(session.HostId, message.Reason));
            }

            if (stateB?.HolepunchSuccess == true)
                stateB.HolepunchSuccess = false;

            return true;
        }

        [Firewall(typeof(MustBeUdpRelay))]
        public Task<bool> OnHandle(MessageContext context, NotifyUdpToTcpFallbackByClientMessage message)
        {
            var session = context.Session;

            session.Logger.LogDebug("Fallback to tcp relay by client");
            session.UdpEnabled = false;
            _udpSessionManager.RemoveSession(session.UdpSessionId);
            return Task.FromResult(true);
        }
    }
}
