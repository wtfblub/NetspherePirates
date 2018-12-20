using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BlubLib;
using BlubLib.Collections.Concurrent;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Flow;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProudNet.Configuration;
using ProudNet.DotNetty.Codecs;
using ProudNet.DotNetty.Handlers;
using ProudNet.Handlers;
using ProudNet.Serialization.Messages;
using ProudNet.Serialization.Messages.Core;
using MessageHandler = ProudNet.DotNetty.Handlers.MessageHandler;

namespace ProudNet.Hosting.Services
{
    internal class ProudNetServerService : IHostedService, IProudNetServerService
    {
        private readonly ILogger _log;
        private readonly IServiceProvider _serviceProvider;
        private readonly NetworkOptions _networkOptions;
        private readonly ThreadingOptions _threadingOptions;
        private readonly P2PGroupManager _groupManager;
        private readonly UdpSocketManager _udpSocketManager;
        private readonly ISchedulerService _schedulerService;
        private readonly IInternalSessionManager<Guid> _magicNumberSessionManager;
        private readonly IInternalSessionManager<uint> _udpSessionManager;
        private IEventLoopGroup _listenerThreads;
        private IEventLoopGroup _workerThreads;

        public bool IsRunning { get; private set; }

        public bool IsShuttingDown { get; private set; }

        #region Events
        public event EventHandler Started;

        public event EventHandler Stopping;

        public event EventHandler Stopped;

        public event EventHandler<ProudSession> Connected;

        public event EventHandler<ProudSession> Disconnected;

        public event EventHandler<ErrorEventArgs> Error;

        public event EventHandler<UnhandledRmiEventArgs> UnhandledRmi;

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
            var log = e.Session?.Logger ?? _log;
            log.LogError(e.Exception, "Unhandled server exception");
            Error?.Invoke(this, e);
        }

        internal void RaiseError(ErrorEventArgs e)
        {
            OnError(e);
        }

        protected virtual void OnUnhandledRmi(MessageContext context)
        {
            UnhandledRmi?.Invoke(this, new UnhandledRmiEventArgs(context.Session, context.Message));
        }
        #endregion

        public ProudNetServerService(ILogger<ProudNetServerService> logger, ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider,
            IOptions<NetworkOptions> networkOptions, IOptions<ThreadingOptions> threadingOptions,
            P2PGroupManager groupManager, UdpSocketManager udpSocketManager, ISchedulerService schedulerService,
            ISessionManagerFactory sessionManagerFactory)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _log = logger;
            _serviceProvider = serviceProvider;
            _networkOptions = networkOptions.Value;
            _threadingOptions = threadingOptions.Value;
            _groupManager = groupManager;
            _udpSocketManager = udpSocketManager;
            _schedulerService = schedulerService;
            _magicNumberSessionManager = sessionManagerFactory.GetSessionManager<Guid>(SessionManagerType.MagicNumber);
            _udpSessionManager = sessionManagerFactory.GetSessionManager<uint>(SessionManagerType.UdpId);
            InternalLoggerFactory.DefaultFactory = loggerFactory;

            var sessionManager = _serviceProvider.GetRequiredService<ISessionManager>();
            sessionManager.Added += SessionManager_OnAdded;
            sessionManager.Removed += SessionManager_OnRemoved;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (IsRunning)
                throw new InvalidOperationException("Server is already running");

            var tcpListener = _networkOptions.TcpListener;
            var udpAddress = _networkOptions.UdpAddress;
            var udpListenerPorts = _networkOptions.UdpListenerPorts;
            _log.LogInformation("Starting - tcp={TcpEndPoint} udp={UdpAddress} udp-port={UdpPorts}",
                tcpListener, udpAddress, udpListenerPorts);

