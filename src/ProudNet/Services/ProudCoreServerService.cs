using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BlubLib.IO;
using BlubLib.Network;
using BlubLib.Network.Pipes;
using BlubLib.Network.Transport.Sockets;
using BlubLib.Serialization;
using NLog;
using NLog.Fluent;
using ProudNet.Data;
using ProudNet.Message;
using ProudNet.Message.Core;

namespace ProudNet.Services
{
    internal class ProudCoreServerService : MessageHandler
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ProudServerPipe _filter;
        private readonly Lazy<DateTime> _startTime = new Lazy<DateTime>(() => Process.GetCurrentProcess().StartTime);

        public ProudCoreServerService(ProudServerPipe filter)
        {
            _filter = filter;
        }

        [MessageHandler(typeof(EncryptedReliableMessage))]
        public void EncryptedReliableMessage(ProudSession session, EncryptedReliableMessage message, MessageReceivedEventArgs e)
        {
            EncryptContext encryptContext;
            if (message.IsRelayed)
            {
                var remotePeer = (ServerRemotePeer)session.P2PGroup?.Members.GetValueOrDefault(message.TargetHostId);
                if (remotePeer == null)
                    return;

                encryptContext = remotePeer.EncryptContext;
                if (encryptContext == null)
                    throw new ProudException($"Received encrypted message but the remote peer has no encryption enabled");
            }
            else
            {
                encryptContext = session.EncryptContext;
            }

            var decrypted = encryptContext.Decrypt(message.Data);

            ushort decryptCounter;
            byte[] data;
            using (var r = decrypted.ToBinaryReader())
            {
                decryptCounter = r.ReadUInt16();
                data = r.ReadToEnd();
            }

            if (decryptCounter != (session.EncryptContext.DecryptCounter - 1))
                throw new ProudException($"Invalid DecryptCounter! Message: {decryptCounter} Client: {session.EncryptContext.DecryptCounter}");

            ProudCoreOpCode opCode;
            using (var r = data.ToBinaryReader())
            {
                opCode = r.ReadEnum<ProudCoreOpCode>();
                data = r.ReadToEnd();
            }

            switch (opCode)
            {
                case ProudCoreOpCode.Rmi:
                    var rmi = new RmiMessage(data)
                    {
                        IsRelayed = message.IsRelayed,
                        SenderHostId = message.SenderHostId,
                        TargetHostId = message.TargetHostId
                    };
                    e.Message = rmi;
                    _filter.OnMessageReceived(e);
                    break;

                case ProudCoreOpCode.Compressed:
                    CompressedMessage compressed;
                    using (var r = data.ToBinaryReader())
                        compressed = Serializer.Deserialize<CompressedMessage>(r);
                    compressed.IsRelayed = message.IsRelayed;
                    compressed.SenderHostId = message.SenderHostId;
                    compressed.TargetHostId = message.TargetHostId;
                    e.Message = compressed;
                    _filter.OnMessageReceived(e);
                    break;

                default:
                    throw new ProudException("Invalid opCode inside EncryptedMessage: " + opCode);
            }
        }

