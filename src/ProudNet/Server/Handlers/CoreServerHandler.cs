using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BlubLib.DotNetty;
using BlubLib.DotNetty.Handlers.MessageHandling;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ProudNet.Codecs;
using ProudNet.Handlers;
using ProudNet.Serialization;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Server.Handlers
{
    internal class CoreServerHandler : ProudMessageHandler
    {
        private readonly ProudServer _server;
        private readonly Lazy<DateTime> _startTime = new Lazy<DateTime>(() => Process.GetCurrentProcess().StartTime);

        public CoreServerHandler(ProudServer server)
        {
            _server = server;
        }

        [MessageHandler(typeof(EncryptedReliableMessage))]
        public void EncryptedReliableMessage(IChannelHandlerContext context, ProudSession session, EncryptedReliableMessage message)
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
                crypt.Decrypt(src, dst, true);

            context.Channel.Pipeline.Context<ProudFrameDecoder>().FireChannelRead(buffer);
        }

        [MessageHandler(typeof(NotifyCSEncryptedSessionKeyMessage))]
        public async Task NotifyCSEncryptedSessionKeyMessage(IChannelHandlerContext context, ProudSession session, NotifyCSEncryptedSessionKeyMessage message)
        {
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                rsa.ImportCspBlob(message.Key);
                session.Crypt = new Crypt(_server.Configuration.EncryptedMessageKeyLength);

                byte[] blob;
                using (var w = new MemoryStream().ToBinaryWriter(false))
                {
                    w.Write((byte)1);
                    w.Write((byte)2);
                    w.Write((byte)0);
                    w.Write((byte)0);
                    w.Write(26625);
                    w.Write(41984);

                    var encrypted = rsa.Encrypt(session.Crypt.RC4.Key, false);
                    w.Write(encrypted.Reverse());
                    blob = w.ToArray();
                }

                await session.SendAsync(new NotifyCSSessionKeySuccessMessage(blob));
            }
        }

        [MessageHandler(typeof(NotifyServerConnectionRequestDataMessage))]
        public async Task NotifyServerConnectionRequestDataMessage(IChannelHandlerContext context, ProudSession session, NotifyServerConnectionRequestDataMessage message)
        {
            if (message.InternalNetVersion != Constants.NetVersion ||
                    message.Version != _server.Configuration.Version)
            {
                await session.SendAsync(new NotifyProtocolVersionMismatchMessage());
                await session.CloseAsync();
                return;
            }

            _server.AddSession(session);
            session.HandhsakeEvent.Set();

            var remoteEndpoint = (IPEndPoint)((ISocketChannel)context.Channel).RemoteAddress;
            remoteEndpoint = new IPEndPoint(remoteEndpoint.Address.MapToIPv4(), remoteEndpoint.Port);
            await session.SendAsync(new NotifyServerConnectSuccessMessage(session.HostId, _server.Configuration.Version, remoteEndpoint));
        }

        [MessageHandler(typeof(UnreliablePingMessage))]
        public async Task UnreliablePingHandler(IChannelHandlerContext context, ProudSession session, UnreliablePingMessage message)
        {
            session.UnreliablePing = TimeSpan.FromSeconds(message.Ping).TotalMilliseconds;
            var ts = DateTime.Now - _startTime.Value;
            await session.SendAsync(new UnreliablePongMessage(message.ClientTime, ts.TotalSeconds));
        }

        [MessageHandler(typeof(SpeedHackDetectorPingMessage))]
        public void SpeedHackDetectorPingHandler(ProudSession session)
        {
            session.LastSpeedHackDetectorPing = DateTime.Now;
        }

        [MessageHandler(typeof(ReliableRelay1Message))]
        public async Task ReliableRelayHandler(IChannel channel, ProudSession session, ReliableRelay1Message message)
        {
            if (session.P2PGroup == null)
                return;

            foreach (var destination in message.Destination.Where(d => d.HostId != session.HostId))
            {
                if (session.P2PGroup == null)
                {
                    //Logger<>.Debug($"Client {session.HostId} is not in a P2PGroup");
                    return;
                }

                if (!session.P2PGroup.Members.ContainsKey(destination.HostId))
                {
                    //Logger<>.Debug($"Client {session.HostId} trying to relay to non existant {destination.HostId}");
                    return;
                }

                if (destination.HostId == 2)
                {
                    //#region Hardcoded ServerMember

                    //ProudCoreOpCode opCode;
                    //byte[] data;
                    //using (var r = message.Data.ToBinaryReader())
                    //{
                    //    opCode = r.ReadEnum<ProudCoreOpCode>();
                    //    data = r.ReadToEnd();
                    //}

                    //if (opCode == ProudCoreOpCode.Rmi)
                    //{
                    //    var core = new RmiMessage(data)
                    //    {
                    //        IsRelayed = true,
                    //        SenderHostId = session.HostId,
                    //        TargetHostId = destination.HostId
                    //    };
                    //    e.Message = core;
                    //    _filter.OnMessageReceived(e);
                    //}
                    //else if (opCode == ProudCoreOpCode.ReliableUdp_Frame)
                    //{
                    //    ReliableUdp_FrameMessage udpFrameMessage;
                    //    using (var r = data.ToBinaryReader())
                    //        udpFrameMessage = Serializer.Deserialize<ReliableUdp_FrameMessage>(r);

                    //    using (var r = udpFrameMessage.Data.ToBinaryReader())
                    //    {
                    //        opCode = r.ReadEnum<ProudCoreOpCode>();
                    //        data = r.ReadToEnd();
                    //    }

                    //    CoreMessage core;
                    //    if (opCode == ProudCoreOpCode.Rmi)
                    //    {
                    //        core = new RmiMessage(data)
                    //        {
                    //            IsRelayed = true,
                    //            SenderHostId = session.HostId,
                    //            TargetHostId = destination.HostId
                    //        };
                    //    }
                    //    else if (opCode == ProudCoreOpCode.EncryptedReliable)
                    //    {
                    //        using (var r = data.ToBinaryReader())
                    //            core = Serializer.Deserialize<EncryptedReliableMessage>(r);
                    //        core.IsRelayed = true;
                    //        core.SenderHostId = session.HostId;
                    //        core.TargetHostId = destination.HostId;
                    //    }
                    //    else
                    //        throw new ProudException($"Invalid opCode {opCode}");

                    //    e.Message = core;
                    //    _filter.OnMessageReceived(e);
                    //}
                    //else
                    //{
                    //    throw new ProudException($"Invalid opCode {opCode}");
                    //}

                    //#endregion
                }
                else
                {
                    var target = _server.Sessions.GetValueOrDefault(destination.HostId);
                    if (target != null)
                        await target.SendAsync(new ReliableRelay2Message(new RelayDestinationDto(session.HostId, destination.FrameNumber), message.Data));
                }
            }
        }

        [MessageHandler(typeof(UnreliableRelay1Message))]
        public async Task UnreliableRelayHandler(IChannel channel, ProudSession session, UnreliableRelay1Message message)
        {
            foreach (var destination in message.Destination.Where(id => id != session.HostId))
            {
                if (session.P2PGroup == null)
                {
                    //Logger<>.Debug($"Client {session.HostId} in not a p2pgroup");
                    return;
                }

                if (!session.P2PGroup.Members.ContainsKey(destination))
                {
                    //Logger<>.Debug($"Client {session.HostId} trying to relay to non existant {destination}");
                    return;
                }

                if (destination == 2)
                {
                    //#region Hardcoded ServerMember

                    //ProudCoreOpCode opCode;
                    //byte[] data;
                    //using (var r = message.Data.ToBinaryReader())
                    //{
                    //    opCode = r.ReadEnum<ProudCoreOpCode>();
                    //    data = r.ReadToEnd();
                    //}

                    //if (opCode == ProudCoreOpCode.Rmi)
                    //{
                    //    var core = new RmiMessage(data)
                    //    {
                    //        IsRelayed = true,
                    //        SenderHostId = session.HostId,
                    //        TargetHostId = destination
                    //    };
                    //    e.Message = core;
                    //    _filter.OnMessageReceived(e);
                    //}
                    //else
                    //{
                    //    throw new ProudException($"Invalid opCode {opCode}");
                    //}

                    //#endregion
                }
                else
                {
                    var target = _server.Sessions.GetValueOrDefault(destination);
                    if (target != null)
                        await target.SendAsync(new UnreliableRelay2Message(session.HostId, message.Data));
                }
            }
        }

        //[MessageHandler(typeof(NotifyHolepunchSuccessMessage))]
        //public async Task NotifyHolepunchSuccess(IChannel channel, ProudSession session, NotifyHolepunchSuccessMessage message)
        //{
        //    if (session.P2PGroup == null || _filter.Config.UdpListener == null || session.Guid != message.MagicNumber)
        //        return;

        //    //Logger<>.Debug($"Client:{session.HostId} - Server holepunch success(EndPoint:{message.EndPoint} LocalEndPoint:{message.LocalEndPoint})");

        //    session.UdpEnabled = true;
        //    session.UdpEndPoint = message.EndPoint;
        //    session.UdpLocalEndPoint = message.LocalEndPoint;
        //    session.UdpSocket = _filter.UdpSocket;
        //    await session.SendAsync(new NotifyClientServerUdpMatchedMessage(message.MagicNumber));
        //}

        //[MessageHandler(typeof(PeerUdp_ServerHolepunchMessage))]
        //public async Task PeerUdp_ServerHolepunch(IChannel channel, ProudSession session, PeerUdp_ServerHolepunchMessage message)
        //{
        //    if (!session.UdpEnabled || _filter.Config.UdpListener == null)
        //        return;

        //    var target = _server.SessionLookupByHostId.GetValueOrDefault(message.HostId);
        //    if (target == null || !target.UdpEnabled)
        //        return;

        //    await session.SendAsync(new PeerUdp_ServerHolepunchAckMessage(message.MagicNumber, target.UdpEndPoint, target.HostId));
        //}

        //[MessageHandler(typeof(PeerUdp_NotifyHolepunchSuccessMessage))]
        //public async Task PeerUdp_NotifyHolepunchSuccess(IChannel channel, ProudSession session, PeerUdp_NotifyHolepunchSuccessMessage message)
        //{
        //    if (!session.UdpEnabled || _filter.Config.UdpListener == null)
        //        return;

        //    //Logger<>.Debug($"Client:{session.HostId} - Peer server holepunch success(EndPoint:{message.EndPoint} LocalEndPoint:{message.LocalEndPoint})");

        //    // ToDo Refactor this
        //    await Task.Delay(2000);
        //    await session.SendAsync(new RequestP2PHolepunchMessage(message.HostId, message.LocalEndPoint, message.EndPoint));
        //}
    }
}