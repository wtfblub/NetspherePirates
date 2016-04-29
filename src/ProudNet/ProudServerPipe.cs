using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using BlubLib.IO;
using BlubLib.Network;
using BlubLib.Network.Message;
using BlubLib.Network.Transport.Sockets;
using BlubLib.Threading.Tasks;
using NLog;
using NLog.Fluent;
using ProudNet.Message;
using ProudNet.Message.Core;
using ProudNet.Services;

namespace ProudNet
{
    public class ProudServerPipe : ProudPipe
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(15);

        // ToDo refactor hostId creation
        private readonly HashSet<uint> _hostIds = new HashSet<uint>();
        private readonly AsyncLock _sync = new AsyncLock();
        
        internal UdpServerSocket UdpSocket { get; private set; }

        public ServerP2PGroupManager P2PGroupManager { get; }
        internal ConcurrentDictionary<uint, ProudSession> SessionLookupByHostId { get; }

        public ProudServerPipe(ProudConfig config)
            : base(config)
        {
            P2PGroupManager = new ServerP2PGroupManager(this);
            SessionLookupByHostId = new ConcurrentDictionary<uint, ProudSession>();
            AddCoreService(new ProudCoreServerService(this));
            AddService(new ProudServerService(this));
        }

        public override async void OnConnected(SessionEventArgs e)
        {
            var processor = (TcpTransport)e.Session.Transport;
            var remoteEndPoint = (IPEndPoint)processor.Socket.RemoteEndPoint;

            Logger.Debug()
                .Message("New incoming client on {0}", remoteEndPoint.ToString())
                .Write();

            await e.Session.SendAsync(new NotifyServerConnectionHintMessage(Config))
                .ConfigureAwait(false);

            using (var cts = new CancellationTokenSource(s_timeout))
            {
                try
                {
                    await ((ProudSession)e.Session).ReadyEvent.WaitAsync(cts.Token)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    if (!e.Session.IsConnected)
                        return;

                    Logger.Error()
                        .Message("Handshake timeout for {0}", remoteEndPoint)
                        .Write();

                    await e.Session.SendAsync(new ConnectServerTimedoutMessage())
                        .ConfigureAwait(false);
                    e.Session.Dispose();
                    return;
                }
            }

            try
            {
                base.OnConnected(e);
            }
            catch (Exception ex)
            {
                Service.Pipeline.OnError(new ExceptionEventArgs(e.Session, ex));
                e.Session.Dispose();
            }
        }

        public override void OnDisconnected(SessionEventArgs e)
        {
            var session = (ProudSession)e.Session;
            ((ServerP2PGroup)session.P2PGroup)?.Leave(session.HostId);
            SessionLookupByHostId.Remove(session.HostId);

            // No need to wait for it
#pragma warning disable 4014
            FreeHostIdAsync(session.HostId);
#pragma warning restore 4014

            base.OnDisconnected(e);
        }

        public override async Task OnSendMessage(MessageEventArgs e)
        {
            var session = (ProudSession)e.Session;
            var message = (ProudMessage)e.Message;

            if (!session.ReadyEvent.IsSet)
                throw new ProudException("Client is not connected");

            if (message.IsRelayed)
            {
                await session.SendAsync(new UnreliableRelay2Message(message.SenderHostId, new RmiMessage(message.ToArray()).ToArray()))
                    .ConfigureAwait(false);
            }

            await ProcessAndSendMessage(e, session.EncryptContext)
                .ConfigureAwait(false);
        }

        public override void OnStart()
        {
            Logger.Debug()
                .Message("Initializing...")
                .Write();

            if (Config.UdpListener != null)
            {
                Logger.Debug()
                    .Message("Creating udp socket on {0}", Config.UdpListener.ToString())
                    .Write();

                // ToDo Add the possibility to listen on multiple ports
                UdpSocket = new UdpServerSocket(this);
                UdpSocket.Start();
            }

            base.OnStart();
        }

        public override void OnClose()
        {
            Logger.Debug()
                .Message("Closing...")
                .Write();

            if (UdpSocket != null)
            {
                UdpSocket.Dispose();
                UdpSocket = null;
            }

            base.OnClose();
        }

        public ProudSession GetSessionByHostId(uint hostId)
        {
            return SessionLookupByHostId.GetValueOrDefault(hostId);
        }

        #region HostIds

        internal uint GetNextHostId()
        {
            using (_sync.Lock())
            {
                uint hostId = 1000;
                while (_hostIds.Contains(hostId))
                    hostId++;

                _hostIds.Add(hostId);
                return hostId;
            }
        }

        internal async Task<uint> GetNextHostIdAsync()
        {
            using (await _sync.LockAsync().ConfigureAwait(false))
            {
                uint hostId = 1000;
                while (_hostIds.Contains(hostId))
                    hostId++;

                _hostIds.Add(hostId);
                return hostId;
            }
        }

        internal void FreeHostId(uint hostId)
        {
            using (_sync.Lock())
                _hostIds.Remove(hostId);
        }

        internal async Task FreeHostIdAsync(uint hostId)
        {
            using (await _sync.LockAsync().ConfigureAwait(false))
                _hostIds.Remove(hostId);
        }

