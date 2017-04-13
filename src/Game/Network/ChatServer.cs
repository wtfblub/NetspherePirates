using System;
using BlubLib.DotNetty.Handlers.MessageHandling;
using Netsphere.Network.Message.Chat;
using Netsphere.Network.Services;
using NLog;
using NLog.Fluent;
using ProudNet;
using ProudNet.Serialization;

namespace Netsphere.Network
{
    internal class ChatServer : ProudServer
    {
        public static ChatServer Instance { get; private set; }

        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Initialize(Configuration config)
        {
            if (Instance != null)
                throw new InvalidOperationException("Server is already initialized");

            config.Version = new Guid("{97d36acf-8cc0-4dfb-bcc9-97cab255e2bc}");
            config.MessageFactories = new MessageFactory[] { new ChatMessageFactory() };
            config.SessionFactory = new ChatSessionFactory();

            // ReSharper disable InconsistentNaming
            Predicate<ChatSession> MustBeLoggedIn = session => session.IsLoggedIn();
            Predicate<ChatSession> MustNotBeLoggedIn = session => !session.IsLoggedIn();
            Predicate<ChatSession> MustBeInChannel = session => session.Player.Channel != null;
            // ReSharper restore InconsistentNaming

            config.MessageHandlers = new IMessageHandler[]
            {
                new FilteredMessageHandler<ChatSession>()
                    .AddHandler(new AuthService())
                    .AddHandler(new CommunityService())
                    .AddHandler(new ChannelService())
                    .AddHandler(new PrivateMessageService())

                    .RegisterRule<LoginReqMessage>(MustNotBeLoggedIn)
                    .RegisterRule<CSetUserDataReqMessage>(MustBeLoggedIn)
                    .RegisterRule<UserDataOneReqMessage>(MustBeLoggedIn, MustBeInChannel)
                    .RegisterRule<DenyActionReqMessage>(MustBeLoggedIn)
                    .RegisterRule<MessageChatReqMessage>(MustBeLoggedIn, MustBeInChannel)
                    .RegisterRule<MessageWhisperChatReqMessage>(MustBeLoggedIn, MustBeInChannel)
                    .RegisterRule<NoteListReqMessage>(MustBeLoggedIn, MustBeInChannel)
                    .RegisterRule<NoteReadReqMessage>(MustBeLoggedIn, MustBeInChannel)
                    .RegisterRule<NoteDeleteReqMessage>(MustBeLoggedIn, MustBeInChannel)
                    .RegisterRule<NoteSendReqMessage>(MustBeLoggedIn, MustBeInChannel)
            };
            Instance = new ChatServer(config);
        }

        private ChatServer(Configuration config)
            : base(config)
        { }

        #region Events

        protected override void OnDisconnected(ProudSession session)
        {
            ((ChatSession)session).GameSession?.Dispose();
            base.OnDisconnected(session);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            var log = Logger.Error();
            if (e.Session != null)
                log = log.Account((ChatSession)e.Session);
            log.Exception(e.Exception)
                .Write();
            base.OnError(e);
        }

        //private void OnUnhandledMessage(object sender, MessageReceivedEventArgs e)
        //{
        //    var session = (ChatSession)e.Session;
        //    Logger.Warn()
        //        .Account(session)
        //        .Message($"Unhandled message {e.Message.GetType().Name}")
        //        .Write();
        //}

        #endregion
    }
}
