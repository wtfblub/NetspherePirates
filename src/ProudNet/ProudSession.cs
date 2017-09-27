using System;
using System.Net;
using System.Threading.Tasks;
using BlubLib.Threading.Tasks;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ProudNet.Serialization.Messages;
using ProudNet.Serialization.Messages.Core;
using Serilog;

namespace ProudNet
{
    public class ProudSession : IDisposable
    {
        private bool _disposed;
        private IPEndPoint _udpEndPoint;
        private IPEndPoint _udpLocalEndPoint;

        public ProudServer Server { get; }
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
                Logger = Logger?.ForContext("UdpEndPoint", value?.ToString());
                _udpEndPoint = value;
            }
        }
        public IPEndPoint UdpLocalEndPoint
        {
            get => _udpLocalEndPoint;
            internal set
            {
                Logger = Logger?.ForContext("UdpLocalEndPoint", value?.ToString());
                _udpLocalEndPoint = value;
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

        public double UnreliablePing { get; internal set; }

        public ProudSession(uint hostId, IChannel channel, ProudServer server)
        {
            HostId = hostId;
            Server = server;
            Channel = (ISocketChannel)channel;
            HandhsakeEvent = new AsyncManualResetEvent();

            var remoteEndPoint = (IPEndPoint)Channel.RemoteAddress;
            RemoteEndPoint = new IPEndPoint(remoteEndPoint.Address.MapToIPv4(), remoteEndPoint.Port);

            var localEndPoint = (IPEndPoint)Channel.LocalAddress;
            LocalEndPoint = new IPEndPoint(localEndPoint.Address.MapToIPv4(), localEndPoint.Port);

            Logger = Server.Configuration.Logger?
                .ForContext("HostId", HostId)
                .ForContext("EndPoint", remoteEndPoint.ToString());
        }

        public Task SendAsync(object message)
        {
            Logger?.Verbose("Sending message {MessageType}", message.GetType().Name);
            return _disposed ? Task.CompletedTask : SendAsync(message, SendOptions.ReliableSecure);
        }

        public Task SendAsync(object message, SendOptions options)
        {
            Logger?.Verbose("Sending message {MessageType} using options={@Options}", message.GetType().Name, options);
            return _disposed ? Task.CompletedTask : Channel.WriteAndFlushAsync(new SendContext(message, options));
        }

        internal Task SendAsync(IMessage message)
        {
            Logger?.Verbose("Sending message {MessageType}", message.GetType().Name);
            return _disposed ? Task.CompletedTask : SendAsync(message, SendOptions.Reliable);
        }

        internal Task SendAsync(ICoreMessage message)
        {
            Logger?.Verbose("Sending core message {MessageType}", message.GetType().Name);
            return _disposed ? Task.CompletedTask : Channel.Pipeline.Context("coreHandler").WriteAndFlushAsync(message);
        }

        internal Task SendUdpIfAvailableAsync(ICoreMessage message)
        {
            var log = Logger?.ForContext("MessageType", message.GetType().Name);
            if (UdpEnabled)
            {
                log?.Verbose("Sending core message {MessageType} using udp");
                return UdpSocket.SendAsync(message, UdpEndPoint);
            }

            log?.Verbose("Sending core message {MessageType} using tcp");
            return SendAsync(message);
        }

        internal Task SendUdpAsync(ICoreMessage message)
        {
            Logger?.Verbose("Sending core message {MessageType} using udp", message.GetType().Name);
            return UdpSocket.SendAsync(message, UdpEndPoint);
        }

        public Task CloseAsync()
        {
            if (_disposed)
                return Task.CompletedTask;

            Logger?.Verbose("Closing");
            _disposed = true;

            Crypt?.Dispose();
            return Channel?.CloseAsync();
        }

        public void Dispose()
        {
            CloseAsync().WaitEx();
        }

        //public override async Task SendAsync(IMessage message)
        //{
        //    var coreMessage = message as CoreMessage;
        //    if (coreMessage != null)
        //    {
        //        if (UdpEnabled)
        //        {
        //            if (message is UnreliableRelay2Message ||
        //                message is PeerUdp_ServerHolepunchAckMessage ||
        //                message is UnreliablePongMessage)
        //            {

        //                await UdpSocket.SendAsync(this, coreMessage)
        //                    .ConfigureAwait(false);
        //                return;
        //            }
        //        }
        //        var pipe = Service.Pipeline.Get("proudnet_protocol");
        //        await pipe.OnSendMessage(new MessageEventArgs(this, message))
        //            .ConfigureAwait(false);
        //        return;
        //    }

        //    await base.SendAsync(message)
        //        .ConfigureAwait(false);
        //}

        //public override void Close()
        //{
        //    Send(new ShutdownTcpAckMessage());

        //    base.Close();

        //    if (EncryptContext != null)
        //    {
        //        EncryptContext.Dispose();
        //        EncryptContext = null;
        //    }

        //    ReadyEvent.Reset();
        //}
    }
}
