using BlubLib;
using BlubLib.DotNetty.Handlers.MessageHandling;
using DotNetty.Transport.Channels;

namespace ProudNet.Handlers
{
    public class ProudMessageHandler : MessageHandler
    {
        protected override Handler GetHandler(IChannelHandlerContext context, object message)
        {
            var recvContext = (RecvContext)message;
            return base.GetHandler(context, recvContext.Message);
        }

        protected override AsyncHandler GetAsyncHandler(IChannelHandlerContext context, object message)
        {
            var recvContext = (RecvContext)message;
            return base.GetAsyncHandler(context, recvContext.Message);
        }

        protected override object GetMessageObject(object message)
        {
            var context = (RecvContext)message;
            return context.Message;
        }

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

            if (typeof(RecvContext).IsAssignableFrom(typeof(T)))
            {
                value = DynamicCast<T>.From(message);
                return true;
            }

            return base.GetParameter(context, message, out value);
        }
    }
}
