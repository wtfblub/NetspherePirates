﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using BlubLib.Security.Cryptography;
using Dapper.FastCrud;
using ExpressMapper.Extensions;
using Netsphere.Database.Auth;
using Netsphere.Database.Game;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using NLog;
using NLog.Fluent;
using ProudNet.Handlers;
using CLoginReqMessage = Netsphere.Network.Message.Game.CLoginReqMessage;
using SLoginAckMessage = Netsphere.Network.Message.Game.SLoginAckMessage;

namespace Netsphere.Network.Services
{
    internal class AuthService : ProudMessageHandler
    {
        private static readonly Version s_version = new Version(0, 8, 31, 18574);
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [MessageHandler(typeof(CLoginReqMessage))]
        public async Task LoginHandler(GameSession session, CLoginReqMessage message)
        {
            Logger.Info()
                .Account(message.AccountId, message.Username)
                .Message($"Login from {session.RemoteEndPoint}")
                .Write();

            if (message.Version != s_version)
            {
                Logger.Error()
                    .Account(message.AccountId, message.Username)
                    .Message($"Invalid client version {message.Version}")
                    .Write();

                session.SendAsync(new SLoginAckMessage(GameLoginResult.WrongVersion));
                return;
            }

            if (GameServer.Instance.PlayerManager.Count >= Config.Instance.PlayerLimit)
            {
                Logger.Error()
                    .Account(message.AccountId, message.Username)
                    .Message("Server is full")
                    .Write();

                session.SendAsync(new SLoginAckMessage(GameLoginResult.ServerFull));
                return;
            }

            #region Validate Login

            AccountDto accountDto;
            using (var db = AuthDatabase.Open())
            {
                accountDto = (await db.FindAsync<AccountDto>(statement => statement
                            .Include<BanDto>(join => join.LeftOuterJoin())
                            .Where($"{nameof(AccountDto.Id):C} = @Id")
                            .WithParameters(new { Id = message.AccountId })))
                    .FirstOrDefault();
            }

            if (accountDto == null)
            {
                Logger.Error()
                    .Account(message.AccountId, message.Username)
                    .Message("Wrong login")
                    .Write();

                session.SendAsync(new SLoginAckMessage(GameLoginResult.SessionTimeout));
                return;
            }

            uint inputSessionId;
            if (!uint.TryParse(message.SessionId, out inputSessionId))
            {
                Logger.Error()
                    .Account(message.AccountId, message.Username)
                    .Message("Wrong login")
                    .Write();

                session.SendAsync(new SLoginAckMessage(GameLoginResult.SessionTimeout));
                return;
            }

            var sessionId = Hash.GetUInt32<CRC32>($"<{accountDto.Username}+{accountDto.Password}>");
            if (sessionId != inputSessionId)
            {
                Logger.Error()
                    .Account(message.AccountId, message.Username)
                    .Message("Wrong login")
                    .Write();

                session.SendAsync(new SLoginAckMessage(GameLoginResult.SessionTimeout));
                return;
            }

            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            var ban = accountDto.Bans.FirstOrDefault(b => b.Date + (b.Duration ?? 0) > now);
            if (ban != null)
            {
                var unbanDate = DateTimeOffset.FromUnixTimeSeconds(ban.Date + (ban.Duration ?? 0));
                Logger.Error()
                    .Account(message.AccountId, message.Username)
                    .Message($"Banned until {unbanDate}")
                    .Write();

                session.SendAsync(new SLoginAckMessage(GameLoginResult.SessionTimeout));
                return;
            }

            var account = new Account(accountDto);

            #endregion

            if (account.SecurityLevel < Config.Instance.SecurityLevel)
            {
                Logger.Error()
                    .Account(account)
                    .Message($"No permission to enter this server({Config.Instance.SecurityLevel} or above required)")
                    .Write();

                session.SendAsync(new SLoginAckMessage((GameLoginResult)9));
                return;
            }

            if (message.KickConnection)
            {
                Logger.Info()
                    .Account(account)
                    .Message("Kicking old connection")
                    .Write();

                var oldPlr = GameServer.Instance.PlayerManager.Get(account.Id);
                oldPlr?.Disconnect();
            }

            if (GameServer.Instance.PlayerManager.Contains(account.Id))
            {
                Logger.Error()
                    .Account(account)
                    .Message("Already online")
                    .Write();

                session.SendAsync(new SLoginAckMessage(GameLoginResult.TerminateOtherConnection));
                return;
            }

            using (var db = GameDatabase.Open())
            {
                var plrDto = (await db.FindAsync<PlayerDto>(statement => statement
                        .Include<PlayerCharacterDto>(join => join.LeftOuterJoin())
                        .Include<PlayerDenyDto>(join => join.LeftOuterJoin())
                        .Include<PlayerItemDto>(join => join.LeftOuterJoin())
                        .Include<PlayerLicenseDto>(join => join.LeftOuterJoin())
                        .Include<PlayerMailDto>(join => join.LeftOuterJoin())
                        .Include<PlayerSettingDto>(join => join.LeftOuterJoin())
                        .Where($"{nameof(PlayerDto.Id):C} = @Id")
                        .WithParameters(new { Id = message.AccountId })))
                .FirstOrDefault();
                if (plrDto == null)
                {
                    // first time connecting to this server
                    plrDto = new PlayerDto
                    {
                        Id = (int)account.Id,
                        Level = Config.Instance.Game.StartLevel,
                        PEN = Config.Instance.Game.StartPEN,
                        AP = Config.Instance.Game.StartAP,
                        Coins1 = Config.Instance.Game.StartCoins1,
                        Coins2 = Config.Instance.Game.StartCoins2,
                        TotalExperience = (int)GameServer.Instance.ResourceCache.GetExperience().ToList()[Config.Instance.Game.StartLevel].Value.TotalExperience
                    };

                    await db.InsertAsync(plrDto);
                }

                session.Player = new Player(session, account, plrDto);
            }

            if (GameServer.Instance.PlayerManager.Contains(session.Player))
            {
                session.Player = null;
                Logger.Error()
                    .Account(account)
                    .Message("Already online")
                    .Write();

                session.SendAsync(new SLoginAckMessage(GameLoginResult.TerminateOtherConnection));
                return;
            }

            GameServer.Instance.PlayerManager.Add(session.Player);

            Logger.Info()
                .Account(account)
                .Message("Login success")
                .Write();

            var result = string.IsNullOrWhiteSpace(account.Nickname)
                ? GameLoginResult.ChooseNickname
                : GameLoginResult.OK;
            await session.SendAsync(new SLoginAckMessage(result, session.Player.Account.Id));

            if (!string.IsNullOrWhiteSpace(account.Nickname))
                await LoginAsync(session);
        }

