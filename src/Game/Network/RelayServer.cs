using System;
using BlubLib.DotNetty.Handlers.MessageHandling;
using Netsphere.Network.Message.Relay;
using Netsphere.Network.Services;
using NLog;
using NLog.Fluent;
using ProudNet;
using ProudNet.Serialization;

namespace Netsphere.Network
{
    internal class RelayServer : ProudServer
    {
        public static RelayServer Instance { get; private set; }

        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Initialize(Configuration config)
        {
            if (Instance != null)
                throw new InvalidOperationException("Server is already initialized");

            config.Version = new Guid("{a43a97d1-9ec7-495e-ad5f-8fe45fde1151}");
            config.MessageFactories = new MessageFactory[] { new RelayMessageFactory() };
            config.SessionFactory = new RelaySessionFactory();

            // ReSharper disable InconsistentNaming
            Predicate<RelaySession> MustNotBeLoggedIn = session => !session.IsLoggedIn();
            // ReSharper restore InconsistentNaming

            config.MessageHandlers = new IMessageHandler[]
            {
                new FilteredMessageHandler<RelaySession>()
                    .AddHandler(new AuthService())

                    .RegisterRule<CRequestLoginMessage>(MustNotBeLoggedIn)
            };
            Instance = new RelayServer(config);
        }

        private RelayServer(Configuration config)
            : base(config)
        { }

        #region Events

        protected override void OnDisconnected(ProudSession session)
        {
            var relaySession = (RelaySession)session;
            if (relaySession.GameSession != null && relaySession.Player != null)
            {
                if (relaySession.Player.Room != null)
                    relaySession.GameSession.Dispose();
            }

            relaySession.GameSession = null;
            base.OnDisconnected(session);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            var log = Logger.Error();
            if (e.Session != null)
                log = log.Account((RelaySession)e.Session);
            log.Exception(e.Exception)
                .Write();
            base.OnError(e);
        }

        //private void OnUnhandledMessage(object sender, MessageReceivedEventArgs e)
        //{
        //    var session = (RelaySession)e.Session;
        //    var message = (ProudMessage)e.Message;
        //    //var unk = e.Message as RelayUnknownMessage;

        //    if (session.Player?.Room == null)
        //        return;

        //    //if (unk != null)
        //    //_logger.Warn().Account(session).Message("Unk message {0}: {1}", unk.OpCode, unk.Data.ToHexString()).Write();
        //    //else
        //    Logger.Warn()
        //        .Account(session)
        //        .Message($"Unhandled message {e.Message.GetType().Name}")
        //        .Write();

        //    //if (message.IsRelayed)
        //    //{
        //    //    var target = Sessions.Cast<RelaySession>().FirstOrDefault(s => s.RelayHostId == message.TargetHostId && s.P2PGroup == session.P2PGroup);
        //    //    target?.SendRelay(message.SenderHostId, message);
        //    //}
        //}

        #endregion
    }
}
