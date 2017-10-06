using System;
using BlubLib.DotNetty.Handlers.MessageHandling;
using Netsphere.Network.Message.Chat;
using Netsphere.Network.Services;
using ProudNet;
using ProudNet.Serialization;
using Serilog;
using Serilog.Core;

namespace Netsphere.Network
{
    internal class ChatServer : ProudServer
    {
        public static ChatServer Instance { get; private set; }

        // ReSharper disable once InconsistentNaming
        private static readonly ILogger Logger = Log.ForContext(Constants.SourceContextPropertyName, nameof(ChatServer));

        public static void Initialize(ProudNet.Configuration config)
        {
            if (Instance != null)
                throw new InvalidOperationException("Server is already initialized");

            config.Version = new Guid("{97d36acf-8cc0-4dfb-bcc9-97cab255e2bc}");
            config.MessageFactories = new MessageFactory[] { new ChatMessageFactory() };
            config.SessionFactory = new ChatSessionFactory();
            config.Logger = Logger.ForContext(Constants.SourceContextPropertyName, "ChatServer-ProudNet");

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

        private ChatServer(ProudNet.Configuration config)
            : base(config)
        { }

        #region Events

        protected override void OnDisconnected(ProudSession session)
        {
            ((ChatSession)session).GameSession?.Dispose();
            ((ChatSession)session).GameSession = null;
            base.OnDisconnected(session);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            var log = Logger;
            if (e.Session != null)
                log = log.ForAccount((ChatSession)e.Session);
            log.Error(e.Exception, "Unhandled server error");
            base.OnError(e);
        }

        //private void OnUnhandledMessage(object sender, MessageReceivedEventArgs e)
        //{
        //    var session = (ChatSession)e.Session;
        //    Log.Warning()
        //        .Account(session)
        //        .Message($"Unhandled message {e.Message.GetType().Name}")
        //        .Write();
        //}

        #endregion
    }
}