            try
            {
                _listenerThreads = _threadingOptions.SocketListenerThreadsFactory();
                _workerThreads = _threadingOptions.SocketWorkerThreadsFactory();

                await new ServerBootstrap()
                    .Group(_listenerThreads, _workerThreads)
                    .Channel<TcpServerSocketChannel>()
                    .Handler(new ActionChannelInitializer<IServerSocketChannel>(ch => { }))
                    .ChildHandler(new ActionChannelInitializer<ISocketChannel>(ch =>
                    {
                        var coreMessageHandler = new MessageHandler(_serviceProvider,
                            new DefaultMessageHandlerResolver(
                                new[] { typeof(AuthenticationHandler).Assembly }, typeof(ICoreMessage)));

                        var internalRmiMessageHandler = new MessageHandler(_serviceProvider,
                            new DefaultMessageHandlerResolver(new[] { typeof(ReliablePingMessage).Assembly }, typeof(IMessage)));

                        var rmiMessageHandler = new MessageHandler(_serviceProvider,
                            _serviceProvider.GetRequiredService<IMessageHandlerResolver>());

                        coreMessageHandler.UnhandledMessage +=
                            ctx => ctx.Session.Logger.LogDebug("Unhandled core message={@Message}", ctx.Message);

                        internalRmiMessageHandler.UnhandledMessage +=
                            ctx => ctx.Session.Logger.LogDebug("Unhandled internal rmi message={@Message}", ctx.Message);

                        rmiMessageHandler.UnhandledMessage += OnUnhandledRmi;

                        ch.Pipeline
                            .AddLast(_serviceProvider.GetRequiredService<SessionHandler>())
                            .AddLast(_serviceProvider.GetRequiredService<ProudFrameDecoder>())
                            .AddLast(_serviceProvider.GetRequiredService<ProudFrameEncoder>())
                            .AddLast(_serviceProvider.GetRequiredService<MessageContextDecoder>())
                            .AddLast(_serviceProvider.GetRequiredService<CoreMessageDecoder>())
                            .AddLast(_serviceProvider.GetRequiredService<CoreMessageEncoder>())
                            .AddLast(new FlowControlHandler(false))
                            .AddLast(Constants.Pipeline.CoreMessageHandlerName, coreMessageHandler)
                            .AddLast(_serviceProvider.GetRequiredService<SendContextEncoder>())
                            .AddLast(_serviceProvider.GetRequiredService<MessageDecoder>())
                            .AddLast(_serviceProvider.GetRequiredService<MessageEncoder>())

                            // MessageHandler discards the message after handling
                            // so internal messages wont reach the rmiMessageHandler
                            .AddLast(Constants.Pipeline.InternalMessageHandlerName, internalRmiMessageHandler)
                            .AddLast(_serviceProvider.GetRequiredService<MessageDecoder>())
                            .AddLast(rmiMessageHandler)
                            .AddLast(_serviceProvider.GetRequiredService<ErrorHandler>());
                    }))
                    .ChildOption(ChannelOption.TcpNodelay, !_networkOptions.EnableNagleAlgorithm)
                    .ChildOption(ChannelOption.AutoRead, false)
                    .ChildAttribute(ChannelAttributes.ServiceProvider, _serviceProvider)
                    .ChildAttribute(ChannelAttributes.Session, default(ProudSession))
                    .BindAsync(_networkOptions.TcpListener);

                if (udpListenerPorts != null)
                    _udpSocketManager.Listen(udpAddress, tcpListener.Address, udpListenerPorts, _workerThreads);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Unable to start server - tcp={TcpEndPoint} udp={UdpAddress} udp-port={UdpPorts}",
                    tcpListener, udpAddress, udpListenerPorts);
                await ShutdownThreads();
                ex.Rethrow();
            }

            IsRunning = true;
            OnStarted();
            ScheduleRetryUdpOrHolepunchIfRequired();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (IsShuttingDown || !IsRunning)
                throw new InvalidOperationException("Server is already stopped");

            _log.LogInformation("Shutting down...");
            IsShuttingDown = true;
            OnStopping();
            _udpSocketManager.Dispose();
            await ShutdownThreads();

            IsRunning = false;
            IsShuttingDown = false;
            OnStopped();
        }

        private void SessionManager_OnAdded(object sender, SessionEventArgs e)
        {
            OnConnected(e.Session);
        }

        private void SessionManager_OnRemoved(object sender, SessionEventArgs e)
        {
            _udpSessionManager.RemoveSession(e.Session.UdpSessionId);
            _magicNumberSessionManager.RemoveSession(e.Session.HolepunchMagicNumber);
            OnDisconnected(e.Session);
        }

        private void ScheduleRetryUdpOrHolepunchIfRequired()
        {
            void RetryUdpOrHolepunchIfRequired(object context, object state)
            {
                var server = (ProudNetServerService)context;
                try
                {
                    var log = server._log;
                    var groupManager = server._groupManager;
                    var udpSocketManager = server._udpSocketManager;
                    var configuration = server._networkOptions;
                    if (!udpSocketManager.IsRunning || server.IsShuttingDown || !server.IsRunning)
                        return;

                    log.LogDebug("RetryUdpOrHolepunchIfRequired");
                    foreach (var group in groupManager.Values)
                    {
                        var now = DateTimeOffset.Now;

                        // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                        foreach (IP2PMemberInternal member in group)
                        {
                            if (!(member is ProudSession session))
                                continue;

                            // Retry udp relay
                            if (session.UdpSocket != null)
                            {
                                var diff = now - session.LastUdpPing;
                                if (!session.UdpEnabled)
                                {
                                    _magicNumberSessionManager.RemoveSession(session.HolepunchMagicNumber);

                                    session.Logger.LogInformation("Trying to switch to udp relay");
                                    var socket = udpSocketManager.NextSocket();
                                    session.UdpSocket = socket;
                                    session.HolepunchMagicNumber = Guid.NewGuid();
                                    _magicNumberSessionManager.AddSession(session.HolepunchMagicNumber, session);
                                    member.Send(new S2C_RequestCreateUdpSocketMessage(
                                        new IPEndPoint(udpSocketManager.Address,
                                            ((IPEndPoint)socket.Channel.LocalAddress).Port)));
                                }
                                else if (diff >= configuration.PingTimeout)
                                {
                                    session.Logger.LogInformation("Fallback to tcp relay by server");

                                    //member.Session.UdpEnabled = false;
                                    //server.SessionsByUdpId.Remove(member.Session.UdpSessionId);
                                    member.Send(new NotifyUdpToTcpFallbackByServerMessage());
                                }
                            }

                            // Skip p2p stuff when not enabled
                            if (!group.AllowDirectP2P)
                                continue;

                            // Retry p2p holepunch
                            foreach (var stateA in member.ConnectionStates.Values)
                            {
                                var stateB = stateA.RemotePeer.ConnectionStates.GetValueOrDefault(member.HostId);
                                if (!(stateA.RemotePeer is ProudSession sessionA) || !sessionA.UdpEnabled ||
                                    !(stateB.RemotePeer is ProudSession sessionB) || !sessionB.UdpEnabled)
                                    continue;

                                using (stateA.Mutex.Lock())
                                {
                                    if (stateA.IsInitialized)
                                    {
                                        var diff = now - stateA.LastHolepunch;
                                        if (!stateA.HolepunchSuccess && diff >= configuration.HolepunchTimeout)
                                        {
                                            session.Logger?.LogInformation("Trying to reconnect P2P to {TargetHostId}",
                                                stateA.RemotePeer.HostId);
                                            sessionA.Logger?.LogInformation("Trying to reconnect P2P to {TargetHostId}",
                                                member.HostId);
                                            stateA.JitTriggered = stateB.JitTriggered = false;
                                            stateA.PeerUdpHolepunchSuccess = stateB.PeerUdpHolepunchSuccess = false;
                                            stateA.LastHolepunch = stateB.LastHolepunch = now;
                                            member.Send(new RenewP2PConnectionStateMessage(stateA.RemotePeer.HostId));
                                            stateA.RemotePeer.Send(new RenewP2PConnectionStateMessage(member.HostId));
                                        }
                                    }
                                    else
                                    {
                                        session.Logger.LogDebug("Initialize P2P with {TargetHostId}", stateA.RemotePeer.HostId);
                                        sessionA.Logger.LogDebug("Initialize P2P with {TargetHostId}", member.HostId);
                                        stateA.LastHolepunch = stateB.LastHolepunch = DateTimeOffset.Now;
                                        stateA.IsInitialized = stateB.IsInitialized = true;
                                        member.Send(new P2PRecycleCompleteMessage(stateA.RemotePeer.HostId));
                                        stateA.RemotePeer.Send(new P2PRecycleCompleteMessage(member.HostId));
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "RetryUdpOrHolepunchIfRequired exception");
                }
                finally
                {
                    server.ScheduleRetryUdpOrHolepunchIfRequired();
                }
            }

            if (!IsShuttingDown && IsRunning)
                _schedulerService.ScheduleAsync(RetryUdpOrHolepunchIfRequired, this, null, TimeSpan.FromSeconds(5));
        }

        private Task ShutdownThreads()
        {
            return Task.WhenAll(_listenerThreads?.ShutdownGracefullyAsync(TimeSpan.Zero, TimeSpan.Zero) ?? Task.CompletedTask,
                _workerThreads?.ShutdownGracefullyAsync(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5)) ?? Task.CompletedTask);
        }
    }
}