        [MessageHandler(typeof(CCheckNickReqMessage))]
        public async Task CheckNickHandler(GameSession session, CCheckNickReqMessage message)
        {
            if (session.Player == null || !string.IsNullOrWhiteSpace(session.Player.Account.Nickname))
            {
                session.CloseAsync();
                return;
            }

            Logger.Info()
                .Account(session)
                .Message($"Checking nickname {message.Nickname}")
                .Write();

            var available = await IsNickAvailableAsync(message.Nickname);
            Logger.Error()
                .Account(session)
                .Message($"Nickname not available: {message.Nickname}")
                .WriteIf(!available);

            session.SendAsync(new SCheckNickAckMessage(!available));
        }

        [MessageHandler(typeof(CCreateNickReqMessage))]
        public async Task CreateNickHandler(GameSession session, CCreateNickReqMessage message)
        {
            if (session.Player == null || !string.IsNullOrWhiteSpace(session.Player.Account.Nickname))
            {
                session.CloseAsync();
                return;
            }

            Logger.Info()
                .Account(session)
                .Message($"Creating nickname {message.Nickname}")
                .Write();

            if (!await IsNickAvailableAsync(message.Nickname))
            {
                Logger.Error()
                    .Account(session)
                    .Message($"Nickname not available: {message.Nickname}")
                    .Write();

                session.SendAsync(new SCheckNickAckMessage(false));
                return;
            }

            session.Player.Account.Nickname = message.Nickname;
            using (var db = AuthDatabase.Open())
            {
                var mapping = OrmConfiguration
                    .GetDefaultEntityMapping<AccountDto>()
                    .Clone()
                    .UpdatePropertiesExcluding(prop => prop.IsExcludedFromUpdates = true, nameof(AccountDto.Nickname));

                await db.UpdateAsync(new AccountDto { Id = (int)session.Player.Account.Id, Nickname = message.Nickname },
                            statement => statement.WithEntityMappingOverride(mapping));

            }
            //session.Send(new SCreateNickAckMessage { Nickname = msg.Nickname });
            await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.CreateNicknameSuccess));

            Logger.Info()
                .Account(session)
                .Message($"Created nickname {message.Nickname}")
                .Write();

