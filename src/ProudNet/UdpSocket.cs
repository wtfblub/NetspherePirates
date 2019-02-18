using System;
using System.Net;
using System.Threading.Tasks;
using BlubLib;
using BlubLib.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Options;
using ProudNet.Configuration;
using ProudNet.DotNetty.Codecs;
using ProudNet.DotNetty.Handlers;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet
{
    internal class UdpSocket : IDisposable
    {
        private readonly NetworkOptions _options;
        private readonly IServiceProvider _serviceProvider;

        private bool _disposed;
        private IEventLoopGroup _eventLoopGroup;

        public IChannel Channel { get; private set; }

        public UdpSocket(IOptions<NetworkOptions> options, IServiceProvider serviceProvider)
        {
            _options = options.Value;
            _serviceProvider = serviceProvider;
        }

        public void Listen(IPEndPoint endPoint, IEventLoopGroup eventLoopGroup)
        {
            ThrowIfDisposed();

            if (eventLoopGroup == null)
                throw new ArgumentNullException(nameof(eventLoopGroup));

            _eventLoopGroup = eventLoopGroup;

            try
            {
                Channel = new Bootstrap()
                    .Group(_eventLoopGroup ?? eventLoopGroup)
                    .Channel<SocketDatagramChannel>()
                    .Handler(new ActionChannelInitializer<IChannel>(ch =>
                    {
                        var udpHandler = _serviceProvider.GetService<UdpHandler>();
                        udpHandler.Socket = this;
                        ch.Pipeline
                            .AddLast(new UdpFrameDecoder((int)_options.MessageMaxLength))
                            .AddLast(new UdpFrameEncoder())
                            .AddLast(udpHandler)
                            .AddLast(_serviceProvider.GetService<ErrorHandler>());
                    }))
                    .BindAsync(endPoint).WaitEx();
            }
            catch (Exception ex)
            {
                _eventLoopGroup?.ShutdownGracefullyAsync(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10)).WaitEx();
                _eventLoopGroup = null;
                Channel = null;
                ex.Rethrow();
            }
        }

        public void Send(ICoreMessage message, IPEndPoint endPoint)
        {
            if (!_disposed && Channel.IsWritable)
                Channel.WriteAndFlushAsync(new SendContext { Message = message, UdpEndPoint = endPoint });
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            Channel.CloseAsync();
            _eventLoopGroup?.ShutdownGracefullyAsync(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10)).WaitEx();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
