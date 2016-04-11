using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BlubLib.Network.Pipes;
using BlubLib.Network.Transport.Sockets;
using BlubLib.Security.Cryptography;
using ExpressMapper.Extensions;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using NLog;
using NLog.Fluent;
using Shaolinq;
using CLoginReqMessage = Netsphere.Network.Message.Game.CLoginReqMessage;
using SLoginAckMessage = Netsphere.Network.Message.Game.SLoginAckMessage;

namespace Netsphere.Network.Services
{
    internal class AuthService : Service
    {
        private static readonly Version Version = new Version(0, 8, 31, 18574);
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [MessageHandler(typeof(CLoginReqMessage))]
        public async Task LoginHandler(GameServer server, GameSession session, CLoginReqMessage message)
        {
            Logger.Info()
                .Account(message.AccountId, message.Username)
                .Message("Login from {0}", ((TcpProcessor)session.Processor).Socket.RemoteEndPoint)
                .Write();

            if (message.Version != Version)
            {
                Logger.Error()
                    .Account(message.AccountId, message.Username)
                    .Message("Invalid client version {0}", message.Version)
                    .Write();

                await session.SendAsync(new SLoginAckMessage { Result = GameLoginResult.WrongVersion })
                    .ConfigureAwait(false);
                return;
            }

            if (server.PlayerManager.Count >= Config.Instance.PlayerLimit)
            {
                Logger.Error()
                    .Account(message.AccountId, message.Username)
                    .Message("Server is full")
                    .Write();

                await session.SendAsync(new SLoginAckMessage { Result = GameLoginResult.ServerFull })
                    .ConfigureAwait(false);
                return;
            }

            Account account;

            #region Validate Login

            var accountDto = AuthDatabase.Instance.Accounts.GetByPrimaryKey((int)message.AccountId);
            if (accountDto == null)
            {
                Logger.Error()
                    .Account(message.AccountId, message.Username)
                    .Message("Wrong login")
                    .Write();

                await session.SendAsync(new SLoginAckMessage { Result = GameLoginResult.SessionTimeout })
                    .ConfigureAwait(false);
                return;
            }

            uint inputSessionId;
            if (!uint.TryParse(message.SessionId, out inputSessionId))
            {
                Logger.Error()
                    .Account(message.AccountId, message.Username)
                    .Message("Wrong login")
                    .Write();

                await session.SendAsync(new SLoginAckMessage { Result = GameLoginResult.SessionTimeout })
                    .ConfigureAwait(false);
                return;
            }

            var sessionId = Hash.GetUInt32<CRC32>($"<{accountDto.Username}+{accountDto.Password}>");
            if (sessionId != inputSessionId)
            {
                Logger.Error()
                    .Account(message.AccountId, message.Username)
                    .Message("Wrong login")
                    .Write();

                await session.SendAsync(new SLoginAckMessage { Result = GameLoginResult.SessionTimeout })
                    .ConfigureAwait(false);
                return;
            }

            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            var ban = await accountDto.Bans.FirstOrDefaultAsync(b => b.Date + b.Duration > now)
                .ConfigureAwait(false);
            if (ban != null)
            {
                var unbanDate = DateTimeOffset.FromUnixTimeSeconds(ban.Date + ban.Duration);
                Logger.Error()
                    .Account(message.AccountId, message.Username)
                    .Message("Banned until {0}", unbanDate)
                    .Write();

                await session.SendAsync(new SLoginAckMessage { Result = GameLoginResult.SessionTimeout })
                    .ConfigureAwait(false);
                return;
            }

            account = new Account(accountDto);

            #endregion

            if (account.SecurityLevel < Config.Instance.SecurityLevel)
            {
                Logger.Error()
                    .Account(account)
                    .Message("No permission to enter this server({0} or above required)", Config.Instance.SecurityLevel)
                    .Write();

                await session.SendAsync(new SLoginAckMessage { Result = (GameLoginResult)9 })
                    .ConfigureAwait(false);
                return;
            }

            if (message.KickConnection)
            {
                Logger.Info()
                    .Account(account)
                    .Message("Kicking old connection")
                    .Write();

                var oldPlr = server.PlayerManager.Get(account.Id);
                oldPlr?.Disconnect();
            }

            if (server.PlayerManager.Contains(account.Id))
            {
                Logger.Error()
                    .Account(account)
                    .Message("Already online")
                    .Write();

                await session.SendAsync(new SLoginAckMessage { Result = GameLoginResult.TerminateOtherConnection })
                    .ConfigureAwait(false);
                return;
            }

            using (var scope = new DataAccessScope())
            {
                var plrDto = await GameDatabase.Instance.Players
                    .FirstOrDefaultAsync(p => p.Id == (int)account.Id)
                    .ConfigureAwait(false);
                if (plrDto == null)
                {
                    // first time connecting to this server
                    plrDto = GameDatabase.Instance.Players.Create((int)account.Id);
                    plrDto.Level = Config.Instance.Game.StartLevel;
                    plrDto.PEN = Config.Instance.Game.StartPEN;
                    plrDto.AP = Config.Instance.Game.StartAP;
                    plrDto.Coins1 = Config.Instance.Game.StartCoins1;
                    plrDto.Coins2 = Config.Instance.Game.StartCoins2;

                    await scope.CompleteAsync()
                        .ConfigureAwait(false);
                }

                session.Player = new Player(session, account, plrDto);
            }

            if (server.PlayerManager.Contains(session.Player))
            {
                session.Player = null;
                Logger.Error()
                    .Account(account)
                    .Message("Already online")
                    .Write();

                await session.SendAsync(new SLoginAckMessage { Result = GameLoginResult.TerminateOtherConnection })
                    .ConfigureAwait(false);
                return;
            }

            server.PlayerManager.Add(session.Player);

            Logger.Info()
                .Account(account)
                .Message("Login success")
                .Write();

            await session.SendAsync(new SLoginAckMessage
            {
                Result = string.IsNullOrWhiteSpace(account.Nickname) ? GameLoginResult.ChooseNickname : GameLoginResult.OK,
                AccountId = session.Player.Account.Id
            }).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(account.Nickname))
                await LoginAsync(server, session).ConfigureAwait(false);
        }

