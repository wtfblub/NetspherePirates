using System;
using System.Buffers;
using System.Net;
using BlubLib.Network;
using BlubLib.Network.Pipes;
using BlubLib.Network.Transport.Sockets;
using Netsphere.Network.Message;
using Netsphere.Network.Message.Relay;
using Netsphere.Network.Services;
using NLog;
using NLog.Fluent;
using ProudNet;
using ProudNet.Message;

namespace Netsphere.Network
{
    internal class RelayServer : TcpServer
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public GameServer GameServer { get; }

        public RelayServer(GameServer gameServer)
            : base(new RelaySessionFactory(), ArrayPool<byte>.Create(1 * 1024 * 1024, 50), Config.Instance.PlayerLimit)
        {
            GameServer = gameServer;

            #region Filter Setup

            var config = new ProudConfig(new Guid("{a43a97d1-9ec7-495e-ad5f-8fe45fde1151}"))
            {
                UdpListener = new IPEndPoint(Config.Instance.Listener.Address, 22000),
                UdpAddress = IPAddress.Parse(Config.Instance.IP),
                EnableP2PEncryptedMessaging = false,
                EncryptedMessageKeyLength = 0
                //EnableServerLog = true,
                //EmergencyLogLineCount = 1000
            };
            var proudFilter = new ProudServerPipe(config);
#if DEBUG
            proudFilter.UnhandledProudCoreMessage += (s, e) =>
            {
                //var unk = e.Message as CoreUnknownMessage;
                //if (unk != null)
                    //_logger.Warn().Message("Unknown CoreMessage {0}: {1}", unk.OpCode, unk.Data.ToHexString()).Write();
                //else
                    _logger.Warn().Message("Unhandled UnhandledProudCoreMessage {0}", e.Message.GetType().Name).Write();
            };
            proudFilter.UnhandledProudMessage += (s, e) =>
            {
                var unk = e.Message as ProudUnknownMessage;
                if (unk != null)
                    _logger.Warn().Message("Unknown ProudMessage {0}: {1}", unk.OpCode, unk.Data.ToHexString()).Write();
                else
                    _logger.Warn().Message("Unhandled UnhandledProudMessage {0}", e.Message.GetType().Name).Write();
            };
#endif
            Pipeline.AddFirst("proudnet", proudFilter);
            Pipeline.AddLast("s4_protocol", new NetspherePipe(new RelayMessageFactory()));

            Pipeline.AddLast("firewall", new FirewallPipe())
                .Add(new PacketFirewallRule<RelaySession>())
                .Get<PacketFirewallRule<RelaySession>>()

                .Register<CRequestLoginMessage>(s => !s.IsLoggedIn());

            //FilterList.AddLast("spam_filter", new SpamFilter { RepeatLimit = 15, TimeFrame = TimeSpan.FromSeconds(3) });

            Pipeline.AddLast("s4_service", new ServicePipe())
                .Add(new AuthService())
                .Add(new RelayService())
                .UnhandledMessage += OnUnhandledMessage;

            #endregion
        }

        #region Events

        protected override void OnDisconnected(SessionEventArgs e)
        {
            var session = (RelaySession)e.Session;
            if (session.Player != null)
            {
                if (session.Player.Room != null)
                    session.GameSession.Dispose();
                else
                    session.Player.RelaySession = null;
            }
            base.OnDisconnected(e);
        }

        protected override void OnError(ExceptionEventArgs e)
        {
            _logger.Error()
                .Exception(e.Exception)
                .Write();
            base.OnError(e);
        }

        private void OnUnhandledMessage(object sender, MessageReceivedEventArgs e)
        {
            var session = (RelaySession)e.Session;
            var message = (ProudMessage)e.Message;
            //var unk = e.Message as RelayUnknownMessage;

            if (session.Player?.Room == null)
                return;

            //if (unk != null)
                //_logger.Warn().Account(session).Message("Unk message {0}: {1}", unk.OpCode, unk.Data.ToHexString()).Write();
            //else
                _logger.Warn().Account(session).Message("Unhandled message {0}", e.Message.GetType().Name).Write();

            //if (message.IsRelayed)
            //{
            //    var target = Sessions.Cast<RelaySession>().FirstOrDefault(s => s.RelayHostId == message.TargetHostId && s.P2PGroup == session.P2PGroup);
            //    target?.SendRelay(message.SenderHostId, message);
            //}
        }

        #endregion
    }
}
