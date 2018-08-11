using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;

namespace ProudNet
{
    internal class UdpSocketManager : IDisposable
    {
        private readonly List<UdpSocket> _sockets = new List<UdpSocket>();
        private readonly IServiceProvider _serviceProvider;
        private int _counter;

        public bool IsRunning => Sockets.Count > 0;
        public IReadOnlyList<UdpSocket> Sockets => _sockets;
        public IPAddress Address { get; private set; }

        public UdpSocketManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Listen(IPAddress address, IPAddress listenerAddress, ushort[] ports, IEventLoopGroup eventLoopGroup)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            if (listenerAddress == null)
                throw new ArgumentNullException(nameof(listenerAddress));

            if (ports == null || ports.Length == 0)
                throw new ArgumentNullException(nameof(ports));

            if (eventLoopGroup == null)
                throw new ArgumentNullException(nameof(eventLoopGroup));

            if (IsRunning)
                throw new InvalidOperationException($"{nameof(UdpSocketManager)} is already running");

            Address = address;
            foreach (var port in ports)
            {
                var socket = _serviceProvider.GetService<UdpSocket>();
                socket.Listen(new IPEndPoint(listenerAddress, port), eventLoopGroup);
                _sockets.Add(socket);
            }
        }

        public UdpSocket NextSocket()
        {
            if (!IsRunning)
                throw new InvalidOperationException($"{nameof(UdpSocketManager)} is not running");

            var counter = Interlocked.Increment(ref _counter);
            return Sockets[counter % Sockets.Count];
        }

        public void Dispose()
        {
            foreach (var socket in _sockets)
                socket.Dispose();

            _sockets.Clear();
        }
    }
}