        [MessageHandler(typeof(CCheckNickReqMessage))]
        public async void CheckNickHandler(GameSession session, CCheckNickReqMessage message)
        {
            if (session.Player == null || !string.IsNullOrWhiteSpace(session.Player.Account.Nickname))
            {
                session.Close();
                return;
            }

            Logger.Info()
                .Account(session)
                .Message("Checking nickname {0}", message.Nickname)
                .Write();

            var available = await IsNickAvailableAsync(message.Nickname).ConfigureAwait(false);
            Logger.Error()
                .Account(session)
                .Message("Nickname not available: {0}", message.Nickname)
                .WriteIf(!available);

            await session.SendAsync(new SCheckNickAckMessage { IsAvailable = available })
                .ConfigureAwait(false);
        }

        [MessageHandler(typeof(CCreateNickReqMessage))]
        public async Task CreateNickHandler(GameServer server, GameSession session, CCreateNickReqMessage message)
        {
            if (session.Player == null || !string.IsNullOrWhiteSpace(session.Player.Account.Nickname))
            {
                session.Dispose();
                return;
            }

            Logger.Info()
                .Account(session)
                .Message("Creating nickname {0}", message.Nickname)
                .Write();

            if (!await IsNickAvailableAsync(message.Nickname).ConfigureAwait(false))
            {
                Logger.Error()
                    .Account(session)
                    .Message("Nickname not available: {0}", message.Nickname)
                    .Write();

                await session.SendAsync(new SCheckNickAckMessage(false))
                    .ConfigureAwait(false);
                return;
            }

            session.Player.Account.Nickname = message.Nickname;
            using (var scope = new DataAccessScope())
            {
                var accountDto = AuthDatabase.Instance.Accounts.GetReference((int)session.Player.Account.Id);
                //if (accountDto == null)
                //{
                //    Logger.Error()
                //        .Account(session)
                //        .Message("Account {0} not found", session.Player.Account.Id)
                //        .Write();

                //    await session.SendAsync(new SCheckNickAckMessage(false))
                //        .ConfigureAwait(false);
                //    return;
                //}
                accountDto.Nickname = message.Nickname;

                await scope.CompleteAsync()
                    .ConfigureAwait(false);
            }
            //session.Send(new SCreateNickAckMessage { Nickname = msg.Nickname });
            await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.CreateNicknameSuccess))
                .ConfigureAwait(false);

            Logger.Info()
                .Account(session)
                .Message("Created nickname {0}", message.Nickname)
                .Write();

            await LoginAsync(server, session)
                .ConfigureAwait(false);
        }

        private static async Task LoginAsync(GameServer server, GameSession session)
        {
            var plr = session.Player;

            uint[] licenses;
            if (Config.Instance.Game.EnableLicenseRequirement)
            {
                licenses = plr.LicenseManager.Select(l => (uint)l.ItemLicense).ToArray();
            }
            else
            {
                licenses = new uint[100];
                for (uint i = 0; i < 100; ++i)
                    licenses[i] = i;
            }

            await session.SendAsync(new SMyLicenseInfoAckMessage(licenses))
                .ConfigureAwait(false);

            await session.SendAsync(new SInventoryInfoAckMessage
            {
                Items = plr.Inventory.Select(i => i.Map<PlayerItem, ItemDto>()).ToArray()
            }).ConfigureAwait(false);

            // Todo random shop
            await session.SendAsync(new SRandomShopChanceInfoAckMessage { Progress = 10000 })
                .ConfigureAwait(false);

            await session.SendAsync(new SCharacterSlotInfoAckMessage
            {
                ActiveCharacter = plr.CharacterManager.CurrentSlot,
                CharacterCount = (byte)plr.CharacterManager.Count,
                MaxSlots = 3
            }).ConfigureAwait(false);

            foreach (var @char in plr.CharacterManager)
            {
                await session.SendAsync(new SOpenCharacterInfoAckMessage
                {
                    Slot = @char.Slot,
                    Style = new CharacterStyle(@char.Gender, @char.Hair.Variation, @char.Face.Variation, @char.Shirt.Variation, @char.Pants.Variation, @char.Slot)
                }).ConfigureAwait(false);


                var message = new SCharacterEquipInfoAckMessage
                {
                    Slot = @char.Slot,
                    Weapons = @char.Weapons.GetItems().Select(i => i?.Id ?? 0).ToArray(),
                    Skills = new[] { @char.Skills.GetItem(SkillSlot.Skill)?.Id ?? 0 },
                    Clothes = @char.Costumes.GetItems().Select(i => i?.Id ?? 0).ToArray(),
                };
                //var weapons = @char.Weapons.GetItems().Select(i => i?.Id ?? 0).ToArray();
                //Array.Copy(weapons, 0, message.Weapons, 6, weapons.Length);

                await session.SendAsync(message).ConfigureAwait(false);
            }

            await session.SendAsync(new SRefreshCashInfoAckMessage { PEN = plr.PEN, AP = plr.AP })
                .ConfigureAwait(false);

            await session.SendAsync(new SSetCoinAckMessage { ArcadeCoins = plr.Coins1, BuffCoins = plr.Coins2 })
                .ConfigureAwait(false);

            await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.WelcomeToS4World))
                .ConfigureAwait(false);

            await session.SendAsync(new SBeginAccountInfoAckMessage
            {
                Level = plr.Level,
                TotalExp = server.ResourceCache.GetExperience()[plr.Level].TotalExperience + plr.TotalExperience,
                AP = plr.AP,
                PEN = plr.PEN,
                TutorialState = (uint)(Config.Instance.Game.EnableTutorial ? plr.TutorialState : 3),
                Nickname = plr.Account.Nickname
            }).ConfigureAwait(false);

            await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.WelcomeToS4World2))
                .ConfigureAwait(false);

            if (plr.Inventory.Count == 0)
            {
                var startItems = await GameDatabase.Instance.StartItems
                    .Where(item => item.RequiredSecurityLevel <= (byte)plr.Account.SecurityLevel)
                    .ToReadOnlyCollectionAsync()
                    .ConfigureAwait(false);

                foreach (var startItem in startItems)
                {
                    var shop = GameServer.Instance.ResourceCache.GetShop();
                    var itemInfo = shop.GetItemInfo(startItem.ShopItemInfo.ShopItem.Id,
                        (ItemPriceType)startItem.ShopItemInfo.PriceGroup.PriceType);

                    if (itemInfo == null)
                    {
                        Logger.Warn()
                            .Message("Cant find ShopItemInfo for Start item {0} - Forgot to reload the cache?", startItem.Id)
                            .Write();

                        continue;
                    }

                    var price = itemInfo.PriceGroup.GetPrice(startItem.ShopItemInfo.Id);
                    if (price == null)
                    {
                        Logger.Warn()
                            .Message("Cant find ShopPrice for Start item {0} - Forgot to reload the cache?", startItem.Id)
                            .Write();

                        continue;
                    }

                    var color = startItem.Color;
                    if (color > itemInfo.ShopItem.ColorGroup)
                    {
                        Logger.Warn()
                            .Message("Start item {0} has an invalid color {1}", startItem.Id, color)
                            .Write();

                        color = 0;
                    }

                    var count = startItem.Count;
                    if (count > 0 && itemInfo.ShopItem.ItemNumber.Category <= ItemCategory.Skill)
                    {
                        Logger.Warn()
                            .Message("Start item {0} cant have stacks(quantity={1})", startItem.Id, count)
                            .Write();

                        count = 0;
                    }

                    if (count < 0)
                        count = 0;

                    plr.Inventory.Create(itemInfo, price, color, startItem.ShopEffect.Effect, (uint)count);
                }
            }

            //session.Send(new SEquipedBoostItemAckMessage());
            //session.Send(new SClearInvalidateItemAckMessage());
        }

        private static async Task<bool> IsNickAvailableAsync(string nickname)
        {
            var minLength = Config.Instance.Game.NickRestrictions.MinLength;
            var maxLength = Config.Instance.Game.NickRestrictions.MaxLength;
            var whitespace = Config.Instance.Game.NickRestrictions.WhitespaceAllowed;
            var ascii = Config.Instance.Game.NickRestrictions.AsciiOnly;

            if (string.IsNullOrWhiteSpace(nickname) || (!whitespace && nickname.Contains(" ")) ||
                nickname.Length < minLength || nickname.Length > maxLength ||
                (ascii && Encoding.UTF8.GetByteCount(nickname) != nickname.Length))
            {
                return false;
            }

            // check for repeating chars example: (AAAHello, HeLLLLo)
            var maxRepeat = Config.Instance.Game.NickRestrictions.MaxRepeat;
            if (maxRepeat > 0)
            {
                var counter = 1;
                var current = nickname[0];
                for (var i = 1; i < nickname.Length; i++)
                {
                    if (current != nickname[i])
                    {
                        if (counter > maxRepeat) return false;
                        counter = 0;
                        current = nickname[i];
                    }
                    counter++;
                }
            }

            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            var nickExists = await AuthDatabase.Instance.Accounts
                .AnyAsync(e => e.Nickname == nickname)
                .ConfigureAwait(false);

            var nickReserved = await AuthDatabase.Instance.NicknameHistory
                .AnyAsync(e => e.Nickname == nickname && (e.ExpireDate == -1 || e.ExpireDate > now))
                .ConfigureAwait(false);

            return !nickExists && !nickReserved;
        }

        [MessageHandler(typeof(Message.Chat.CLoginReqMessage))]
        public void Chat_LoginHandler(ChatServer server, ChatSession session, Message.Chat.CLoginReqMessage message)
        {
            Logger.Info()
                .Account(message.AccountId, "")
                .Message("Login from {0}", ((TcpProcessor)session.Processor).Socket.RemoteEndPoint)
                .Write();

            uint sessionId;
            if (!uint.TryParse(message.SessionId, out sessionId))
            {
                Logger.Error()
                    .Account(message.AccountId, "")
                    .Message("Invalid sessionId")
                    .Write();
                session.Send(new Message.Chat.SLoginAckMessage(2));
                return;
            }

            var plr = server.GameServer.PlayerManager[message.AccountId];
            if (plr == null)
            {
                Logger.Error()
                    .Account(message.AccountId, "")
                    .Message("Login failed")
                    .Write();
                session.Send(new Message.Chat.SLoginAckMessage(3));
                return;
            }

            if (plr.ChatSession != null)
            {
                Logger.Error()
                    .Account(session)
                    .Message("Already online")
                    .Write();
                session.Send(new Message.Chat.SLoginAckMessage(4));
                return;
            }

            session.GameSession = plr.Session;
            plr.ChatSession = session;

            Logger.Info()
                .Account(session)
                .Message("Login success")
                .Write();
            session.Send(new Message.Chat.SLoginAckMessage(0));
            session.Send(new Message.Chat.SDenyChatListAckMessage(plr.DenyManager.Select(d => d.Map<Deny, DenyDto>()).ToArray()));
        }

        [MessageHandler(typeof(Message.Relay.CRequestLoginMessage))]
        public void Relay_LoginHandler(RelayServer server, RelaySession session, Message.Relay.CRequestLoginMessage message)
        {
            var ip = (IPEndPoint)((TcpProcessor)session.Processor).Socket.RemoteEndPoint;
            Logger.Info()
                .Account(message.AccountId, "")
                .Message("Login from {0}", ip)
                .Write();

            var plr = server.GameServer.PlayerManager[message.AccountId];
            if (plr == null)
            {
                Logger.Error()
                    .Account(message.AccountId, "")
                    .Message("Login failed")
                    .Write();
                session.Send(new Message.Relay.SNotifyLoginResultMessage(1));
                return;
            }

            if (plr.RelaySession != null)
            {
                Logger.Error()
                    .Account(session)
                    .Message("Already online")
                    .Write();
                session.Send(new Message.Relay.SNotifyLoginResultMessage(2));
                return;
            }

            var gameIp = (IPEndPoint)((TcpProcessor)plr.Session.Processor).Socket.RemoteEndPoint;
            if (!gameIp.Address.Equals(ip.Address))
            {
                Logger.Error()
                    .Account(message.AccountId, "")
                    .Message("Suspicious login")
                    .Write();
                session.Send(new Message.Relay.SNotifyLoginResultMessage(3));
                return;
            }

            if (plr.Room == null || plr.Room?.Id != message.RoomLocation.RoomId)
            {
                Logger.Error()
                    .Account(message.AccountId, "")
                    .Message("Suspicious login(Not in a room/Invalid room id)")
                    .Write();
                session.Send(new Message.Relay.SNotifyLoginResultMessage(4));
                return;
            }

            session.GameSession = plr.Session;
            plr.RelaySession = session;

            Logger.Info().Account(session).Message("Login success").Write();

            session.Send(new Message.Relay.SEnterLoginPlayerMessage(plr.RoomInfo.Slot, plr.Account.Id, plr.Account.Nickname));
            foreach (var p in plr.Room.Players.Values.Where(p => p.RelaySession?.P2PGroup != null && p.Account.Id != plr.Account.Id))
            {
                p.RelaySession.Send(new Message.Relay.SEnterLoginPlayerMessage(plr.RoomInfo.Slot, plr.Account.Id, plr.Account.Nickname));
                session.Send(new Message.Relay.SEnterLoginPlayerMessage(p.RoomInfo.Slot, p.Account.Id, p.Account.Nickname));
            }

            plr.Room.Group.Join(session.HostId, true);
            session.Send(new Message.Relay.SNotifyLoginResultMessage(0));

            plr.RoomInfo.IsConnecting = false;
            plr.Room.OnPlayerJoined(new RoomPlayerEventArgs(plr));
        }
    }
}
