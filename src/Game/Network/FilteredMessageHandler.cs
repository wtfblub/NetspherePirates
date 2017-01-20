using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using NLog;
using ProudNet;
using ProudNet.Handlers;

namespace Netsphere.Network
{
    internal class FilteredMessageHandler<TSession> : ProudMessageHandler
        where TSession : ProudSession
    {
        // ReSharper disable once StaticMemberInGenericType
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDictionary<Type, List<Predicate<TSession>>> _filter = new Dictionary<Type, List<Predicate<TSession>>>();
        private readonly IList<IMessageHandler> _messageHandlers = new List<IMessageHandler>();

        public override async Task<bool> OnMessageReceived(IChannelHandlerContext context, object message)
        {
            List<Predicate<TSession>> predicates;
            _filter.TryGetValue(message.GetType(), out predicates);

            TSession session;
            if (!GetParameter(context, message, out session))
                throw new Exception("Unable to retrieve session");

            if (predicates != null && predicates.Any(predicate => !predicate(session)))
            {
                Logger.Debug($"Dropping message {message.GetType().Name} from client {((ISocketChannel)context.Channel).RemoteAddress}");
                return false;
            }

            var handled = false;
            foreach (var messageHandler in _messageHandlers)
            {
                var result = await messageHandler.OnMessageReceived(context, message)
                    .ConfigureAwait(false);
                if (result)
                    handled = true;
            }
            return handled;
        }

        public FilteredMessageHandler<TSession> AddHandler(IMessageHandler handler)
        {
            _messageHandlers.Add(handler);
            return this;
        }

        public FilteredMessageHandler<TSession> RegisterRule<T>(params Predicate<TSession>[] predicates)
        {
            if (predicates == null)
                throw new ArgumentNullException(nameof(predicates));

            _filter.AddOrUpdate(typeof(T),
                new List<Predicate<TSession>>(predicates),
                (key, oldValue) =>
                {
                    oldValue.AddRange(predicates);
                    return oldValue;
                });
            return this;
        }

        public FilteredMessageHandler<TSession> RegisterRule<T>(Predicate<TSession> predicate)
        {
            _filter.AddOrUpdate(typeof(T),
                new List<Predicate<TSession>> { predicate },
                (key, oldValue) =>
                {
                    oldValue.Add(predicate);
                    return oldValue;
                });
            return this;
        }
    }
}
