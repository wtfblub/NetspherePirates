using System;
using System.Net;
using System.Threading.Tasks;
using BlubLib.Threading.Tasks;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ProudNet.Serialization.Messages;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet
{
    public class ProudSession : IDisposable
    {
        private bool _disposed;

        public ISocketChannel Channel { get; }
        public bool IsConnected => Channel.Active;
        public IPEndPoint RemoteEndPoint => (IPEndPoint)Channel.RemoteAddress;
        public IPEndPoint LocalEndPoint => (IPEndPoint)Channel.LocalAddress;

        public uint HostId { get; }
        public P2PGroup P2PGroup { get; internal set; }
        public IPEndPoint UdpEndPoint { get; internal set; }
        public IPEndPoint UdpLocalEndPoint { get; internal set; }

        internal bool UdpEnabled { get; set; }
        internal ushort UdpSessionId { get; set; }
        internal Crypt Crypt { get; set; }
        internal DateTime LastSpeedHackDetectorPing { get; set; }
        internal AsyncManualResetEvent HandhsakeEvent { get; set; }
        internal Guid HolepunchMagicNumber { get; set; }
        internal UdpSocket UdpSocket { get; set; }

        public double UnreliablePing { get; internal set; }

        public ProudSession(uint hostId, IChannel channel)
        {
            HostId = hostId;
            Channel = (ISocketChannel)channel;
            HandhsakeEvent = new AsyncManualResetEvent();
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
            return _disposed ? Task.CompletedTask : Channel.Pipeline.Context("coreHandler").WriteAndFlushAsync(message);
        }

        internal Task SendUdpIfAvailableAsync(ICoreMessage message)
        {
            return UdpEnabled
                ? UdpSocket.SendAsync(message, UdpEndPoint)
                : SendAsync(message);
        }

        public Task CloseAsync()
        {
            if (_disposed)
                return Task.CompletedTask;

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
