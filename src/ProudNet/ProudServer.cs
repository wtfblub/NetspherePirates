using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ProudNet.Codecs;
using ProudNet.Handlers;

namespace ProudNet
{
    public class ProudServer : IDisposable
    {
        private bool _disposed;
        private IEventLoopGroup _listenerEventLoopGroup;
        private IEventLoopGroup _workerEventLoopGroup;
        private IChannel _listenerChannel;
        private readonly ConcurrentDictionary<uint, ProudSession> _sessions = new ConcurrentDictionary<uint, ProudSession>();
        private readonly ConcurrentDictionary<uint, ProudSession> _sessionsByUdpId = new ConcurrentDictionary<uint, ProudSession>();

        public bool IsRunning { get; private set; }
        public IReadOnlyDictionary<uint, ProudSession> Sessions => _sessions;
        public P2PGroupManager P2PGroupManager { get; }

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

            Configuration = configuration;
            Rsa = new RSACryptoServiceProvider(1024);
            P2PGroupManager = new P2PGroupManager(this);
            UdpSocketManager = new UdpSocketManager(this);
        }

        public void Listen(IPEndPoint tcpListener, IPAddress udpAddress = null, int[] udpListenerPorts = null, IEventLoopGroup listenerEventLoopGroup = null, IEventLoopGroup workerEventLoopGroup = null)
        {
            ThrowIfDisposed();
            
            _listenerEventLoopGroup = listenerEventLoopGroup ?? new MultithreadEventLoopGroup(1);
            _workerEventLoopGroup = workerEventLoopGroup ?? new MultithreadEventLoopGroup();
            try
            {
                _listenerChannel = new ServerBootstrap()
                    .Group(_listenerEventLoopGroup, _workerEventLoopGroup)
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

                            .AddLast(userMessageHandler);
                    }))
                    .ChildOption(ChannelOption.TcpNodelay, !Configuration.EnableNagleAlgorithm)
                    .ChildAttribute(ChannelAttributes.Session, default(ProudSession))
                    .ChildAttribute(ChannelAttributes.Server, this)
                    .BindAsync(tcpListener).WaitEx();

                if (udpListenerPorts != null)
                    UdpSocketManager.Listen(udpAddress, tcpListener.Address, udpListenerPorts, _workerEventLoopGroup);
            }
            catch (Exception ex)
            {
                _listenerEventLoopGroup.ShutdownGracefullyAsync();
                _listenerEventLoopGroup = null;
                _workerEventLoopGroup.ShutdownGracefullyAsync();
                _workerEventLoopGroup = null;
                ex.Rethrow();
            }

            IsRunning = true;
            OnStarted();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            OnStopping();
            UdpSocketManager.Dispose();
            _listenerChannel.CloseAsync().WaitEx();
            _listenerEventLoopGroup.ShutdownGracefullyAsync().WaitEx();
            _workerEventLoopGroup.ShutdownGracefullyAsync().WaitEx();
            Rsa.Dispose();
            OnStopped();
        }

        public void Broadcast(object message)
        {
            foreach (var session in Sessions.Values)
                session.SendAsync(message);
        }

        internal void AddSession(ProudSession session)
        {
            _sessions[session.HostId] = session;
            OnConnected(session);
        }

        internal void RemoveSession(ProudSession session)
        {
            _sessions.Remove(session.HostId);
            SessionsByUdpId.Remove(session.UdpSessionId);
            OnDisconnected(session);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
