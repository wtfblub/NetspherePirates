using System;
using System.Collections.Generic;
using System.Linq;
using BlubLib.Network;
using BlubLib.Network.Message;
using BlubLib.Network.Pipes;
using BlubLib.Network.Transport.Sockets;
using NLog;
using NLog.Fluent;
using ProudNet;

namespace Netsphere.Network
{
    internal class PacketFirewallRule<TSession> : FirewallPipe.IFirewallRule
        where TSession : ProudSession
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDictionary<Type, List<Predicate<TSession>>> _filter = new Dictionary<Type, List<Predicate<TSession>>>();

        public PacketFirewallRule<TSession> Register<T>(params Predicate<TSession>[] predicates)
            where T : IMessage
        {
            if(predicates == null)
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

        public PacketFirewallRule<TSession> Register<T>(Predicate<TSession> predicate)
            where T : IMessage
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

        public bool OnConnecting(SessionEventArgs e)
        {
            return true;
        }

        public bool OnConnected(SessionEventArgs e)
        {
            return true;
        }

        public bool OnMessageReceived(MessageReceivedEventArgs e)
        {
            List<Predicate<TSession>> predicates;
            if (!_filter.TryGetValue(e.Message.GetType(), out predicates))
                return true;

            if (predicates.Any(predicate => !predicate((TSession)e.Session)))
            {
                Logger.Debug()
                    .Message("Dropping message {0} from client {1}", e.Message.GetType().Name, ((TcpProcessor)e.Session.Processor).Socket.RemoteEndPoint.ToString())
                    .Write();
                return false;
            }
            return true;
        }

        public bool OnSendMessage(MessageEventArgs e)
        {
            return true;
        }
    }
}
