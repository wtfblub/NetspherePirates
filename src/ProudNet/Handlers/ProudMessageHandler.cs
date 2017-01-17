using BlubLib;
using BlubLib.DotNetty.Handlers.MessageHandling;
using DotNetty.Transport.Channels;
using ProudNet.Server;

namespace ProudNet.Handlers
{
    public class ProudMessageHandler : MessageHandler
    {
        protected override bool GetParameter<T>(IChannelHandlerContext context, object message, out T value)
        {
            if (typeof(T) == typeof(ProudSession))
            {
                value = DynamicCast<T>.From(context.Channel.GetAttribute(ChannelAttributes.Session));
                return true;
            }
            if (typeof(ProudServer).IsAssignableFrom(typeof(T)))
            {
                value = DynamicCast<T>.From(context.Channel.GetAttribute(ChannelAttributes.Server));
                return true;
            }
            return base.GetParameter(context, message, out value);
        }
    }
}
