using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ProudNet.Codecs;
using ProudNet.Handlers;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet
{
    internal class UdpSocket : IDisposable
    {
        private bool _disposed;
        private IEventLoopGroup _eventLoopGroup;
        private readonly ProudServer _owner;

        public IChannel Channel { get; private set; }

        public UdpSocket(ProudServer owner)
        {
            _owner = owner;
        }

        public void Listen(IPEndPoint endPoint, IEventLoopGroup eventLoopGroup)
        {
            ThrowIfDisposed();

            _eventLoopGroup = eventLoopGroup != null
                ? null
                : new MultithreadEventLoopGroup();
            try
            {
                Channel = new Bootstrap()
                    .Group(_eventLoopGroup ?? eventLoopGroup)
                    .Channel<SocketDatagramChannel>()
                    .Handler(new ActionChannelInitializer<IChannel>(ch =>
                    {
                        ch.Pipeline
                            .AddLast(new UdpFrameDecoder((int)_owner.Configuration.MessageMaxLength))
                            .AddLast(new UdpFrameEncoder())
                            .AddLast(new UdpHandler(this, _owner));
                    }))
                    .BindAsync(endPoint).WaitEx();
            }
            catch (Exception ex)
            {
                _eventLoopGroup?.ShutdownGracefullyAsync();
                _eventLoopGroup = null;
                Channel = null;
                ex.Rethrow();
            }
        }

        public Task SendAsync(ICoreMessage message, IPEndPoint endPoint)
        {
            return Channel.WriteAndFlushAsync(new SendContext { Message = message, UdpEndPoint = endPoint });
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            Channel.CloseAsync().WaitEx();
            _eventLoopGroup?.ShutdownGracefullyAsync().WaitEx();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