            await LoginAsync(session);
        }

        private static async Task LoginAsync(GameSession session)
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

            await session.SendAsync(new SMyLicenseInfoAckMessage(licenses));
            await session.SendAsync(new SInventoryInfoAckMessage
            {
                Items = plr.Inventory.Select(i => i.Map<PlayerItem, ItemDto>()).ToArray()
            });

            // Todo random shop
            await session.SendAsync(new SRandomShopChanceInfoAckMessage { Progress = 10000 });
            await session.SendAsync(new SCharacterSlotInfoAckMessage
            {
                ActiveCharacter = plr.CharacterManager.CurrentSlot,
                CharacterCount = (byte)plr.CharacterManager.Count,
                MaxSlots = 3
            });

            foreach (var @char in plr.CharacterManager)
            {
                await session.SendAsync(new SOpenCharacterInfoAckMessage
                {
                    Slot = @char.Slot,
                    Style = new CharacterStyle(@char.Gender, @char.Hair.Variation, @char.Face.Variation, @char.Shirt.Variation, @char.Pants.Variation, @char.Slot)
                });


                var message = new SCharacterEquipInfoAckMessage
                {
                    Slot = @char.Slot,
                    Weapons = @char.Weapons.GetItems().Select(i => i?.Id ?? 0).ToArray(),
                    Skills = new[] { @char.Skills.GetItem(SkillSlot.Skill)?.Id ?? 0 },
                    Clothes = @char.Costumes.GetItems().Select(i => i?.Id ?? 0).ToArray(),
                };
                //var weapons = @char.Weapons.GetItems().Select(i => i?.Id ?? 0).ToArray();
                //Array.Copy(weapons, 0, message.Weapons, 6, weapons.Length);

                await session.SendAsync(message);
            }

            await session.SendAsync(new SRefreshCashInfoAckMessage { PEN = plr.PEN, AP = plr.AP });
            await session.SendAsync(new SSetCoinAckMessage { ArcadeCoins = plr.Coins1, BuffCoins = plr.Coins2 });
            await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.WelcomeToS4World));
            await session.SendAsync(new SBeginAccountInfoAckMessage
            {
                Level = plr.Level,
                TotalExp = plr.TotalExperience,
                AP = plr.AP,
                PEN = plr.PEN,
                TutorialState = (uint)(Config.Instance.Game.EnableTutorial ? plr.TutorialState : 2),
                Nickname = plr.Account.Nickname
            });

            await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.WelcomeToS4World2));

            if (plr.Inventory.Count == 0)
            {
                IEnumerable<StartItemDto> startItems;
                using (var db = GameDatabase.Open())
                {
                    startItems = await db.FindAsync<StartItemDto>(statement => statement
                            .Where($"{nameof(StartItemDto.RequiredSecurityLevel):C} <= @{nameof(plr.Account.SecurityLevel)}")
                            .WithParameters(new { plr.Account.SecurityLevel }));
                }

                foreach (var startItem in startItems)
                {
                    var shop = GameServer.Instance.ResourceCache.GetShop();
                    var item = shop.Items.Values.First(group => group.GetItemInfo(startItem.ShopItemInfoId) != null);
                    var itemInfo = item.GetItemInfo(startItem.ShopItemInfoId);
                    var effect = itemInfo.EffectGroup.GetEffect(startItem.ShopEffectId);

                    if (itemInfo == null)
                    {
                        Logger.Warn($"Cant find ShopItemInfo for Start item {startItem.Id} - Forgot to reload the cache?");
                        continue;
                    }

                    var price = itemInfo.PriceGroup.GetPrice(startItem.ShopPriceId);
                    if (price == null)
                    {
                        Logger.Warn($"Cant find ShopPrice for Start item {startItem.Id} - Forgot to reload the cache?");
                        continue;
                    }

                    var color = startItem.Color;
                    if (color > item.ColorGroup)
                    {
                        Logger.Warn($"Start item {startItem.Id} has an invalid color {color}");
                        color = 0;
                    }

                    var count = startItem.Count;
                    if (count > 0 && item.ItemNumber.Category <= ItemCategory.Skill)
                    {
                        Logger.Warn($"Start item {startItem.Id} cant have stacks(quantity={count})");
                        count = 0;
                    }

                    if (count < 0)
                        count = 0;

                    plr.Inventory.Create(itemInfo, price, color, effect.Effect, (uint)count);
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
            using (var db = AuthDatabase.Open())
            {
                var nickExists = (await db.FindAsync<AccountDto>(statement => statement
                            .Where($"{nameof(AccountDto.Nickname):C} = @{nameof(nickname)}")
                            .WithParameters(new { nickname })))
                    .Any();

                var nickReserved = (await db.FindAsync<NicknameHistoryDto>(statement => statement
                            .Where($"{nameof(NicknameHistoryDto.Nickname):C} = @{nameof(nickname)} AND ({nameof(NicknameHistoryDto.ExpireDate):C} = -1 OR {nameof(NicknameHistoryDto.ExpireDate):C} > @{nameof(now)})")
                            .WithParameters(new { nickname, now })))
                    .Any();
                return !nickExists && !nickReserved;
            }
        }

        [MessageHandler(typeof(Message.Chat.CLoginReqMessage))]
        public async Task Chat_LoginHandler(ChatServer server, ChatSession session, Message.Chat.CLoginReqMessage message)
        {
            Logger.Info()
                .Account(message.AccountId, "")
                .Message($"Login from {session.RemoteEndPoint}")
                .Write();

            uint sessionId;
            if (!uint.TryParse(message.SessionId, out sessionId))
            {
                Logger.Error()
                    .Account(message.AccountId, "")
                    .Message("Invalid sessionId")
                    .Write();
                session.SendAsync(new Message.Chat.SLoginAckMessage(2));
                return;
            }

            var plr = GameServer.Instance.PlayerManager[message.AccountId];
            if (plr == null)
            {
                Logger.Error()
                    .Account(message.AccountId, "")
                    .Message("Login failed")
                    .Write();
                session.SendAsync(new Message.Chat.SLoginAckMessage(3));
                return;
            }

            if (plr.ChatSession != null)
            {
                Logger.Error()
                    .Account(session)
                    .Message("Already online")
                    .Write();
                session.SendAsync(new Message.Chat.SLoginAckMessage(4));
                return;
            }

            session.GameSession = plr.Session;
            plr.ChatSession = session;

            Logger.Info()
                .Account(session)
                .Message("Login success")
                .Write();
            session.SendAsync(new Message.Chat.SLoginAckMessage(0));
            session.SendAsync(new Message.Chat.SDenyChatListAckMessage(plr.DenyManager.Select(d => d.Map<Deny, DenyDto>()).ToArray()));
        }

        [MessageHandler(typeof(Message.Relay.CRequestLoginMessage))]
        public async Task Relay_LoginHandler(RelayServer server, RelaySession session, Message.Relay.CRequestLoginMessage message)
        {
            var ip = session.RemoteEndPoint;
            Logger.Info()
                .Account(message.AccountId, "")
                .Message($"Login from {ip}")
                .Write();

            var plr = GameServer.Instance.PlayerManager[message.AccountId];
            if (plr == null)
            {
                Logger.Error()
                    .Account(message.AccountId, "")
                    .Message("Login failed")
                    .Write();
                session.SendAsync(new Message.Relay.SNotifyLoginResultMessage(1));
                return;
            }

            if (plr.RelaySession != null)
            {
                Logger.Error()
                    .Account(session)
                    .Message("Already online")
                    .Write();
                session.SendAsync(new Message.Relay.SNotifyLoginResultMessage(2));
                return;
            }

            var gameIp = plr.Session.RemoteEndPoint;
            if (!gameIp.Address.Equals(ip.Address))
            {
                Logger.Error()
                    .Account(message.AccountId, "")
                    .Message("Suspicious login")
                    .Write();
                session.SendAsync(new Message.Relay.SNotifyLoginResultMessage(3));
                return;
            }

            if (plr.Room == null || plr.Room?.Id != message.RoomLocation.RoomId)
            {
                Logger.Error()
                    .Account(message.AccountId, "")
                    .Message("Suspicious login(Not in a room/Invalid room id)")
                    .Write();
                session.SendAsync(new Message.Relay.SNotifyLoginResultMessage(4));
                return;
            }

            session.GameSession = plr.Session;
            plr.RelaySession = session;

            Logger.Info()
                .Account(session)
                .Message("Login success")
                .Write();

            await session.SendAsync(new Message.Relay.SEnterLoginPlayerMessage(plr.RoomInfo.Slot, plr.Account.Id, plr.Account.Nickname));
            foreach (var p in plr.Room.Players.Values.Where(p => p.RelaySession?.P2PGroup != null && p.Account.Id != plr.Account.Id))
            {
                await p.RelaySession.SendAsync(new Message.Relay.SEnterLoginPlayerMessage(plr.RoomInfo.Slot, plr.Account.Id, plr.Account.Nickname));
                await session.SendAsync(new Message.Relay.SEnterLoginPlayerMessage(p.RoomInfo.Slot, p.Account.Id, p.Account.Nickname));
            }

            plr.Room.Group.Join(session.HostId);
            await session.SendAsync(new Message.Relay.SNotifyLoginResultMessage(0));

            plr.RoomInfo.IsConnecting = false;
            plr.Room.OnPlayerJoined(new RoomPlayerEventArgs(plr));
        }
    }
}
