using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using BlubLib.Collections.Concurrent;
using BlubLib.DotNetty;
using BlubLib.DotNetty.Handlers.MessageHandling;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProudNet.Codecs;
using ProudNet.Configuration;
using ProudNet.Serialization;
using ProudNet.Serialization.Messages;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Handlers
{
    internal class CoreHandler : ProudMessageHandler
    {
        private readonly IInternalSessionManager<uint> _sessionManager;
        private readonly UdpSocketManager _udpSocketManager;
        private readonly NetworkOptions _networkOptions;
        private readonly Lazy<DateTime> _startTime = new Lazy<DateTime>(() => Process.GetCurrentProcess().StartTime); // TODO Put this somewhere else
        private readonly RSACryptoServiceProvider _rsa;

        public CoreHandler(ISessionManagerFactory sessionManagerFactory, UdpSocketManager udpSocketManager,
            IOptions<NetworkOptions> networkOptions, RSACryptoServiceProvider rsa)
        {
            _sessionManager = sessionManagerFactory.GetSessionManager<uint>(SessionManagerType.HostId);
            _udpSocketManager = udpSocketManager;
            _networkOptions = networkOptions.Value;
            _rsa = rsa;
        }

        [MessageHandler(typeof(RmiMessage))]
        public void RmiMessage(IChannelHandlerContext context, RmiMessage message, RecvContext recvContext)
        {
            var buffer = Unpooled.WrappedBuffer(message.Data);
            recvContext.Message = buffer;
            context.FireChannelRead(recvContext);
        }

        [MessageHandler(typeof(CompressedMessage))]
        public void CompressedMessage(IChannelHandlerContext context, CompressedMessage message, RecvContext recvContext)
        {
            var decompressed = message.Data.DecompressZLib();
            var buffer = Unpooled.WrappedBuffer(decompressed);
            recvContext.Message = buffer;
            context.Channel.Pipeline.Context<ProudFrameDecoder>().FireChannelRead(recvContext);
        }

        [MessageHandler(typeof(EncryptedReliableMessage))]
        public void EncryptedReliableMessage(IChannelHandlerContext context, ProudSession session, EncryptedReliableMessage message, RecvContext recvContext)
        {
            Crypt crypt;
            // TODO Decrypt P2P
            //if (message.IsRelayed)
            //{
            //    //var remotePeer = (ServerRemotePeer)session.P2PGroup?.Members.GetValueOrDefault(message.TargetHostId);
            //    //if (remotePeer == null)
            //    //    return;

            //    //encryptContext = remotePeer.EncryptContext;
            //    //if (encryptContext == null)
            //    //    throw new ProudException($"Received encrypted message but the remote peer has no encryption enabled");
            //}
            //else
            {
                crypt = session.Crypt;
            }

            var buffer = context.Allocator.Buffer(message.Data.Length);
            using (var src = new MemoryStream(message.Data))
            using (var dst = new WriteOnlyByteBufferStream(buffer, false))
                crypt.Decrypt(context.Allocator, message.EncryptMode, src, dst, true);

            recvContext.Message = buffer;
            context.Channel.Pipeline.Context<ProudFrameDecoder>().FireChannelRead(recvContext);
        }

        [MessageHandler(typeof(NotifyCSEncryptedSessionKeyMessage))]
        public void NotifyCSEncryptedSessionKeyMessage(ProudSession session, NotifyCSEncryptedSessionKeyMessage message)
        {
            session.Logger.LogTrace("Handshake:NotifyCSEncryptedSessionKey");
            var secureKey = _rsa.Decrypt(message.SecureKey, true);
            session.Crypt = new Crypt(secureKey);
            session.SendAsync(new NotifyCSSessionKeySuccessMessage());
        }

        [MessageHandler(typeof(NotifyServerConnectionRequestDataMessage))]
        public void NotifyServerConnectionRequestDataMessage(IChannelHandlerContext context, ProudSession session, NotifyServerConnectionRequestDataMessage message)
        {
            session.Logger.LogTrace("Handshake:NotifyServerConnectionRequestData");
            if (message.InternalNetVersion != Constants.NetVersion ||
                    message.Version != _networkOptions.Version)
            {
                session.Logger.LogWarning(
                    "Protocol version mismatch - Client={@ClientVersion} Server={@ServerVersion}",
                    new { NetVersion = message.InternalNetVersion, Version = message.Version },
                    new { NetVersion = Constants.NetVersion, Version = _networkOptions.Version });
                session.SendAsync(new NotifyProtocolVersionMismatchMessage());
                var _ = session.CloseAsync();
                return;
            }

            _sessionManager.AddSession(session.HostId, session);
            session.HandhsakeEvent.Set();
            session.SendAsync(new NotifyServerConnectSuccessMessage(session.HostId, _networkOptions.Version, session.RemoteEndPoint));
        }

        [MessageHandler(typeof(UnreliablePingMessage))]
        public void UnreliablePingHandler(IChannelHandlerContext context, ProudSession session, UnreliablePingMessage message, RecvContext recvContext)
        {
            session.UnreliablePing = TimeSpan.FromSeconds(message.Ping).TotalMilliseconds;
            if (recvContext.UdpEndPoint != null)
                session.LastUdpPing = DateTimeOffset.Now;

            var ts = DateTime.Now - _startTime.Value;
            session.SendUdpIfAvailableAsync(new UnreliablePongMessage(message.ClientTime, ts.TotalSeconds));
        }

        [MessageHandler(typeof(SpeedHackDetectorPingMessage))]
        public void SpeedHackDetectorPingHandler(ProudSession session)
        {
            session.LastSpeedHackDetectorPing = DateTime.Now;
        }

        [MessageHandler(typeof(ReliableRelay1Message))]
        public void ReliableRelayHandler(IChannel channel, ProudSession session, ReliableRelay1Message message)
        {
            if (session.P2PGroup == null)
                return;

            foreach (var destination in message.Destination.Where(d => d.HostId != session.HostId))
            {
                if (session.P2PGroup?.GetMember(destination.HostId) == null)
                    return;

                var target = _sessionManager.GetSession(destination.HostId);
                target?.SendAsync(new ReliableRelay2Message(new RelayDestinationDto(session.HostId, destination.FrameNumber), message.Data));
            }
        }

        [MessageHandler(typeof(UnreliableRelay1Message))]
        public void UnreliableRelayHandler(IChannel channel, ProudSession session, UnreliableRelay1Message message)
        {
            foreach (var destination in message.Destination.Where(id => id != session.HostId))
            {
                if (session.P2PGroup?.GetMember(destination) == null)
                    return;

                var target = _sessionManager.GetSession(destination);
                target?.SendUdpIfAvailableAsync(new UnreliableRelay2Message(session.HostId, message.Data));
            }
        }

        [MessageHandler(typeof(ServerHolepunchMessage))]
        public void ServerHolepunch(ProudSession session, ServerHolepunchMessage message)
        {
            session.Logger.LogDebug("ServerHolepunch={@Message}", message);
            if (session.P2PGroup == null || !_udpSocketManager.IsRunning || session.HolepunchMagicNumber != message.MagicNumber)
                return;

            session.SendUdpAsync(new ServerHolepunchAckMessage(session.HolepunchMagicNumber, session.UdpEndPoint));
        }

        [MessageHandler(typeof(NotifyHolepunchSuccessMessage))]
        public void NotifyHolepunchSuccess(ProudSession session, NotifyHolepunchSuccessMessage message)
        {
            session.Logger.LogDebug("NotifyHolepunchSuccess={@Message}", message);
            if (session.P2PGroup == null || !_udpSocketManager.IsRunning || session.HolepunchMagicNumber != message.MagicNumber)
                return;

            session.LastUdpPing = DateTimeOffset.Now;
            session.UdpEnabled = true;
            session.UdpLocalEndPoint = message.LocalEndPoint;
            session.SendUdpAsync(new NotifyClientServerUdpMatchedMessage(message.MagicNumber));
        }

        [MessageHandler(typeof(PeerUdp_ServerHolepunchMessage))]
        public void PeerUdp_ServerHolepunch(IChannel channel, ProudSession session, PeerUdp_ServerHolepunchMessage message)
        {
            session.Logger.LogDebug("PeerUdp_ServerHolepunch={@Message}", message);
            if (!session.UdpEnabled || !_udpSocketManager.IsRunning)
                return;

            if (!(session.P2PGroup.GetMember(message.HostId) is ProudSession target) || !target.UdpEnabled)
                return;

            session.SendUdpAsync(new PeerUdp_ServerHolepunchAckMessage(message.MagicNumber, target.UdpEndPoint, target.HostId));
        }

        [MessageHandler(typeof(PeerUdp_NotifyHolepunchSuccessMessage))]
        public void PeerUdp_NotifyHolepunchSuccess(IChannel channel, ProudSession session, PeerUdp_NotifyHolepunchSuccessMessage message)
        {
            session.Logger.LogDebug("PeerUdp_NotifyHolepunchSuccess={@Message}", message);
            if (!session.UdpEnabled || !_udpSocketManager.IsRunning)
                return;

            var remotePeer = session.P2PGroup.GetMemberInternal(session.HostId);
            var connectionState = remotePeer.ConnectionStates.GetValueOrDefault(message.HostId);

            connectionState.PeerUdpHolepunchSuccess = true;
            connectionState.LocalEndPoint = message.LocalEndPoint;
            connectionState.EndPoint = message.EndPoint;
            var connectionStateB = connectionState.RemotePeer.ConnectionStates[session.HostId];
            if (connectionStateB.PeerUdpHolepunchSuccess)
            {
                remotePeer.SendAsync(new RequestP2PHolepunchMessage(message.HostId, connectionStateB.LocalEndPoint, connectionState.EndPoint));
                connectionState.RemotePeer.SendAsync(new RequestP2PHolepunchMessage(session.HostId, connectionState.LocalEndPoint, connectionStateB.EndPoint));
            }
        }
    }
}
