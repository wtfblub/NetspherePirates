using System;
using System.Linq;
using System.Net;
using Auth.ServiceModel;
using BlubLib.Threading;
using ExpressMapper;
using Netsphere.Commands;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using Netsphere.Resource;
using NLog;
using NLog.Fluent;
using ProudNet;
using BlubLib.DotNetty.Handlers.MessageHandling;
using Netsphere.Network.Services;
using Netsphere.Network.Message.GameRule;
using ExpressMapper.Extensions;
using Netsphere.Network.Message.Club;
using ProudNet.Serialization;
using ClubInfoReqMessage = Netsphere.Network.Message.Game.ClubInfoReqMessage;

namespace Netsphere.Network
{
    internal class GameServer : ProudServer
    {
        public static GameServer Instance { get; private set; }

        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ILoop _worker;
        private readonly ServerlistManager _serverlistManager;

        private TimeSpan _mailBoxCheckTimer;
        private TimeSpan _saveTimer;

        public CommandManager CommandManager { get; }
        public PlayerManager PlayerManager { get; }
        public ChannelManager ChannelManager { get; }
        public ResourceCache ResourceCache { get; }

        public static void Initialize(Configuration config)
        {
            if (Instance != null)
                throw new InvalidOperationException("Server is already initialized");

            config.Version = new Guid("{beb92241-8333-4117-ab92-9b4af78c688f}");
            config.MessageFactories = new MessageFactory[] { new GameMessageFactory(), new GameRuleMessageFactory(), new ClubMessageFactory() };
            config.SessionFactory = new GameSessionFactory();

            // ReSharper disable InconsistentNaming
            Predicate<GameSession> MustBeLoggedIn = session => session.IsLoggedIn();
            Predicate<GameSession> MustNotBeLoggedIn = session => !session.IsLoggedIn();
            Predicate<GameSession> MustBeInChannel = session => session.Player.Channel != null;
            Predicate<GameSession> MustNotBeInChannel = session => session.Player.Channel == null;
            Predicate<GameSession> MustBeInRoom = session => session.Player.Room != null;
            Predicate<GameSession> MustNotBeInRoom = session => session.Player.Room == null;
            Predicate<GameSession> MustBeRoomHost = session => session.Player.Room.Host == session.Player;
            Predicate<GameSession> MustBeRoomMaster = session => session.Player.Room.Master == session.Player;
            // ReSharper restore InconsistentNaming

            config.MessageHandlers = new IMessageHandler[]
            {
                new FilteredMessageHandler<GameSession>()
                    .AddHandler(new AuthService())
                    .AddHandler(new CharacterService())
                    .AddHandler(new GeneralService())
                    .AddHandler(new AdminService())
                    .AddHandler(new ChannelService())
                    .AddHandler(new ShopService())
                    .AddHandler(new InventoryService())
                    .AddHandler(new RoomService())
                    .AddHandler(new ClubService())

                    .RegisterRule<LoginRequestReqMessage>(MustNotBeLoggedIn)
                    .RegisterRule<CharacterCreateReqMessage>(MustBeLoggedIn)
                    .RegisterRule<CharacterSelectReqMessage>(MustBeLoggedIn)
                    .RegisterRule<CharacterDeleteReqMessage>(MustBeLoggedIn)
                    .RegisterRule<AdminShowWindowReqMessage>(MustBeLoggedIn)
                    .RegisterRule<AdminActionReqMessage>(MustBeLoggedIn)
                    .RegisterRule<ChannelInfoReqMessage>(MustBeLoggedIn)
                    .RegisterRule<ChannelEnterReqMessage>(MustBeLoggedIn, MustNotBeInChannel)
                    .RegisterRule<ChannelLeaveReqMessage>(MustBeLoggedIn, MustBeInChannel)
                    .RegisterRule<LicenseGainReqMessage>(MustBeLoggedIn, MustBeInChannel)
                    .RegisterRule<LicenseExerciseReqMessage>(MustBeLoggedIn, MustBeInChannel)
                    .RegisterRule<ItemBuyItemReqMessage>(MustBeLoggedIn)
                    .RegisterRule<RandomShopRollingStartReqMessage>(MustBeLoggedIn)
                    //.RegisterRule<CRandomShopItemSaleReqMessage>(MustBeLoggedIn)
                    .RegisterRule<ItemUseItemReqMessage>(MustBeLoggedIn)
                    .RegisterRule<ItemRepairItemReqMessage>(MustBeLoggedIn)
                    .RegisterRule<ItemRefundItemReqMessage>(MustBeLoggedIn)
                    .RegisterRule<ItemDiscardItemReqMessage>(MustBeLoggedIn)
                    .RegisterRule<RoomEnterPlayerReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom,
                        session => session.Player.RoomInfo.IsConnecting)
                    .RegisterRule<RoomMakeReqMessage>(MustBeLoggedIn, MustBeInChannel, MustNotBeInRoom)
                    .RegisterRule<RoomEnterReqMessage>(MustBeLoggedIn, MustBeInChannel, MustNotBeInRoom)
                    .RegisterRule<RoomLeaveReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom)
                    .RegisterRule<RoomTeamChangeReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom)
                    .RegisterRule<RoomPlayModeChangeReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom)
                    .RegisterRule<ScoreKillReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom)
                    .RegisterRule<ScoreKillAssistReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom)
                    .RegisterRule<ScoreOffenseReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom)
                    .RegisterRule<ScoreOffenseAssistReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom)
                    .RegisterRule<ScoreDefenseReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom)
                    .RegisterRule<ScoreDefenseAssistReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom)
                    .RegisterRule<ScoreTeamKillReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom)
                    .RegisterRule<ScoreHealAssistReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom)
                    .RegisterRule<ScoreSuicideReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom)
                    .RegisterRule<ScoreReboundReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom, MustBeRoomHost,
                        session => session.Player.RoomInfo.State != PlayerState.Lobby &&
                                   session.Player.RoomInfo.State != PlayerState.Spectating)
                    .RegisterRule<ScoreGoalReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom, MustBeRoomHost,
                        session => session.Player.RoomInfo.State != PlayerState.Lobby &&
                                   session.Player.RoomInfo.State != PlayerState.Spectating)
                    .RegisterRule<RoomBeginRoundReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom, MustBeRoomMaster)
                    .RegisterRule<RoomReadyRoundReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom,
                        session => session.Player.RoomInfo.State == PlayerState.Lobby)
                    .RegisterRule<GameEventMessageReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom)
                    .RegisterRule<RoomItemChangeReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom,
                        session => session.Player.RoomInfo.State == PlayerState.Lobby)
                    .RegisterRule<GameAvatarChangeReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom,
                        session => session.Player.RoomInfo.State == PlayerState.Lobby ||
                                   session.Player.Room.GameRuleManager.GameRule.StateMachine.IsInState(
                                       GameRuleState.HalfTime))
                    .RegisterRule<RoomChangeRuleNotifyReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom,
                        MustBeRoomMaster,
                        session =>
                            session.Player.Room.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Waiting))
                    .RegisterRule<ClubAddressReqMessage>(MustBeLoggedIn, MustBeInChannel)
                    .RegisterRule<ClubInfoReqMessage>(MustBeLoggedIn, MustBeInChannel)
                    .RegisterRule<RoomLeaveReguestReqMessage>(MustBeLoggedIn, MustBeInChannel, MustBeInRoom)

            };

            Instance = new GameServer(config);
        }

        private GameServer(Configuration config)
            : base(config)
        {
            RegisterMappings();

            //ServerTime = TimeSpan.Zero;

            CommandManager = new CommandManager(this);
            CommandManager.Add(new ServerCommand())
                .Add(new ReloadCommand())
                .Add(new GameCommands())
                .Add(new InventoryCommands());

            PlayerManager = new PlayerManager();
            ResourceCache = new ResourceCache();
            ChannelManager = new ChannelManager(ResourceCache.GetChannels());

            _worker = new ThreadLoop(TimeSpan.FromMilliseconds(100), (Action<TimeSpan>)Worker);
            _serverlistManager = new ServerlistManager();
        }

        #region Events

        protected override void OnStarted()
        {
            ResourceCache.PreCache();
            _worker.Start();
            _serverlistManager.Start();
        }

        protected override void OnStopping()
        {
            _worker.Stop(new TimeSpan(0));
            _serverlistManager.Dispose();
        }

        protected override void OnDisconnected(ProudSession session)
        {
            var gameSession = (GameSession)session;
            if (gameSession.Player != null)
            {
                gameSession.Player.Room?.Leave(gameSession.Player);
                gameSession.Player.Channel?.Leave(gameSession.Player);

                gameSession.Player.Save();

                PlayerManager.Remove(gameSession.Player);

                Logger.Debug()
                    .Account(gameSession)
                    .Message("Disconnected")
                    .Write();

                if (gameSession.Player.ChatSession != null)
                {
                    gameSession.Player.ChatSession.GameSession = null;
                    gameSession.Player.ChatSession.Dispose();
                }

                if (gameSession.Player.RelaySession != null)
                {
                    gameSession.Player.RelaySession.GameSession = null;
                    gameSession.Player.RelaySession.Dispose();
                }

                gameSession.Player.Session = null;
                gameSession.Player.ChatSession = null;
                gameSession.Player.RelaySession = null;
                gameSession.Player = null;
            }

            base.OnDisconnected(session);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            var log = Logger.Error();
            if (e.Session != null)
                log = log.Account((GameSession)e.Session);
            log.Exception(e.Exception)
                .Write();
            base.OnError(e);
        }

        //private void OnUnhandledMessage(object sender, MessageReceivedEventArgs e)
        //{
        //    var session = (GameSession)e.Session;
        //    Logger.Warn()
        //        .Account(session)
        //        .Message($"Unhandled message {e.Message.GetType().Name}")
        //        .Write();
        //}

        #endregion

        public void BroadcastNotice(string message)
        {
            Broadcast(new NoticeAdminMessageAckMessage(message));
        }

        private void Worker(TimeSpan delta)
        {
            ChannelManager.Update(delta);

            // ToDo Use another thread for this?
            _saveTimer = _saveTimer.Add(delta);
            if (_saveTimer >= Config.Instance.SaveInterval)
            {
                _saveTimer = TimeSpan.Zero;

                Logger.Info("Saving players...");

                foreach (var plr in PlayerManager.Where(plr => plr.IsLoggedIn()))
                {
                    try
                    {
                        plr.Save();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error()
                            .Account(plr)
                            .Exception(ex)
                            .Message("Failed to save player")
                            .Write();
                    }
                }

                Logger.Info("Saving players completed");
            }

            _mailBoxCheckTimer = _mailBoxCheckTimer.Add(delta);
            if (_mailBoxCheckTimer >= TimeSpan.FromMinutes(10))
            {
                _mailBoxCheckTimer = TimeSpan.Zero;

                foreach (var plr in PlayerManager.Where(plr => plr.IsLoggedIn()))
                    plr.Mailbox.Remove(plr.Mailbox.Where(mail => mail.Expires >= DateTimeOffset.Now));
            }
        }

        private static void RegisterMappings()
        {
            Mapper.Register<GameServer, ServerInfoDto>()
                .Member(dest => dest.Id, src => Config.Instance.Id)
                .Member(dest => dest.Name, src => Config.Instance.Name)
                .Member(dest => dest.PlayerLimit, src => Config.Instance.PlayerLimit)
                .Member(dest => dest.PlayerOnline, src => src.Sessions.Count)
                .Member(dest => dest.EndPoint,
                    src => new IPEndPoint(IPAddress.Parse(Config.Instance.IP), Config.Instance.Listener.Port))
                .Member(dest => dest.ChatEndPoint,
                    src => new IPEndPoint(IPAddress.Parse(Config.Instance.IP), Config.Instance.ChatListener.Port));

            Mapper.Register<Player, PlayerAccountInfoDto>()
                .Function(dest => dest.IsGM, src => src.Account.SecurityLevel > SecurityLevel.User)
                .Member(dest => dest.TotalExp, src => src.TotalExperience)
                .Function(dest => dest.TutorialState, src => (uint)(Config.Instance.Game.EnableTutorial ? src.TutorialState : 2))
                .Member(dest => dest.Nickname, src => src.Account.Nickname);

            Mapper.Register<Channel, ChannelInfoDto>()
                .Member(dest => dest.PlayersOnline, src => src.Players.Count);

            Mapper.Register<PlayerItem, ItemDto>()
                .Function(dest => dest.ExpireTime,
                    src => src.ExpireDate == DateTimeOffset.MinValue ? -1 : src.ExpireDate.ToUnixTimeSeconds());

            Mapper.Register<Deny, DenyDto>()
                .Member(dest => dest.AccountId, src => src.DenyId)
                .Member(dest => dest.Nickname, src => src.Nickname);

            //Mapper.Register<Room, RoomDto>()
            //    .Member(dest => dest.RoomId, src => src.Id)
            //    .Member(dest => dest.MatchKey, src => src.Options.MatchKey)
            //    .Member(dest => dest.Name, src => src.Options.Name)
            //    .Member(dest => dest.HasPassword, src => !string.IsNullOrWhiteSpace(src.Options.Password))
            //    .Member(dest => dest.TimeLimit, src => src.Options.TimeLimit.TotalMilliseconds)
            //    .Member(dest => dest.ScoreLimit, src => src.Options.ScoreLimit)

            //    .Member(dest => dest.IsFriendly, src => src.Options.IsFriendly)
            //    .Member(dest => dest.IsBalanced, src => src.Options.IsBalanced)
            //    .Member(dest => dest.MinLevel, src => src.Options.MinLevel)
            //    .Member(dest => dest.MaxLevel, src => src.Options.MaxLevel)
            //    .Member(dest => dest.EquipLimit, src => src.Options.ItemLimit)
            //    .Member(dest => dest.IsNoIntrusion, src => src.Options.IsNoIntrusion)

            //    .Member(dest => dest.ConnectingCount, src => src.TeamManager.Players.Count())
            //    .Member(dest => dest.PlayerCount, src => src.TeamManager.Players.Count())
            //    .Member(dest => dest.Latency, src => src.GetLatency())
            //    .Function(dest => dest.State, src =>
            //    {
            //        if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Waiting))
            //            return GameState.Waiting;

            //        if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Result))
            //            return GameState.Result;

            //        if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Playing))
            //            return GameState.Playing;

            //        throw new InvalidOperationException();
            //    });

            //Mapper.Register<Room, EnterRoomInfoDto>()
            //    .Member(dest => dest.RoomId, src => src.Id)
            //    .Member(dest => dest.MatchKey, src => src.Options.MatchKey)
            //    .Member(dest => dest.TimeLimit, src => src.Options.TimeLimit.TotalMilliseconds)
            //    .Member(dest => dest.TimeSync, src => src.GameRuleManager.GameRule.RoundTime.TotalMilliseconds)
            //    .Member(dest => dest.ScoreLimit, src => src.Options.ScoreLimit)
            //    .Member(dest => dest.IsFriendly, src => src.Options.IsFriendly)
            //    .Member(dest => dest.IsBalanced, src => src.Options.IsBalanced)
            //    .Member(dest => dest.MinLevel, src => src.Options.MinLevel)
            //    .Member(dest => dest.MaxLevel, src => src.Options.MaxLevel)
            //    .Member(dest => dest.ItemLimit, src => src.Options.ItemLimit)
            //    .Member(dest => dest.IsNoIntrusion, src => src.Options.IsNoIntrusion)
            //    .Member(dest => dest.RelayEndPoint, src => src.Options.ServerEndPoint)
            //    .Function(dest => dest.State, src =>
            //    {
            //        if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Waiting))
            //            return GameState.Waiting;

            //        if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Result))
            //            return GameState.Result;

            //        if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Playing))
            //            return GameState.Playing;

            //        throw new InvalidOperationException();
            //    })
            //    .Function(dest => dest.TimeState, src =>
            //    {
            //        //if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.FirstHalf))
            //        //    return GameTimeState.FirstHalf;

            //        if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.HalfTime))
            //            return GameTimeState.HalfTime;

            //        if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.SecondHalf))
            //            return GameTimeState.SecondHalf;

            //        return GameTimeState.FirstHalf;
            //    });

            Mapper.Register<Player, RoomPlayerDto>()
                .Member(dest => dest.AccountId, src => src.Account.Id)
                .Member(dest => dest.Nickname, src => src.Account.Nickname)
                .Value(dest => dest.Unk1, (byte)144);

            Mapper.Register<PlayerItem, Data.P2P.ItemDto>()
                .Function(dest => dest.ItemNumber, src => src?.ItemNumber ?? 0);

            //Mapper.Register<RoomCreationOptions, ChangeRuleDto>()
            //    .Member(dest => dest.Name, src => src.Name)
            //    .Member(dest => dest.Password, src => src.Password)
            //    .Function(dest => dest.MatchKey, src => src.MatchKey)
            //    .Member(dest => dest.TimeLimit, src => src.TimeLimit)
            //    .Member(dest => dest.ScoreLimit, src => src.ScoreLimit)
            //    .Member(dest => dest.IsFriendly, src => src.IsFriendly)
            //    .Member(dest => dest.IsBalanced, src => src.IsBalanced)
            //    .Member(dest => dest.ItemLimit, src => src.ItemLimit)
            //    .Member(dest => dest.IsNoIntrusion, src => src.IsNoIntrusion);

            Mapper.Register<Mail, NoteDto>()
                .Function(dest => dest.ReadCount, src => src.IsNew ? 0 : 1)
                .Function(dest => dest.DaysLeft,
                    src => DateTimeOffset.Now < src.Expires ? (src.Expires - DateTimeOffset.Now).TotalDays : 0);

            Mapper.Register<Mail, NoteContentDto>()
                .Member(dest => dest.Id, src => src.Id)
                .Member(dest => dest.Message, src => src.Message);

            Mapper.Register<PlayerItem, ItemDurabilityInfoDto>()
                .Member(dest => dest.ItemId, src => src.Id);

            Mapper.Register<Player, PlayerInfoShortDto>()
                .Member(dest => dest.AccountId, src => src.Account.Id)
                .Member(dest => dest.Nickname, src => src.Account.Nickname)
                .Function(dest => dest.TotalExp, src => src.TotalExperience);

            Mapper.Register<Player, PlayerLocationDto>()
                .Function(dest => dest.ServerGroupId, src => Config.Instance.Id)
                .Function(dest => dest.ChannelId, src => src.Channel != null ? (short)src.Channel.Id : (short)-1)
                .Function(dest => dest.RoomId, src => src.Room?.Id ?? 0xFFFFFFFF) // ToDo: Tutorial, License
                .Function(dest => dest.GameServerId, src => 0) // TODO Server ids
                .Function(dest => dest.ChatServerId, src => 0);

            Mapper.Register<Player, PlayerInfoDto>()
                .Function(dest => dest.Info, src => src.Map<Player, PlayerInfoShortDto>())
                .Function(dest => dest.Location, src => src.Map<Player, PlayerLocationDto>());

            Mapper.Compile(CompilationTypes.Source);
        }
    }
}
