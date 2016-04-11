using System;
using System.Collections.Generic;
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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDictionary<Type, Predicate<TSession>> _filter = new Dictionary<Type, Predicate<TSession>>();

        public PacketFirewallRule<TSession> Register<T>(Predicate<TSession> predicate)
            where T : IMessage
        {
            _filter.Add(typeof(T), predicate);
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
            Predicate<TSession> predicate;
            if (_filter.TryGetValue(e.Message.GetType(), out predicate))
            {
                if (!predicate((TSession) e.Session))
                {
                    Logger.Debug()
                        .Message("Dropping message {0} from client {1}", e.Message.GetType().Name, ((TcpProcessor)e.Session.Processor).Socket.RemoteEndPoint.ToString())
                        .Write();
                    return false;
                }
            }
            return true;
        }

        public bool OnSendMessage(MessageEventArgs e)
        {
            return true;
        }
    }
}
