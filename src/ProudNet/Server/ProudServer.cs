using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ProudNet.Codecs;
using ProudNet.Handlers;
using ProudNet.Server.Handlers;

namespace ProudNet.Server
{
    public class ProudServer : IDisposable
    {
        private bool _disposed;
        private IEventLoopGroup _eventLoopGroup;
        private IChannel _listenerChannel;
        private readonly ConcurrentDictionary<uint, ProudSession> _sessions = new ConcurrentDictionary<uint, ProudSession>();

        public bool IsRunning { get; private set; }
        internal Configuration Configuration { get; }
        public IReadOnlyDictionary<uint, ProudSession> Sessions => _sessions;

        public event EventHandler Started;
        public event EventHandler Stopping;
        public event EventHandler Stopped;

        public event EventHandler<ProudSession> Connected;
        public event EventHandler<ProudSession> Disconnected;

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

        public ProudServer(Configuration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (configuration.Version == null)
                throw new ArgumentNullException(nameof(configuration.Version));

            if (configuration.HostIdFactory == null)
                throw new ArgumentNullException(nameof(configuration.HostIdFactory));

            if (configuration.MessageFactory == null)
                throw new ArgumentNullException(nameof(configuration.MessageFactory));

            Configuration = configuration;
        }

        public void Listen(IPEndPoint tcpListener, int[] udpListenerPorts = null)
        {
            ThrowIfDisposed();

            // TODO UDP listener

            _eventLoopGroup = new MultithreadEventLoopGroup();
            try
            {
                _listenerChannel = new ServerBootstrap()
                    .Group(_eventLoopGroup)
                    .Channel<TcpServerSocketChannel>()
                    .Handler(new ActionChannelInitializer<ISocketChannel>(ch => { }))
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

                            .AddLast(new SimpleMessageHandler()
                                .Add(new CoreHandler())
                                .Add(new CoreServerHandler(this)))

                            .AddLast(new MessageDecoder(Configuration.MessageFactory))
                            .AddLast(new MessageEncoder(Configuration.MessageFactory))

                            // SimpleMessageHandler discards all handled messages
                            // So internal messages(if handled) wont reach the user messagehandler
                            .AddLast(new SimpleMessageHandler()
                                .Add(new ServerHandler()))

                            .AddLast(userMessageHandler);
                    }))
                    .ChildOption(ChannelOption.TcpNodelay, !Configuration.EnableNagleAlgorithm)
                    .ChildAttribute(ChannelAttributes.Session, default(ProudSession))
                    .ChildAttribute(ChannelAttributes.Server, this)
                    .BindAsync(tcpListener).Result;
            }
            catch (Exception ex)
            {
                _eventLoopGroup.ShutdownGracefullyAsync();
                _eventLoopGroup = null;
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
            _listenerChannel.CloseAsync().WaitEx();
            _eventLoopGroup.ShutdownGracefullyAsync().WaitEx();
            OnStopped();
        }

        internal void AddSession(ProudSession session)
        {
            _sessions[session.HostId] = session;
            OnConnected(session);
        }

        internal void RemoveSession(ProudSession session)
        {
            _sessions.Remove(session.HostId);
            OnDisconnected(session);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
