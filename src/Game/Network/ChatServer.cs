using System;
using System.Buffers;
using BlubLib.Network;
using BlubLib.Network.Message;
using BlubLib.Network.Pipes;
using BlubLib.Network.Transport.Sockets;
using Netsphere.Network.Message;
using Netsphere.Network.Message.Chat;
using Netsphere.Network.Services;
using NLog;
using NLog.Fluent;
using ProudNet;

namespace Netsphere.Network
{
    internal class ChatServer : TcpServer
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public GameServer GameServer { get; private set; }

        public ChatServer(GameServer server)
            : base(new ChatSessionFactory(), ArrayPool<byte>.Create(1 * 1024 * 1024, 50), Config.Instance.PlayerLimit)
        {
            #region Filter Setup

            var config = new ProudConfig(new Guid("{97d36acf-8cc0-4dfb-bcc9-97cab255e2bc}"));
            var proudFilter = new ProudServerPipe(config);
#if DEBUG
            proudFilter.UnhandledProudCoreMessage += (s, e) => _logger.Warn().Message("Unhandled ProudCoreMessage {0}", e.Message.GetType().Name).Write();
            proudFilter.UnhandledProudMessage +=
                (s, e) => _logger.Warn().Message("Unhandled UnhandledProudMessage {0}: {1}", e.Message.GetType().Name, e.Message.ToArray().ToHexString()).Write();
#endif
            Pipeline.AddFirst("proudnet", proudFilter);
            Pipeline.AddLast("s4_protocol", new NetspherePipe(new ChatMessageFactory()));

            // ReSharper disable InconsistentNaming
            Predicate<ChatSession> MustBeLoggedIn = session => session.IsLoggedIn();
            Predicate<ChatSession> MustNotBeLoggedIn = session => !session.IsLoggedIn();
            Predicate<ChatSession> MustBeInChannel = session => session.Player.Channel != null;
            // ReSharper restore InconsistentNaming

            Pipeline.AddLast("firewall", new FirewallPipe())
                .Add(new PacketFirewallRule<ChatSession>())
                .Get<PacketFirewallRule<ChatSession>>()

                .Register<CLoginReqMessage>(MustNotBeLoggedIn)
                .Register<CSetUserDataReqMessage>(MustBeLoggedIn)
                .Register<CGetUserDataReqMessage>(MustBeLoggedIn, MustBeInChannel)
                .Register<CDenyChatReqMessage>(MustBeLoggedIn)
                .Register<CChatMessageReqMessage>(MustBeLoggedIn, MustBeInChannel)
                .Register<CWhisperChatMessageReqMessage>(MustBeLoggedIn, MustBeInChannel)
                .Register<CNoteListReqMessage>(MustBeLoggedIn, MustBeInChannel)
                .Register<CReadNoteReqMessage>(MustBeLoggedIn, MustBeInChannel)
                .Register<CDeleteNoteReqMessage>(MustBeLoggedIn, MustBeInChannel)
                .Register<CSendNoteReqMessage>(MustBeLoggedIn, MustBeInChannel);

            Pipeline.AddLast("s4_service", new MessageHandlerPipe())
                .Add(new AuthService())
                .Add(new CommunityService())
                .Add(new ChannelService())
                .Add(new PrivateMessageService())
                .UnhandledMessage += OnUnhandledMessage;

            #endregion

            GameServer = server;
        }

        #region Events

        protected override void OnDisconnected(SessionEventArgs e)
        {
            var session = (ChatSession)e.Session;
            session.GameSession?.Dispose();
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
            var session = (ChatSession)e.Session;
            _logger.Warn()
                .Account(session)
                .Message("Unhandled message {0}", e.Message.GetType().Name)
                .Write();
        }

        #endregion
    }
}
