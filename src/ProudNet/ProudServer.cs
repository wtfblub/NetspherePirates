using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using BlubLib;
using BlubLib.Collections.Concurrent;
using BlubLib.DotNetty.Handlers.MessageHandling;
using BlubLib.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ProudNet.Codecs;
using ProudNet.Handlers;
using ProudNet.Serialization.Messages;

namespace ProudNet
{
    public class ProudServer : IDisposable
    {
        private bool _disposed;
        // ReSharper disable once NotAccessedField.Local
        private IChannel _listenerChannel;
        private readonly IEventLoopGroup _socketListenerThreads;
        private readonly IEventLoopGroup _socketWorkerThreads;
        private readonly IEventLoop _workerThread;
        private readonly ConcurrentDictionary<uint, ProudSession> _sessions = new ConcurrentDictionary<uint, ProudSession>();

        public bool IsRunning { get; private set; }
        public IReadOnlyDictionary<uint, ProudSession> Sessions => _sessions;
        public P2PGroupManager P2PGroupManager { get; }
        public bool IsShuttingDown { get; private set; }

        internal Configuration Configuration { get; }
        internal RSACryptoServiceProvider Rsa { get; }
        internal ConcurrentDictionary<uint, ProudSession> SessionsByUdpId => _sessionsByUdpId;
        internal UdpSocketManager UdpSocketManager { get; }

        #region Events
        public event EventHandler Started;
        public event EventHandler Stopping;
        public event EventHandler Stopped;

        public event EventHandler<ProudSession> Connected;
        public event EventHandler<ProudSession> Disconnected;

        public event EventHandler<ErrorEventArgs> Error;

