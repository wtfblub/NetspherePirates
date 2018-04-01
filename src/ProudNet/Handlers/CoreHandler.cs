using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BlubLib.Collections.Generic;
using BlubLib.Collections.Concurrent;
using BlubLib.DotNetty;
using BlubLib.DotNetty.Handlers.MessageHandling;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ProudNet.Codecs;
using ProudNet.Serialization;
using ProudNet.Serialization.Messages;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Handlers
{
    internal class CoreHandler : ProudMessageHandler
    {
        private readonly ProudServer _server;
        private readonly Lazy<DateTime> _startTime = new Lazy<DateTime>(() => Process.GetCurrentProcess().StartTime);

        public CoreHandler(ProudServer server)
        {
            _server = server;
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
        public void NotifyCSEncryptedSessionKeyMessage(ProudServer server, ProudSession session, NotifyCSEncryptedSessionKeyMessage message)
        {
            session.Logger?.Verbose("Handshake:NotifyCSEncryptedSessionKey");
            var secureKey = server.Rsa.Decrypt(message.SecureKey, true);
            session.Crypt = new Crypt(secureKey);
            session.SendAsync(new NotifyCSSessionKeySuccessMessage());
        }

        [MessageHandler(typeof(NotifyServerConnectionRequestDataMessage))]
        public void NotifyServerConnectionRequestDataMessage(IChannelHandlerContext context, ProudSession session, NotifyServerConnectionRequestDataMessage message)
        {
            session.Logger?.Verbose("Handshake:NotifyServerConnectionRequestData");
            if (message.InternalNetVersion != Constants.NetVersion ||
                    message.Version != _server.Configuration.Version)
            {

                session.Logger?.Warning(
                    "Protocol version mismatch - Client={@ClientVersion} Server={@ServerVersion}",
                    new { NetVersion = message.InternalNetVersion, Version = message.Version },
                    new { NetVersion = Constants.NetVersion, Version = _server.Configuration.Version });
                session.SendAsync(new NotifyProtocolVersionMismatchMessage());
                session.CloseAsync();
                return;
            }

            _server.AddSession(session);
            session.HandhsakeEvent.Set();
            session.SendAsync(new NotifyServerConnectSuccessMessage(session.HostId, _server.Configuration.Version, session.RemoteEndPoint));
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
                if (session.P2PGroup == null)
                {
                    //Logger<>.Debug($"Client {session.HostId} is not in a P2PGroup");
                    continue;
                }

                if (!session.P2PGroup.Members.ContainsKey(destination.HostId))
                {
                    //Logger<>.Debug($"Client {session.HostId} trying to relay to non existant {destination.HostId}");
                    continue;
                }

                var target = _server.Sessions.GetValueOrDefault(destination.HostId);
                target?.SendAsync(new ReliableRelay2Message(new RelayDestinationDto(session.HostId, destination.FrameNumber), message.Data));
            }
        }

        [MessageHandler(typeof(UnreliableRelay1Message))]
        public void UnreliableRelayHandler(IChannel channel, ProudSession session, UnreliableRelay1Message message)
        {
            foreach (var destination in message.Destination.Where(id => id != session.HostId))
            {
                if (session.P2PGroup == null)
                {
                    //Logger<>.Debug($"Client {session.HostId} in not a p2pgroup");
                    continue;
                }

                if (!session.P2PGroup.Members.ContainsKey(destination))
                {
                    //Logger<>.Debug($"Client {session.HostId} trying to relay to non existant {destination}");
                    continue;
                }

                var target = _server.Sessions.GetValueOrDefault(destination);
                target?.SendUdpIfAvailableAsync(new UnreliableRelay2Message(session.HostId, message.Data));
            }
        }

        [MessageHandler(typeof(ServerHolepunchMessage))]
        public void ServerHolepunch(ProudServer server, ProudSession session, ServerHolepunchMessage message)
        {
            session.Logger?.Debug("ServerHolepunch={@Message}", message);
            if (session.P2PGroup == null || !_server.UdpSocketManager.IsRunning || session.HolepunchMagicNumber != message.MagicNumber)
                return;

            session.SendUdpAsync(new ServerHolepunchAckMessage(session.HolepunchMagicNumber, session.UdpEndPoint));
        }

        [MessageHandler(typeof(NotifyHolepunchSuccessMessage))]
        public void NotifyHolepunchSuccess(ProudServer server, ProudSession session, NotifyHolepunchSuccessMessage message)
        {
            session.Logger?.Debug("NotifyHolepunchSuccess={@Message}", message);
            if (session.P2PGroup == null || !_server.UdpSocketManager.IsRunning || session.HolepunchMagicNumber != message.MagicNumber)
                return;

            session.LastUdpPing = DateTimeOffset.Now;
            session.UdpEnabled = true;
            session.UdpLocalEndPoint = message.LocalEndPoint;
            session.SendUdpAsync(new NotifyClientServerUdpMatchedMessage(message.MagicNumber));
        }

        [MessageHandler(typeof(PeerUdp_ServerHolepunchMessage))]
        public void PeerUdp_ServerHolepunch(IChannel channel, ProudSession session, PeerUdp_ServerHolepunchMessage message)
        {
            session.Logger?.Debug("PeerUdp_ServerHolepunch={@Message}", message);
            if (!session.UdpEnabled || !_server.UdpSocketManager.IsRunning)
                return;

            var target = session.P2PGroup?.Members.GetValueOrDefault(message.HostId)?.Session;
            if (target == null || !target.UdpEnabled)
                return;

            session.SendUdpAsync(new PeerUdp_ServerHolepunchAckMessage(message.MagicNumber, target.UdpEndPoint, target.HostId));
        }

        [MessageHandler(typeof(PeerUdp_NotifyHolepunchSuccessMessage))]
        public void PeerUdp_NotifyHolepunchSuccess(IChannel channel, ProudSession session, PeerUdp_NotifyHolepunchSuccessMessage message)
        {
            session.Logger?.Debug("PeerUdp_NotifyHolepunchSuccess={@Message}", message);
            if (!session.UdpEnabled || !_server.UdpSocketManager.IsRunning)
                return;

            var remotePeer = session.P2PGroup.Members[session.HostId];
            var connectionState = remotePeer.ConnectionStates.GetValueOrDefault(message.HostId);

            connectionState.PeerUdpHolepunchSuccess = true;
            connectionState.LocalEndPoint = message.LocalEndPoint;
            connectionState.EndPoint = message.EndPoint;
            var connectionStateB = connectionState.RemotePeer.ConnectionStates[session.HostId];
            if (connectionStateB.PeerUdpHolepunchSuccess)
            {
                remotePeer.SendAsync(new RequestP2PHolepunchMessage(message.HostId, connectionStateB.LocalEndPoint, connectionState.EndPoint));
                connectionState.RemotePeer.SendAsync(new RequestP2PHolepunchMessage(session.HostId, connectionState.LocalEndPoint, connectionStateB.EndPoint));

                //remotePeer.SendAsync(new RequestP2PHolepunchMessage(message.HostId, message.LocalEndPoint, new IPEndPoint(message.EndPoint.Address, message.LocalEndPoint.Port)));
                //connectionState.RemotePeer.SendAsync(new RequestP2PHolepunchMessage(session.HostId, session.UdpLocalEndPoint, new IPEndPoint(session.UdpEndPoint.Address, session.UdpLocalEndPoint.Port)));
                //remotePeer.SendAsync(new RequestP2PHolepunchMessage(message.HostId, message.LocalEndPoint, message.EndPoint));
                //connectionState.RemotePeer.SendAsync(new RequestP2PHolepunchMessage(session.HostId, session.UdpLocalEndPoint, session.UdpEndPoint));
            }
        }
    }
}
