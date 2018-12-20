using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using BlubLib.Threading.Tasks;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Logging;
using ProudNet.Serialization.Messages;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet
{
    public class ProudSession : IP2PMemberInternal, IDisposable
    {
        private readonly object _disposeMutex = new object();
        private readonly ConcurrentDictionary<uint, P2PConnectionState> _connectionStates =
            new ConcurrentDictionary<uint, P2PConnectionState>();
        private volatile bool _disposed;
        private volatile bool _isDisposing;
        private IPEndPoint _udpEndPoint;
        private IPEndPoint _udpLocalEndPoint;

        public ISocketChannel Channel { get; }
        public bool IsConnected => Channel.Active;
        public IPEndPoint RemoteEndPoint { get; }
        public IPEndPoint LocalEndPoint { get; }

        public uint HostId { get; }
        public P2PGroup P2PGroup { get; internal set; }

        public IPEndPoint UdpEndPoint
        {
            get => _udpEndPoint;
            internal set
            {
                _udpEndPoint = value;
                SetLoggingScope();
            }
        }

        public IPEndPoint UdpLocalEndPoint
        {
            get => _udpLocalEndPoint;
            internal set
            {
                _udpLocalEndPoint = value;
                SetLoggingScope();
            }
        }

        internal ILogger Logger { get; private set; }
        internal bool UdpEnabled { get; set; }
        internal ushort UdpSessionId { get; set; }
        internal Crypt Crypt { get; set; }
        internal DateTime LastSpeedHackDetectorPing { get; set; }
        internal AsyncManualResetEvent HandhsakeEvent { get; set; }
        internal Guid HolepunchMagicNumber { get; set; }
        internal UdpSocket UdpSocket { get; set; }
        internal SessionState State { get; set; }

        Crypt IP2PMemberInternal.Crypt { get; set; }
        ConcurrentDictionary<uint, P2PConnectionState> IP2PMemberInternal.ConnectionStates => _connectionStates;

        public double UnreliablePing { get; internal set; }
        internal DateTimeOffset LastUdpPing { get; set; }

        public ProudSession(ILogger logger, uint hostId, IChannel channel)
        {
            HostId = hostId;
            Channel = (ISocketChannel)channel;
            HandhsakeEvent = new AsyncManualResetEvent();

            var remoteEndPoint = (IPEndPoint)Channel.RemoteAddress;
            RemoteEndPoint = new IPEndPoint(remoteEndPoint.Address.MapToIPv4(), remoteEndPoint.Port);

            var localEndPoint = (IPEndPoint)Channel.LocalAddress;
            LocalEndPoint = new IPEndPoint(localEndPoint.Address.MapToIPv4(), localEndPoint.Port);

            Logger = logger;
            SetLoggingScope();
        }

        public void Send(object message)
        {
            SendAsync(message, SendOptions.ReliableSecure);
        }

        public Task SendAsync(object message)
        {
            return _disposed ? Task.CompletedTask : SendAsync(message, SendOptions.ReliableSecure);
        }

        public Task SendAsync(object message, SendOptions options)
        {
            return _disposed ? Task.CompletedTask : Channel.WriteAndFlushAsync(new SendContext(message, options));
        }

        internal Task SendAsync(IMessage message)
        {
            return _disposed ? Task.CompletedTask : SendAsync(message, SendOptions.Reliable);
        }

        internal Task SendAsync(ICoreMessage message)
        {
            return _disposed
                ? Task.CompletedTask
                : Channel.Pipeline.Context(Constants.Pipeline.CoreMessageHandlerName).WriteAndFlushAsync(message);
        }

        internal Task SendUdpIfAvailableAsync(ICoreMessage message)
        {
            if (UdpEnabled)
            {
                return UdpSocket.SendAsync(message, UdpEndPoint);
            }

            return SendAsync(message);
        }

        internal Task SendUdpAsync(ICoreMessage message)
        {
            return UdpSocket.SendAsync(message, UdpEndPoint);
        }

        void IP2PMemberInternal.Send(ICoreMessage message, bool udp)
        {
            if (udp)
                SendUdpIfAvailableAsync(message);
            else
                SendAsync(message);
        }

        void IP2PMemberInternal.Send(IMessage message)
        {
            SendAsync(message);
        }

        private void SetLoggingScope()
        {
            Logger = Logger.ForContext(
                ("HostId", HostId),
                ("EndPoint", RemoteEndPoint?.ToString()),
                ("UdpEndPoint", UdpEndPoint?.ToString()),
                ("UdpLocalEndPoint", UdpLocalEndPoint?.ToString())
            );
        }

        protected virtual Task CloseInternalAsync()
        {
            return Task.CompletedTask;
        }

        public async Task CloseAsync()
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (_isDisposing || _disposed)
                return;

            lock (_disposeMutex)
            {
                if (_isDisposing || _disposed)
                    return;

                _isDisposing = true;
            }

            await CloseInternalAsync();
            Logger.Debug("Closing...");

            Crypt?.Dispose();

            lock (_disposeMutex)
            {
                _disposed = true;
                _isDisposing = false;
            }

            if (Channel != null)
                await Channel.CloseAsync();
        }

        public void Dispose()
        {
            CloseAsync().WaitEx();
        }
    }
}
