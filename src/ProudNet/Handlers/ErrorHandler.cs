using System;
using System.Net.Sockets;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using ProudNet.Hosting.Services;

namespace ProudNet.Handlers
{
    internal class ErrorHandler : ChannelHandlerAdapter
    {
        private readonly ILogger _log;
        private readonly ProudNetServerService _server;

        public ErrorHandler(ILogger<ErrorHandler> logger, IProudNetServerService server)
        {
            _log = logger;
            _server = (ProudNetServerService)server; // TODO This is bad and should be changed
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            if (exception is SocketException socketException)
            {
                if (socketException.SocketErrorCode == SocketError.ConnectionReset)
                    return;
            }

            _log.LogError(exception, "Unhandled exception");
            var session = context.Channel.GetAttribute(ChannelAttributes.Session).Get();
            _server.RaiseError(new ErrorEventArgs(session, exception));
            session?.CloseAsync();
        }
    }
}