        #endregion
    }

    internal class UdpServerSocket : IDisposable
    {
        private const int Mtu = 500;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ProudProtocol _protocol = new ProudProtocol();
        private readonly byte[] _buffer = new byte[Mtu];
        private Socket _socket;

        private readonly ProudServerPipe _pipe;

        public UdpServerSocket(ProudServerPipe pipe)
        {
            _pipe = pipe;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(pipe.Config.UdpListener);
        }

        public void Send(ProudSession session, CoreMessage message)
        {
            using (var ms = new PooledMemoryStream(_pipe.Service.ArrayPool))
            using (var w = new BinaryWriter(ms))
            {
                var @out = _protocol.Serialize(session, message);
                using (var ms2 = new PooledMemoryStream(_pipe.Service.ArrayPool))
                {
                    @out.Serialize(ms2);
                    var segment = ms2.ToSegment();

                    w.Write((ushort)43981);
                    w.Write((ushort)0);
                    w.Write(segment.Count);
                    w.Write(0);
                    w.Write(0);
                    w.Write(segment.Array, segment.Offset, segment.Count);

                    segment = ms.ToSegment();
                    _socket.SendTo(segment.Array, segment.Offset, segment.Count, SocketFlags.None, session.UdpEndPoint);
                }
            }
        }

        public async Task SendAsync(ProudSession session, CoreMessage message)
        {
            using (var ms = new PooledMemoryStream(_pipe.Service.ArrayPool))
            using (var w = new BinaryWriter(ms))
            {
                var @out = _protocol.Serialize(session, message);
                using (var ms2 = new PooledMemoryStream(_pipe.Service.ArrayPool))
                {
                    @out.Serialize(ms2);
                    var segment = ms2.ToSegment();

                    w.Write((ushort)43981);
                    w.Write((ushort)0);
                    w.Write(segment.Count);
                    w.Write(0);
                    w.Write(0);
                    w.Write(segment.Array, segment.Offset, segment.Count);

                    segment = ms.ToSegment();
                    await _socket.SendToTaskAsync(segment.Array, segment.Offset, segment.Count, SocketFlags.None, session.UdpEndPoint)
                        .ConfigureAwait(false);
                }
            }
        }

        public void Start()
        {
            DoUdpReceive();
        }

        public void Dispose()
        {
            if (_socket != null)
            {
                _socket.Dispose();
                _socket = null;
            }
        }

        private void DoUdpReceive()
        {
            EndPoint endPoint = new IPEndPoint(0, 0);
            _socket.BeginReceiveFrom(_buffer, 0, _buffer.Length, SocketFlags.None, ref endPoint, ProcessUdp, null);
        }

        private async void ProcessUdp(IAsyncResult ar)
        {
            try
            {
                if (_socket == null)
                    return;

                EndPoint endPoint = new IPEndPoint(0, 0);
                var bytesTransferred = _socket.EndReceiveFrom(ar, ref endPoint);
                var remoteEndPoint = (IPEndPoint)endPoint;

                if (bytesTransferred == 0)
                    return;

                using (var data = new PooledMemoryStream(_pipe.Service.ArrayPool))
                {
                    data.Write(_buffer, 0, bytesTransferred);
                    data.Position = 0;

                    ushort flag;
                    ushort sessionId;
                    int length;
                    int id;
                    int fragId;
                    using (var r = data.ToBinaryReader(true))
                    {
                        flag = r.ReadUInt16();
                        sessionId = r.ReadUInt16();
                        length = r.ReadInt32();
                        id = r.ReadInt32();
                        fragId = r.ReadInt32();
                    }

                    if (flag != 43981)
                    {
                        Logger.Warn()
                            .Message("Received Fragment instead of FullFragment")
                            .Write();
                        DoUdpReceive();
                        return;
                    }

                    var message = (CoreMessage)_protocol.Deserialize(null, data);
                    message.IsUdp = true;
                    message.RemoteEndPoint = remoteEndPoint;

                    Logger.Trace()
                        .Message("Client:{0} Received:{1}", remoteEndPoint.ToString(), message.GetType().Name)
                        .Write();

                    var holepunch = message as ServerHolepunchMessage;
                    if (holepunch != null)
                    {
                        var session = (ProudSession)_pipe.Service[holepunch.MagicNumber];
                        if (session == null)
                        {
                            Logger.Warn()
                                .Message("Client:{0} Invalid MagicNumber", remoteEndPoint.ToString())
                                .Write();
                            DoUdpReceive();
                            return;
                        }

                        Logger.Debug()
                            .Message("Client:{0} Received ServerHolepunch", session.HostId)
                            .Write();

                        if (session.UdpEnabled)
                        {
                            Logger.Warn()
                                .Message("Client:{0} UDP relaying is already enabled", session.HostId)
                                .Write();

                            DoUdpReceive();
                            return;
                        }

                        session.UdpSessionId = sessionId;
                        session.UdpEndPoint = remoteEndPoint;
                        Send(session, new ServerHolepunchAckMessage(holepunch.MagicNumber, remoteEndPoint));
                    }
                    else
                    {
                        // ToDo Add a lookup for UdpSessionId
                        var session = _pipe.Service.Sessions.Cast<ProudSession>().FirstOrDefault(s => s.UdpSessionId == sessionId);
                        if (session == null)
                        {
                            Logger.Warn()
                                .Message("Client:{0} Invalid session id", remoteEndPoint.ToString())
                                .Write();
                        }
                        else
                        {
                            try
                            {
                                var deferral = new DeferralManager();
                                _pipe.OnMessageReceived(new MessageReceivedEventArgs(session, message, deferral));
                                await deferral.WaitAsync().ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                _pipe.Service.Pipeline.OnError(new ExceptionEventArgs(session, ex));
                                session.Dispose();
                            }
                        }
                    }

                    DoUdpReceive();
                }
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                _pipe.Service.Pipeline.OnError(new ExceptionEventArgs(ex));
            }
        }
    }
}
