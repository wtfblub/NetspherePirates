using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using DotNetty.Transport.Channels;

namespace ProudNet
{
    internal class UdpSocketManager : IDisposable
    {
        private readonly ProudServer _server;
        private readonly List<UdpSocket> _sockets = new List<UdpSocket>();
        private int _counter;

        public bool IsRunning => Sockets.Count > 0;
        public IReadOnlyList<UdpSocket> Sockets => _sockets;

        public UdpSocketManager(ProudServer server)
        {
            _server = server;
        }

        public void Listen(IPAddress address, int[] ports, IEventLoopGroup eventLoopGroup)
        {
            if (IsRunning)
                throw new InvalidOperationException($"{nameof(UdpSocketManager)} is already running");

            foreach (var port in ports)
            {
                var socket = new UdpSocket(_server);
                socket.Listen(new IPEndPoint(address, port), eventLoopGroup);
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