        protected virtual void OnStarted()
        {
            Started?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnStopping()
        {
            Stopping?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnStopped()
        {
            Stopped?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnConnected(ProudSession session)
        {
            Connected?.Invoke(this, session);
        }

        protected virtual void OnDisconnected(ProudSession session)
        {
            Disconnected?.Invoke(this, session);
        }

        protected virtual void OnError(ErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        internal void RaiseError(ErrorEventArgs e)
        {
            OnError(e);
        }
        #endregion

        public ProudServer(Configuration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (configuration.Version == null)
                throw new ArgumentNullException(nameof(configuration.Version));

            if (configuration.HostIdFactory == null)
                throw new ArgumentNullException(nameof(configuration.HostIdFactory));

            if (configuration.MessageFactories == null)
                throw new ArgumentNullException(nameof(configuration.MessageFactories));

            _socketListenerThreads = configuration.SocketListenerThreads ?? new MultithreadEventLoopGroup(1);
            _socketWorkerThreads = configuration.SocketWorkerThreads ?? new MultithreadEventLoopGroup();
            _workerThread = configuration.WorkerThread ?? new SingleThreadEventLoop();

            Configuration = configuration;
            Rsa = new RSACryptoServiceProvider(1024);
            P2PGroupManager = new P2PGroupManager(this);
            SessionsByUdpId = new ConcurrentDictionary<uint, ProudSession>();
            UdpSocketManager = new UdpSocketManager(this);
        }

        public void Listen(IPEndPoint tcpListener, IPAddress udpAddress = null, int[] udpListenerPorts = null)
        {
            ThrowIfDisposed();

            var log = Configuration.Logger?
                .ForContext("TcpEndPoint", tcpListener)
                .ForContext("UdpAddress", udpAddress)
                .ForContext("UdpPorts", udpListenerPorts);
            log?.Information("Starting - tcp={TcpEndPoint} udp={UdpAddress} udp-port={UdpPorts}");

            try
            {
                _listenerChannel = new ServerBootstrap()
                    .Group(_socketListenerThreads, _socketWorkerThreads)
                    .Channel<TcpServerSocketChannel>()
                    .Handler(new ActionChannelInitializer<IServerSocketChannel>(ch => { }))
                    .ChildHandler(new ActionChannelInitializer<ISocketChannel>(ch =>
                    {
                        var userMessageHandler = new SimpleMessageHandler();
                        foreach (var handler in Configuration.MessageHandlers)
                            userMessageHandler.Add(handler);

                        ch.Pipeline
                            .AddLast(new SessionHandler(this))

                            .AddLast(new ProudFrameDecoder((int)Configuration.MessageMaxLength))
                            .AddLast(new ProudFrameEncoder())

                            .AddLast(new RecvContextDecoder())
                            .AddLast(new CoreMessageDecoder())
                            .AddLast(new CoreMessageEncoder())

                            .AddLast("coreHandler", new SimpleMessageHandler()
                                .Add(new CoreHandler(this)))

                            .AddLast(new SendContextEncoder())
                            .AddLast(new MessageDecoder(Configuration.MessageFactories))
                            .AddLast(new MessageEncoder(Configuration.MessageFactories))

                            // SimpleMessageHandler discards all handled messages
                            // So internal messages(if handled) wont reach the user messagehandler
                            .AddLast(new SimpleMessageHandler()
                                .Add(new ServerHandler()))

                            .AddLast(userMessageHandler)
                            .AddLast(new ErrorHandler(this));
                    }))
                    .ChildOption(ChannelOption.TcpNodelay, !Configuration.EnableNagleAlgorithm)
                    .ChildAttribute(ChannelAttributes.Session, default(ProudSession))
                    .ChildAttribute(ChannelAttributes.Server, this)
                    .BindAsync(tcpListener).WaitEx();

                if (udpListenerPorts != null)
                    UdpSocketManager.Listen(udpAddress, tcpListener.Address, udpListenerPorts, _socketWorkerThreads);
            }
            catch (Exception ex)
            {
                log?.Error(ex, "Unable to start server - tcp={TcpEndPoint} udp={UdpAddress} udp-port={UdpPorts}");
                ShutdownThreads();
                ex.Rethrow();
            }

            IsRunning = true;
            OnStarted();
            RetryUdpOrHolepunchIfRequired(this, null);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            Configuration.Logger?.Information("Shutting down...");
            _disposed = true;
            IsShuttingDown = true;
            OnStopping();

            UdpSocketManager.Dispose();
            ShutdownThreads();
            Rsa.Dispose();

            IsShuttingDown = false;
            IsRunning = false;
            OnStopped();
        }

        public void Broadcast(object message)
        {
            foreach (var session in Sessions.Values)
                session.SendAsync(message);
        }

        #region EventLoop tasks
        public void Execute(Action action)
        {
            ThrowIfDisposed();
            _workerThread.Execute(action);
        }

        public void Execute(Action<object, object> action, object context, object state)
        {
            ThrowIfDisposed();
            _workerThread.Execute(action, context, state);
        }

        public Task ScheduleAsync(Action action, TimeSpan delay)
        {
            ThrowIfDisposed();
            return _workerThread.ScheduleAsync(action, delay);
        }

        public Task ScheduleAsync(Action<object, object> action, object context, object state,
            TimeSpan delay)
        {
            ThrowIfDisposed();
            return _workerThread.ScheduleAsync(action, context, state, delay);
        }

        public Task ScheduleAsync(Action<object, object> action, object context, object state,
            TimeSpan delay, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return _workerThread.ScheduleAsync(action, context, state, delay, cancellationToken);
        }

        public Task<T> SubmitAsync<T>(Func<T> func)
        {
            ThrowIfDisposed();
            return _workerThread.SubmitAsync(func);
        }

        public Task<T> SubmitAsync<T>(Func<T> func, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return _workerThread.SubmitAsync(func, cancellationToken);
        }

        public Task<T> SubmitAsync<T>(Func<object, T> func, object state)
        {
            ThrowIfDisposed();
            return _workerThread.SubmitAsync(func, state);
        }

        public Task<T> SubmitAsync<T>(Func<object, T> func, object state, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return _workerThread.SubmitAsync(func, state, cancellationToken);
        }

        public Task<T> SubmitAsync<T>(Func<object, object, T> func, object context, object state)
        {
            ThrowIfDisposed();
            return _workerThread.SubmitAsync(func, context, state);
        }

        public Task<T> SubmitAsync<T>(Func<object, object, T> func, object context, object state, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return _workerThread.SubmitAsync(func, context, state, cancellationToken);
        }
        #endregion

        internal void AddSession(ProudSession session)
        {
            Configuration.Logger?.Debug("Adding new session {HostId}", session.HostId);
            _sessions[session.HostId] = session;
            OnConnected(session);
        }

        internal void RemoveSession(ProudSession session)
        {
            Configuration.Logger?.Debug("Removing session {HostId}", session.HostId);
            _sessions.Remove(session.HostId);
            SessionsByUdpId.Remove(session.UdpSessionId);
            OnDisconnected(session);
        }

        private static void RetryUdpOrHolepunchIfRequired(object context, object _)
        {
            var server = (ProudServer)context;
            if (!server.UdpSocketManager.IsRunning || server.IsShuttingDown || !server.IsRunning)
                return;

            server.Configuration.Logger?.Debug("RetryUdpOrHolepunchIfRequired");

            foreach (var group in server.P2PGroupManager.Values)
            {
                var now = DateTimeOffset.Now;
                foreach (var member in group.Members.Values)
                {
                    // Retry udp relay
                    if (member.Session.UdpSocket != null)
                    {
                        var diff = now - member.Session.LastUdpPing;
                        if (!member.Session.UdpEnabled)
                        {
                            member.Session.Logger?.Information("Trying to switch to udp relay");
                            var socket = server.UdpSocketManager.NextSocket();
                            member.Session.UdpSocket = socket;
                            member.Session.HolepunchMagicNumber = Guid.NewGuid();
                            member.SendAsync(new S2C_RequestCreateUdpSocketMessage(new IPEndPoint(server.UdpSocketManager.Address, ((IPEndPoint)socket.Channel.LocalAddress).Port)));
                        }
                        else if (diff >= server.Configuration.PingTimeout)
                        {
                            member.Session.Logger?.Information("Fallback to tcp relay by server");
                            //member.Session.UdpEnabled = false;
                            //server.SessionsByUdpId.Remove(member.Session.UdpSessionId);
                            member.SendAsync(new NotifyUdpToTcpFallbackByServerMessage());
                        }
                    }

                    // Retry p2p holepunch
                    foreach (var stateA in member.ConnectionStates.Values)
                    {
                        var stateB = stateA.RemotePeer.ConnectionStates.GetValueOrDefault(member.HostId);
                        if (!stateA.RemotePeer.Session.UdpEnabled || !stateB.RemotePeer.Session.UdpEnabled)
                            continue;

                        if (stateA.IsInitialized)
                        {
                            var diff = now - stateA.LastHolepunch;
                            if (!stateA.HolepunchSuccess && diff >= server.Configuration.HolepunchTimeout)
                            {
                                member.Session.Logger?.Information("Trying to reconnect P2P to {TargetHostId}", stateA.RemotePeer.HostId);
                                stateA.RemotePeer.Session.Logger?.Information("Trying to reconnect P2P to {TargetHostId}", member.HostId);
                                stateA.JitTriggered = stateB.JitTriggered = false;
                                stateA.PeerUdpHolepunchSuccess = stateB.PeerUdpHolepunchSuccess = false;
                                stateA.LastHolepunch = stateB.LastHolepunch = now;
                                member.SendAsync(new RenewP2PConnectionStateMessage(stateA.RemotePeer.HostId));
                                stateA.RemotePeer.SendAsync(new RenewP2PConnectionStateMessage(member.HostId));
                                //member.SendAsync(new P2PRecycleCompleteMessage(stateA.RemotePeer.HostId));
                                //stateA.RemotePeer.SendAsync(new P2PRecycleCompleteMessage(member.HostId));
                            }
                        }
                        else
                        {
                            member.Session.Logger?.Debug("Initialize P2P with {TargetHostId}", stateA.RemotePeer.HostId);
                            stateA.RemotePeer.Session.Logger?.Debug("Initialize P2P with {TargetHostId}", member.HostId);
                            stateA.LastHolepunch = stateB.LastHolepunch = DateTimeOffset.Now;
                            stateA.IsInitialized = stateB.IsInitialized = true;
                            member.SendAsync(new P2PRecycleCompleteMessage(stateA.RemotePeer.HostId));
                            stateA.RemotePeer.SendAsync(new P2PRecycleCompleteMessage(member.HostId));
                        }
                    }
                }
            }

            if (!server.IsShuttingDown && server.IsRunning)
            {
                var __ = server.ScheduleAsync(RetryUdpOrHolepunchIfRequired, server, null, TimeSpan.FromSeconds(10));
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        private void ShutdownThreads()
        {
            _socketListenerThreads.ShutdownGracefullyAsync(TimeSpan.Zero, TimeSpan.Zero).WaitEx();
            _socketWorkerThreads.ShutdownGracefullyAsync(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5)).WaitEx();
            _workerThread.ShutdownGracefullyAsync(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5)).WaitEx();
        }
    }
}
