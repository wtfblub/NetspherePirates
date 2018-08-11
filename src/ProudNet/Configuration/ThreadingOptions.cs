using System;
using DotNetty.Transport.Channels;

namespace ProudNet.Configuration
{
    public class ThreadingOptions
    {
        public Func<IEventLoopGroup> SocketListenerThreadsFactory { get; set; }
        public Func<IEventLoopGroup> SocketWorkerThreadsFactory { get; set; }
        public Func<IEventLoop> WorkerThreadFactory { get; set; }
    }
}