        [MessageHandler(typeof(NotifyCSEncryptedSessionKeyMessage))]
        public void NotifyCSEncryptedSessionKeyMessage(IService service, ProudSession session, NotifyCSEncryptedSessionKeyMessage message)
        {
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                rsa.ImportCspBlob(message.Key);
                session.EncryptContext = new EncryptContext(128 /*_filter.Config.EncryptedMessageKeyLength*/);

                byte[] blob;
                using (var w = new BinaryWriter(new PooledMemoryStream(service.ArrayPool)))
                {
                    w.Write((byte)1);
                    w.Write((byte)2);
                    w.Write((byte)0);
                    w.Write((byte)0);
                    w.Write(26625);
                    w.Write(41984);

                    var encrypted = rsa.Encrypt(session.EncryptContext.RC4.Key, false);
                    w.Write(encrypted.Reverse());
                    blob = w.ToArray();
                }
                session.Send(new NotifyCSSessionKeySuccessMessage(blob));
            }
        }

        [MessageHandler(typeof(NotifyServerConnectionRequestDataMessage))]
        public async Task NotifyServerConnectionRequestDataMessage(ProudSession session, NotifyServerConnectionRequestDataMessage message)
        {
            if (message.InternalNetVersion != ProudConfig.InternalNetVersion ||
                    message.Version != _filter.Config.Version)
            {
                await session.SendAsync(new NotifyProtocolVersionMismatchMessage())
                    .ConfigureAwait(false);
                session.Close();
                return;
            }

            IPEndPoint ip;
            var processor = session.Transport as TcpTransport;
            if (processor != null)
                ip = (IPEndPoint)processor.Socket.RemoteEndPoint;
            else
                ip = new IPEndPoint(0, 0);

            var hostId = await _filter.GetNextHostIdAsync().ConfigureAwait(false);
            session.HostId = hostId;
            _filter.SessionLookupByHostId.TryAdd(session.HostId, session);
            session.ReadyEvent.Set();

            await session.SendAsync(new NotifyServerConnectSuccessMessage(hostId, _filter.Config.Version, ip))
                .ConfigureAwait(false);
        }

        [MessageHandler(typeof(UnreliablePingMessage))]
        public void UnreliablePingHandler(ProudSession session, UnreliablePingMessage message)
        {
            session.UnreliablePing = TimeSpan.FromSeconds(message.Ping).TotalMilliseconds;
            var ts = DateTime.Now - _startTime.Value;
            session.Send(new UnreliablePongMessage(message.ClientTime, ts.TotalSeconds));
        }

        [MessageHandler(typeof(SpeedHackDetectorPingMessage))]
        public void SpeedHackDetectorPingHandler(ProudSession session)
        {
            session.LastSpeedHackDetectorPing = DateTime.Now;
        }

        [MessageHandler(typeof(ReliableRelay1Message))]
        public void ReliableRelayHandler(ProudSession session, ReliableRelay1Message message, MessageReceivedEventArgs e)
        {
            if (session.P2PGroup == null)
                return;

            foreach (var destination in message.Destination.Where(d => d.HostId != session.HostId))
            {
                if (session.P2PGroup == null)
                {
                    Logger.Debug()
                        .Message("Client {0} is not in a P2PGroup", session.HostId)
                        .Write();
                    return;
                }

                if (!session.P2PGroup.Members.ContainsKey(destination.HostId))
                {
                    Logger.Debug()
                        .Message("Client {0} trying to relay to non existant {1}", session.HostId, destination.HostId)
                        .Write();
                    return;
                }

                if (destination.HostId == 2)
                {
                    #region Hardcoded ServerMember

                    ProudCoreOpCode opCode;
                    byte[] data;
                    using (var r = message.Data.ToBinaryReader())
                    {
                        opCode = r.ReadEnum<ProudCoreOpCode>();
                        data = r.ReadToEnd();
                    }

                    if (opCode == ProudCoreOpCode.Rmi)
                    {
                        var core = new RmiMessage(data)
                        {
                            IsRelayed = true,
                            SenderHostId = session.HostId,
                            TargetHostId = destination.HostId
                        };
                        e.Message = core;
                        _filter.OnMessageReceived(e);
                    }
                    else if (opCode == ProudCoreOpCode.ReliableUdp_Frame)
                    {
                        ReliableUdp_FrameMessage udpFrameMessage;
                        using (var r = data.ToBinaryReader())
                            udpFrameMessage = Serializer.Deserialize<ReliableUdp_FrameMessage>(r);

                        using (var r = udpFrameMessage.Data.ToBinaryReader())
                        {
                            opCode = r.ReadEnum<ProudCoreOpCode>();
                            data = r.ReadToEnd();
                        }

                        CoreMessage core;
                        if (opCode == ProudCoreOpCode.Rmi)
                        {
                            core = new RmiMessage(data)
                            {
                                IsRelayed = true,
                                SenderHostId = session.HostId,
                                TargetHostId = destination.HostId
                            };
                        }
                        else if (opCode == ProudCoreOpCode.EncryptedReliable)
                        {
                            using (var r = data.ToBinaryReader())
                                core = Serializer.Deserialize<EncryptedReliableMessage>(r);
                            core.IsRelayed = true;
                            core.SenderHostId = session.HostId;
                            core.TargetHostId = destination.HostId;
                        }
                        else
                            throw new ProudException($"Invalid opCode {opCode}");

                        e.Message = core;
                        _filter.OnMessageReceived(e);
                    }
                    else
                    {
                        throw new ProudException($"Invalid opCode {opCode}");
                    }

                    #endregion
                }
                else
                {
                    var target = _filter.SessionLookupByHostId.GetValueOrDefault(destination.HostId);
                    target?.Send(new ReliableRelay2Message(new RelayDestinationDto(session.HostId, destination.FrameNumber), message.Data));
                }
            }
        }

        [MessageHandler(typeof(UnreliableRelay1Message))]
        public void UnreliableRelayHandler(ProudSession session, UnreliableRelay1Message message, MessageReceivedEventArgs e)
        {
            foreach (var destination in message.Destination.Where(id => id != session.HostId))
            {
                if (session.P2PGroup == null)
                {
                    Logger.Debug()
                        .Message("Client {0} in not a p2pgroup", session.HostId)
                        .Write();
                    return;
                }

                if (!session.P2PGroup.Members.ContainsKey(destination))
                {
                    Logger.Debug()
                        .Message("Client {0} trying to relay to non existant {1}", session.HostId, destination)
                        .Write();
                    return;
                }

                if (destination == 2)
                {
                    #region Hardcoded ServerMember

                    ProudCoreOpCode opCode;
                    byte[] data;
                    using (var r = message.Data.ToBinaryReader())
                    {
                        opCode = r.ReadEnum<ProudCoreOpCode>();
                        data = r.ReadToEnd();
                    }

                    if (opCode == ProudCoreOpCode.Rmi)
                    {
                        var core = new RmiMessage(data)
                        {
                            IsRelayed = true,
                            SenderHostId = session.HostId,
                            TargetHostId = destination
                        };
                        e.Message = core;
                        _filter.OnMessageReceived(e);
                    }
                    else
                    {
                        throw new ProudException($"Invalid opCode {opCode}");
                    }

                    #endregion
                }
                else
                {
                    var target = _filter.SessionLookupByHostId.GetValueOrDefault(destination);
                    target?.Send(new UnreliableRelay2Message(session.HostId, message.Data));
                }
            }
        }

        [MessageHandler(typeof(NotifyHolepunchSuccessMessage))]
        public void NotifyHolepunchSuccess(ProudSession session, NotifyHolepunchSuccessMessage message)
        {
            if (session.P2PGroup == null || _filter.Config.UdpListener == null || session.Guid != message.MagicNumber)
                return;

            Logger.Debug()
                .Message("Client:{0} - Server holepunch success(EndPoint:{1} LocalEndPoint:{2})",
                    session.HostId, message.EndPoint, message.LocalEndPoint)
                .Write();

            session.UdpEnabled = true;
            session.UdpEndPoint = message.EndPoint;
            session.UdpLocalEndPoint = message.LocalEndPoint;
            session.UdpSocket = _filter.UdpSocket;
            session.Send(new NotifyClientServerUdpMatchedMessage(message.MagicNumber));
        }

        [MessageHandler(typeof(PeerUdp_ServerHolepunchMessage))]
        public void PeerUdp_ServerHolepunch(ProudSession session, PeerUdp_ServerHolepunchMessage message)
        {
            if (!session.UdpEnabled || _filter.Config.UdpListener == null)
                return;

            var target = _filter.SessionLookupByHostId.GetValueOrDefault(message.HostId);
            if (target == null || !target.UdpEnabled)
                return;

            session.Send(new PeerUdp_ServerHolepunchAckMessage(message.MagicNumber, target.UdpEndPoint, target.HostId));
        }

        [MessageHandler(typeof(PeerUdp_NotifyHolepunchSuccessMessage))]
        public void PeerUdp_NotifyHolepunchSuccess(ProudSession session, PeerUdp_NotifyHolepunchSuccessMessage message)
        {
            if (!session.UdpEnabled || _filter.Config.UdpListener == null)
                return;

            Logger.Debug()
                .Message("Client:{0} - Peer server holepunch success(EndPoint:{1} LocalEndPoint:{2})",
                    session.HostId, message.EndPoint, message.LocalEndPoint)
                .Write();

            // ToDo Refactor this shit...
            Task.Delay(2000).ContinueWith(_ =>
            {
                session.Send(new RequestP2PHolepunchMessage(message.HostId, message.LocalEndPoint, message.EndPoint));
            });
        }
    }
}