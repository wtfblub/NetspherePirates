using System.Net;
using DotNetty.Transport.Channels;

namespace ProudNet
{
    public class MessageContext
    {
        public IChannelHandlerContext ChannelHandlerContext { get; set; }
        public ProudSession Session { get; set; }
        public object Message { get; set; }

        internal IPEndPoint UdpEndPoint { get; set; }
    }
}
