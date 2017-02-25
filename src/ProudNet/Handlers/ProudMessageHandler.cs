using BlubLib;
using BlubLib.DotNetty.Handlers.MessageHandling;
using DotNetty.Transport.Channels;

namespace ProudNet.Handlers
{
    public class ProudMessageHandler : MessageHandler
    {
        protected override bool GetParameter<T>(IChannelHandlerContext context, object message, out T value)
        {
            if (typeof(ProudSession).IsAssignableFrom(typeof(T)))
            {
                var session = context.Channel.GetAttribute(ChannelAttributes.Session).Get();
                value = DynamicCast<T>.From(session);
                return true;
            }
            if (typeof(ProudServer).IsAssignableFrom(typeof(T)))
            {
                var server = context.Channel.GetAttribute(ChannelAttributes.Server).Get();
                value = DynamicCast<T>.From(server);
                return true;
            }
            return base.GetParameter(context, message, out value);
        }
    }
}
