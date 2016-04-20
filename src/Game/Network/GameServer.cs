using System;
using System.Buffers;
using System.Linq;
using System.Net;
using Auth.ServiceModel;
using BlubLib.Network;
using BlubLib.Network.Pipes;
using BlubLib.Network.Transport.Sockets;
using BlubLib.Threading;
using ExpressMapper;
using ExpressMapper.Extensions;
using Netsphere.Commands;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Message;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;
using Netsphere.Network.Services;
using Netsphere.Resource;
using NLog;
using NLog.Fluent;
using ProudNet;
using BlubLib.Network.Message;

namespace Netsphere.Network
{
    internal class GameServer : TcpServer
    {
        public static GameServer Instance { get; } = new GameServer();

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ChatServer _chatServer;
        private readonly ILoop _worker;
        private readonly AuthWebAPIClient _authWebAPI;

        private TimeSpan _serverListUpdateTimer;
        private TimeSpan _mailBoxCheckTimer;
        private TimeSpan _saveTimer;

        public CommandManager CommandManager { get; }
        public PlayerManager PlayerManager { get; }
        public ChannelManager ChannelManager { get; }
        public ResourceCache ResourceCache { get; }

        public RelayServer RelayServer { get; }

        private GameServer()
            : base(new GameSessionFactory(), ArrayPool<byte>.Create(1 * 1024 * 1024, 50), Config.Instance.PlayerLimit)
        {
            #region Filter Setup

            var config = new ProudConfig(new Guid("{beb92241-8333-4117-ab92-9b4af78c688f}"));
            var proudFilter = new ProudServerPipe(config);
#if DEBUG
            proudFilter.UnhandledProudCoreMessage += (s, e) => Logger.Warn().Message("Unhandled ProudCoreMessage {0}", e.Message.GetType().Name).Write();
            proudFilter.UnhandledProudMessage +=
                (s, e) => Logger.Warn().Message("Unhandled UnhandledProudMessage {0}: {1}", e.Message.GetType().Name, e.Message.ToArray().ToHexString()).Write();
#endif
            Pipeline.AddFirst("proudnet", proudFilter);
            Pipeline.AddLast("s4_protocol", new NetspherePipe(new GameMessageFactory()));

            Pipeline.AddLast("firewall", new FirewallPipe())
                .Add(new PacketFirewallRule<GameSession>())
                .Get<PacketFirewallRule<GameSession>>()

                .Register<CLoginReqMessage>(s => !s.IsLoggedIn())
                .Register<CCreateCharacterReqMessage>(s => s.IsLoggedIn())
                .Register<CSelectCharacterReqMessage>(s => s.IsLoggedIn())
                .Register<CDeleteCharacterReqMessage>(s => s.IsLoggedIn())
                .Register<CAdminShowWindowReqMessage>(s => s.IsLoggedIn())
                .Register<CAdminActionReqMessage>(s => s.IsLoggedIn())
                .Register<CGetChannelInfoReqMessage>(s => s.IsLoggedIn())
                .Register<CChannelEnterReqMessage>(s => s.IsLoggedIn() && s.Player.Channel == null)
                .Register<CChannelLeaveReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null)
                .Register<CNewShopUpdateCheckReqMessage>(s => s.IsLoggedIn())
                .Register<CLicensedReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null)
                .Register<CExerciseLicenceReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null)
                .Register<CBuyItemReqMessage>(s => s.IsLoggedIn())
                .Register<CRandomShopRollingStartReqMessage>(s => s.IsLoggedIn())
                .Register<CRandomShopItemSaleReqMessage>(s => s.IsLoggedIn())
                .Register<CUseItemReqMessage>(s => s.IsLoggedIn())
                .Register<CRepairItemReqMessage>(s => s.IsLoggedIn())
                .Register<CRefundItemReqMessage>(s => s.IsLoggedIn())
                .Register<CDiscardItemReqMessage>(s => s.IsLoggedIn())
                .Register<CEnterPlayerReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null && s.Player.RoomInfo.IsConnecting)
                .Register<CMakeRoomReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room == null)
                .Register<CGameRoomEnterReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room == null)
                .Register<CJoinTunnelInfoReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null)
                .Register<CChangeTeamReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null)
                .Register<CPlayerGameModeChangeReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null)

                .Register<CScoreKillReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null && s.Player.RoomInfo.State != PlayerState.Lobby && s.Player.RoomInfo.State != PlayerState.Spectating)
                .Register<CScoreKillAssistReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null && s.Player.RoomInfo.State != PlayerState.Lobby && s.Player.RoomInfo.State != PlayerState.Spectating)
                .Register<CScoreOffenseReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null && s.Player.RoomInfo.State != PlayerState.Lobby && s.Player.RoomInfo.State != PlayerState.Spectating)
                .Register<CScoreOffenseAssistReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null && s.Player.RoomInfo.State != PlayerState.Lobby && s.Player.RoomInfo.State != PlayerState.Spectating)
                .Register<CScoreDefenseReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null && s.Player.RoomInfo.State != PlayerState.Lobby && s.Player.RoomInfo.State != PlayerState.Spectating)
                .Register<CScoreDefenseAssistReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null && s.Player.RoomInfo.State != PlayerState.Lobby && s.Player.RoomInfo.State != PlayerState.Spectating)
                .Register<CScoreTeamKillReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null && s.Player.RoomInfo.State != PlayerState.Lobby && s.Player.RoomInfo.State != PlayerState.Spectating)
                .Register<CScoreHealAssistReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null && s.Player.RoomInfo.State != PlayerState.Lobby && s.Player.RoomInfo.State != PlayerState.Spectating)
                .Register<CScoreSuicideReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null && s.Player.RoomInfo.State != PlayerState.Lobby && s.Player.RoomInfo.State != PlayerState.Spectating)
                .Register<CScoreReboundReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null && s.Player.Room.Host == s.Player && s.Player.RoomInfo.State != PlayerState.Lobby && s.Player.RoomInfo.State != PlayerState.Spectating)
                .Register<CScoreGoalReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null && s.Player.Room.Host == s.Player && s.Player.RoomInfo.State != PlayerState.Lobby && s.Player.RoomInfo.State != PlayerState.Spectating)

                .Register<CBeginRoundReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null && s.Player.Room.Master == s.Player)
                .Register<CReadyRoundReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null && s.Player.RoomInfo.State == PlayerState.Lobby)
                .Register<CEventMessageReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null)
                .Register<CItemsChangeReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null && s.Player.RoomInfo.State == PlayerState.Lobby)
                .Register<CAvatarChangeReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null && (s.Player.RoomInfo.State == PlayerState.Lobby || s.Player.Room.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.HalfTime)))
                .Register<CChangeRuleNotifyReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null && s.Player.Room != null && s.Player == s.Player.Room.Master && s.Player.Room.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Waiting))

                .Register<CClubAddressReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null)
                .Register<CClubInfoReqMessage>(s => s.IsLoggedIn() && s.Player.Channel != null);

            //FilterList.AddLast("spam_filter", new SpamFilter { RepeatLimit = 30, TimeFrame = TimeSpan.FromSeconds(3) });

            Pipeline.AddLast("s4_service", new ServicePipe())
                .Add(new AuthService())
                .Add(new AccountService())
                .Add(new CharacterService())
                .Add(new GeneralService())
                .Add(new AdminService())
                .Add(new ChannelService())
                .Add(new ShopService())
                .Add(new InventoryService())
                .Add(new RoomService())
                .Add(new ClubService())
                .UnhandledMessage += OnUnhandledMessage;

            #endregion

            RegisterMappings();

            //ServerTime = TimeSpan.Zero;
            _chatServer = new ChatServer(this);
            RelayServer = new RelayServer(this);

            CommandManager = new CommandManager(this);
            CommandManager.Add(new ServerCommand())
                .Add(new ReloadCommand())
                .Add(new GameCommands())
                .Add(new InventoryCommands());

            PlayerManager = new PlayerManager();
            ResourceCache = new ResourceCache();
            ChannelManager = new ChannelManager(ResourceCache.GetChannels());

            _worker = new ThreadLoop(TimeSpan.FromMilliseconds(100), (Action<TimeSpan>)Worker);
            _authWebAPI = new AuthWebAPIClient();
        }

        #region Events

        protected override void OnDisconnected(SessionEventArgs e)
        {
            var session = (GameSession)e.Session;
            if (session.Player != null)
            {
                session.Player.Room?.Leave(session.Player);
                session.Player.Channel?.Leave(session.Player);

                session.Player.Save(true);

                PlayerManager.Remove(session.Player);

                Logger.Debug()
                    .Account(session)
                    .Message("Disconnected")
                    .Write();

                if (session.Player.ChatSession != null)
                {
                    session.Player.ChatSession.GameSession = null;
                    session.Player.ChatSession.Close();
                }

                if (session.Player.RelaySession != null)
                {
                    session.Player.RelaySession.GameSession = null;
                    session.Player.RelaySession.Close();
                }

                session.Player.Session = null;
                session.Player.ChatSession = null;
                session.Player.RelaySession = null;
                session.Player = null;
            }
            base.OnDisconnected(e);
        }

        protected override void OnError(ExceptionEventArgs e)
        {
            Logger.Error()
                .Exception(e.Exception)
                .Write();
            base.OnError(e);
        }

        private void OnUnhandledMessage(object sender, MessageReceivedEventArgs e)
        {
            var session = (GameSession)e.Session;
            Logger.Warn()
                .Account(session)
                .Message("Unhandled message {0}", e.Message.GetType().Name)
                .Write();
        }

        #endregion

        public override void Start(IPEndPoint localEP)
        {
            ResourceCache.PreCache();
            base.Start(localEP);
            _chatServer.Start(Config.Instance.ChatListener);
            RelayServer.Start(Config.Instance.RelayListener);
            _worker.Start();

            RegisterServer();
        }

        public override void Stop()
        {
            _worker.Stop(new TimeSpan(0));

            try
            {
                _authWebAPI.RemoveServer(Config.Instance.Id);
            }
            catch
            {
                // ignored
            }

            _chatServer.Stop();
            RelayServer.Stop();
            base.Stop();
        }

        public void BroadcastNotice(string message)
        {
            Send(new SNoticeMessageAckMessage(message));
        }

        private void Worker(TimeSpan delta)
        {
            ChannelManager.Update(delta);

            _serverListUpdateTimer = _serverListUpdateTimer.Add(delta);
            if (_serverListUpdateTimer >= Config.Instance.AuthWebAPI.UpdateInterval)
            {
                _serverListUpdateTimer = TimeSpan.Zero;
                try
                {
                    if(!_authWebAPI.UpdateServer(this.Map<GameServer, ServerInfoDto>()))
                        RegisterServer();
                }
                catch (Exception ex)
                {
                    Logger.Error()
                        .Exception(ex)
                        .Message("Failed to update serverlist")
                        .Write();
                }
            }

            // ToDo Use another thread for this?
            _saveTimer = _saveTimer.Add(delta);
            if (_saveTimer >= Config.Instance.SaveInterval)
            {
                _saveTimer = TimeSpan.Zero;

                Logger.Info()
                    .Message("Saving players...")
                    .Write();

                foreach (var plr in PlayerManager.Where(plr => plr.IsLoggedIn()))
                {
                    try
                    {
                        plr.Save(true);
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

                Logger.Info()
                    .Message("Saving players completed")
                    .Write();
            }

            _mailBoxCheckTimer = _mailBoxCheckTimer.Add(delta);
            if (_mailBoxCheckTimer >= TimeSpan.FromMinutes(10))
            {
                _mailBoxCheckTimer = TimeSpan.Zero;

                foreach (var plr in PlayerManager.Where(plr => plr.IsLoggedIn()))
                    plr.Mailbox.Remove(plr.Mailbox.Where(mail => mail.Expires >= DateTimeOffset.Now));
            }
        }

        private void RegisterServer()
        {
            try
            {
                var result = _authWebAPI.RegisterServer(this.Map<GameServer, ServerInfoDto>());
                if (result == RegisterResult.AlreadyExists)
                {
                    Logger.Error()
                        .Message("Server id already exists. Please make sure you're using a unique id")
                        .Write();
                }
            }
            catch (Exception ex)
            {
                Logger.Error()
                    .Exception(ex)
                    .Message("Failed to register to serverlist")
                    .Write();
            }
        }

        private void RegisterMappings()
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

            Mapper.Register<Channel, ChannelInfoDto>()
                .Member(dest => dest.ChannelId, src => src.Id)
                .Member(dest => dest.PlayerCount, src => src.Players.Count);

            Mapper.Register<PlayerItem, ItemDto>()
                .Member(dest => dest.Refund, src => src.CalculateRefund())
                .Member(dest => dest.PurchaseTime, src => src.PurchaseDate.ToUnixTimeSeconds())
                .Member(dest => dest.ExpireTime,
                    src => src.ExpireDate == DateTimeOffset.MinValue ? -1 : src.ExpireDate.ToUnixTimeSeconds())

                // ToDo
                .Value(dest => dest.TimeLeft, 0)
                .Value(dest => dest.Unk1, (uint)0)
                .Value(dest => dest.Unk2, 0)
                .Value(dest => dest.Unk3, 0)
                .Value(dest => dest.Unk4, 0)
                .Value(dest => dest.Unk5, (uint)0)
                .Value(dest => dest.Unk6, (uint)0);

            Mapper.Register<Deny, DenyDto>()
                .Member(dest => dest.AccountId, src => src.DenyId)
                .Member(dest => dest.Nickname, src => src.Nickname);

            Mapper.Register<Room, RoomDto>()
                .Member(dest => dest.RoomId, src => src.Id)
                .Member(dest => dest.MatchKey, src => src.Options.MatchKey)
                .Member(dest => dest.Name, src => src.Options.Name)
                .Member(dest => dest.HasPassword, src => !string.IsNullOrWhiteSpace(src.Options.Password))
                .Member(dest => dest.TimeLimit, src => src.Options.TimeLimit.TotalMilliseconds)
                .Member(dest => dest.ScoreLimit, src => src.Options.ScoreLimit)

                .Member(dest => dest.IsFriendly, src => src.Options.IsFriendly)
                .Member(dest => dest.IsBalanced, src => src.Options.IsBalanced)
                .Member(dest => dest.MinLevel, src => src.Options.MinLevel)
                .Member(dest => dest.MaxLevel, src => src.Options.MaxLevel)
                .Member(dest => dest.EquipLimit, src => src.Options.ItemLimit)
                .Member(dest => dest.IsNoIntrusion, src => src.Options.IsNoIntrusion)

                .Member(dest => dest.ConnectingCount, src => src.TeamManager.Players.Count())
                .Member(dest => dest.PlayerCount, src => src.TeamManager.Players.Count())
                .Member(dest => dest.Latency, src => src.GetLatency())
                .Function(dest => dest.State, src =>
                {
                    if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Waiting))
                        return GameState.Waiting;

                    if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Result))
                        return GameState.Result;

                    if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Playing))
                        return GameState.Playing;

                    throw new InvalidOperationException();
                });

            Mapper.Register<Room, EnterRoomInfoDto>()
                .Member(dest => dest.RoomId, src => src.Id)
                .Member(dest => dest.MatchKey, src => src.Options.MatchKey)
                .Member(dest => dest.TimeLimit, src => src.Options.TimeLimit.TotalMilliseconds)
                .Member(dest => dest.TimeSync, src => src.GameRuleManager.GameRule.RoundTime.TotalMilliseconds)
                .Member(dest => dest.ScoreLimit, src => src.Options.ScoreLimit)
                .Member(dest => dest.IsFriendly, src => src.Options.IsFriendly)
                .Member(dest => dest.IsBalanced, src => src.Options.IsBalanced)
                .Member(dest => dest.MinLevel, src => src.Options.MinLevel)
                .Member(dest => dest.MaxLevel, src => src.Options.MaxLevel)
                .Member(dest => dest.ItemLimit, src => src.Options.ItemLimit)
                .Member(dest => dest.IsNoIntrusion, src => src.Options.IsNoIntrusion)
                .Member(dest => dest.RelayEndPoint, src => src.Options.ServerEndPoint)
                .Function(dest => dest.State, src =>
                {
                    if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Waiting))
                        return GameState.Waiting;

                    if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Result))
                        return GameState.Result;

                    if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Playing))
                        return GameState.Playing;

                    throw new InvalidOperationException();
                })
                .Function(dest => dest.TimeState, src =>
                {
                    //if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.FirstHalf))
                    //    return GameTimeState.FirstHalf;

                    if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.HalfTime))
                        return GameTimeState.HalfTime;

                    if (src.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.SecondHalf))
                        return GameTimeState.SecondHalf;

                    return GameTimeState.FirstHalf;
                });

            Mapper.Register<Player, RoomPlayerDto>()
                .Member(dest => dest.AccountId, src => src.Account.Id)
                .Member(dest => dest.Nickname, src => src.Account.Nickname)
                .Value(dest => dest.Unk1, (byte)144);

            //Mapper.Register<PlayerItem, Netsphere.Network.Data.P2P.ItemDto>()
            //    .Function(dest => dest.ItemNumber, src => src?.ItemNumber ?? 0);

            Mapper.Register<RoomCreationOptions, ChangeRuleDto>();

            Mapper.Register<Mail, NoteDto>()
                .Function(dest => dest.ReadCount, src => src.IsNew ? 0 : 1)
                .Function(dest => dest.DaysLeft,
                    src => DateTimeOffset.Now < src.Expires ? (src.Expires - DateTimeOffset.Now).TotalDays : 0);

            Mapper.Register<Mail, NoteContentDto>();

            Mapper.Register<PlayerItem, ItemDurabilityInfoDto>()
                .Member(dest => dest.ItemId, src => src.Id);


            Mapper.Register<Player, UserDataDto>()
                .Member(dest => dest.AccountId, src => src.Account.Id)
                .Member(dest => dest.ServerId, src => Config.Instance.Id)
                .Function(dest => dest.ChannelId, src => src.Channel != null ? (short)src.Channel.Id : (short)-1)
                .Function(dest => dest.RoomId, src => src.Room?.Id ?? 0xFFFFFFFF) // ToDo: Tutorial, License
                .Function(dest => dest.Team, src => src.RoomInfo?.Team?.Team ?? Team.Neutral)
                .Function(dest => dest.TotalExp, src => src.TotalExperience);

            Mapper.Register<Player, UserDataWithNickDto>()
                .Member(dest => dest.AccountId, src => src.Account.Id)
                .Member(dest => dest.Nickname, src => src.Account.Nickname)
                .Function(dest => dest.Data, src => src.Map<Player, UserDataDto>());

            Mapper.Compile(CompilationTypes.Source);
        }
    }
}
