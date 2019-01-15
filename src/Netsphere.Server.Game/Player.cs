using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using LinqToDB;
using Logging;
using Microsoft.Extensions.Options;
using Netsphere.Common;
using Netsphere.Common.Configuration;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;
using Netsphere.Network;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game
{
    public class Player : DatabaseObject, ISaveable
    {
        private static readonly uint[] s_licensesCompleted;

        private ILogger _logger;
        private readonly GameOptions _gameOptions;
        private readonly IDatabaseProvider _databaseProvider;
        private readonly GameDataService _gameDataService;
        private byte _tutorialState;
        private uint _totalExperience;
        private uint _pen;
        private uint _ap;
        private uint _coins1;
        private uint _coins2;
        private PlayerState _state;

        public Session Session { get; private set; }
        public Account Account { get; private set; }
        public CharacterManager CharacterManager { get; }
        public LicenseManager LicenseManager { get; }
        public PlayerInventory Inventory { get; }
        public byte TutorialState
        {
            get => _tutorialState;
            set => SetIfChanged(ref _tutorialState, value);
        }
        public uint TotalExperience
        {
            get => _totalExperience;
            set => SetIfChanged(ref _totalExperience, value);
        }
        public uint PEN
        {
            get => _pen;
            set => SetIfChanged(ref _pen, value);
        }
        public uint AP
        {
            get => _ap;
            set => SetIfChanged(ref _ap, value);
        }
        public uint Coins1
        {
            get => _coins1;
            set => SetIfChanged(ref _coins1, value);
        }
        public uint Coins2
        {
            get => _coins2;
            set => SetIfChanged(ref _coins2, value);
        }
        public Channel Channel { get; internal set; }
        public int Level => _gameDataService.GetLevelFromExperience(_totalExperience).Level;

        public Room Room { get; internal set; }
        public byte Slot { get; internal set; }
        public PlayerState State
        {
            get => _state;
            internal set
            {
                if (_state == value)
                    return;

                _state = value;
                OnStateChanged();
            }
        }
        public PlayerGameMode Mode { get; internal set; }
        public bool IsConnectingToRoom { get; internal set; }
        public bool IsReady { get; internal set; }
        public Team Team { get; internal set; }
        public PlayerScore Score { get; internal set; }
        public LongPeerId PeerId { get; internal set; }
        public DateTimeOffset StartPlayTime { get; internal set; }
        public DateTimeOffset[] CharacterStartPlayTime { get; internal set; }

        public event EventHandler<PlayerEventArgs> Disconnected;
        public event EventHandler<PlayerEventArgs> StateChanged;
        public event EventHandler<NicknameEventArgs> NicknameCreated;

        internal void OnDisconnected()
        {
            Room?.Leave(this);
            Channel?.Leave(this);

            Disconnected?.Invoke(this, new PlayerEventArgs(this));
        }

        protected virtual void OnStateChanged()
        {
            StateChanged?.Invoke(this, new PlayerEventArgs(this));
        }

        protected internal virtual void OnNicknameCreated(string nickname)
        {
            NicknameCreated?.Invoke(this, new NicknameEventArgs(this, nickname));
        }

        static Player()
        {
            s_licensesCompleted = new uint[100];
            for (uint i = 0; i < 100; ++i)
                s_licensesCompleted[i] = i;
        }

        public Player(ILogger<Player> logger, IOptions<GameOptions> gameOptions, IDatabaseProvider databaseProvider,
            GameDataService gameDataService,
            CharacterManager characterManager, LicenseManager licenseManager, PlayerInventory inventory)
        {
            _logger = logger;
            _gameOptions = gameOptions.Value;
            _databaseProvider = databaseProvider;
            _gameDataService = gameDataService;
            CharacterManager = characterManager;
            LicenseManager = licenseManager;
            Inventory = inventory;
            CharacterStartPlayTime = new DateTimeOffset[3];
        }

        internal void Initialize(Session session, Account account, PlayerEntity entity)
        {
            Session = session;
            Account = account;
            _logger = AddContextToLogger(_logger);
            _tutorialState = entity.TutorialState;
            _totalExperience = (uint)entity.TotalExperience;
            _pen = (uint)entity.PEN;
            _ap = (uint)entity.AP;
            _coins1 = (uint)entity.Coins1;
            _coins2 = (uint)entity.Coins2;
            LicenseManager.Initialize(this, entity.Licenses);
            Inventory.Initialize(this, entity);
            CharacterManager.Initialize(this, entity);
        }

        public void Disconnect()
        {
            var _ = DisconnectAsync();
        }

        public Task DisconnectAsync()
        {
            return Session.CloseAsync();
        }

        /// <summary>
        /// Gains experiences and levels up if the player earned enough experience
        /// </summary>
        /// <param name="amount">Amount of experience to earn</param>
        /// <returns>true if the player leveled up</returns>
        public bool GainExperience(uint amount)
        {
            _logger.Debug("Gained {Amount} experience", amount);

            var levels = _gameDataService.Levels;
            var levelInfo = levels.GetValueOrDefault(Level);
            if (levelInfo == null)
            {
                _logger.Warning("Level={Level} not found", Level);
                return false;
            }

            // We cant earn experience when we reached max level
            if (levelInfo.ExperienceToNextLevel == 0 || Level >= _gameOptions.MaxLevel)
                return false;

            var leveledUp = false;
            TotalExperience += amount;

            // Did we level up?
            // Using a loop for multiple level ups
            while (levelInfo.ExperienceToNextLevel != 0 &&
                   levelInfo.ExperienceToNextLevel <= (int)(TotalExperience - levelInfo.TotalExperience))
            {
                var newLevel = Level + 1;
                levelInfo = levels.GetValueOrDefault(newLevel);

                if (levelInfo == null)
                {
                    _logger.Warning("Can't level up because level={Level} not found", newLevel);
                    break;
                }

                _logger.Debug("Leveled up to {Level}", newLevel);

                // TODO level rewards
                leveledUp = true;
            }

            if (!leveledUp)
                return false;

            // TODO Update chat server
            // TODO Do we need this?
            // Session.Send(new SBeginAccountInfoAckMessage())

            return true;
        }

        public TimeSpan GetCurrentPlayTime()
        {
            return DateTimeOffset.Now - StartPlayTime;
        }

        public TimeSpan GetCharacterPlayTime(byte slot)
        {
            if (slot >= CharacterStartPlayTime.Length)
                return default;

            return DateTimeOffset.Now - CharacterStartPlayTime[slot];
        }

        /// <summary>
        /// Gets the maximum hp for the current character
        /// </summary>
        public float GetMaxHP()
        {
            return _gameDataService.GameTempos["GAMETEMPO_FREE"].ActorDefaultHPMax +
                   GetAttributeValue(Attribute.HP);
        }

        /// <summary>
        /// Gets the total attribute value for the current character
        /// </summary>
        /// <param name="attribute">The attribute to retrieve</param>
        /// <returns></returns>
        public float GetAttributeValue(Attribute attribute)
        {
            if (CharacterManager.CurrentCharacter == null)
                return 0;

            var character = CharacterManager.CurrentCharacter;
            var value = GetAttributeValueFromItems(attribute, character.Weapons.GetItems());
            value += GetAttributeValueFromItems(attribute, character.Skills.GetItems());
            value += GetAttributeValueFromItems(attribute, character.Costumes.GetItems());

            return value;
        }

        /// <summary>
        /// Gets the total attribute rate for the current character
        /// </summary>
        /// <param name="attribute">The attribute to retrieve</param>
        /// <returns></returns>
        public float GetAttributeRate(Attribute attribute)
        {
            if (CharacterManager.CurrentCharacter == null)
                return 0;

            var character = CharacterManager.CurrentCharacter;
            var value = GetAttributeRateFromItems(attribute, character.Weapons.GetItems());
            value += GetAttributeRateFromItems(attribute, character.Skills.GetItems());
            value += GetAttributeRateFromItems(attribute, character.Costumes.GetItems());

            return value;
        }

        public async Task SendAccountInformation()
        {
            var licenses = _gameOptions.EnableLicenseRequirement
                ? LicenseManager.Select(x => (uint)x.ItemLicense).ToArray()
                : s_licensesCompleted;

            await Session.SendAsync(new SMyLicenseInfoAckMessage(licenses));
            await Session.SendAsync(new SInventoryInfoAckMessage
            {
                Items = Inventory.Select(x => x.Map<PlayerItem, ItemDto>()).ToArray()
            });

            await Session.SendAsync(new SCharacterSlotInfoAckMessage
            {
                ActiveCharacter = CharacterManager.CurrentSlot,
                CharacterCount = (byte)CharacterManager.Count,
                MaxSlots = 3
            });

            foreach (var character in CharacterManager)
            {
                await Session.SendAsync(new SOpenCharacterInfoAckMessage
                {
                    Slot = character.Slot,
                    Style = new CharacterStyle(character.Gender, character.Slot,
                        character.Hair.Variation, character.Face.Variation,
                        character.Shirt.Variation, character.Pants.Variation)
                });

                var message = new SCharacterEquipInfoAckMessage
                {
                    Slot = character.Slot,
                    Weapons = character.Weapons.GetItems().Select(x => x?.Id ?? 0).ToArray(),
                    Skills = new[] { character.Skills.GetItem(0).Item1?.Id ?? 0 },
                    Clothes = character.Costumes.GetItems().Select(x => x?.Id ?? 0).ToArray()
                };

                await Session.SendAsync(message);
            }

            await Session.SendAsync(new SRefreshCashInfoAckMessage(PEN, AP));
            await Session.SendAsync(new SSetCoinAckMessage(Coins1, Coins2));
            await Session.SendAsync(new SServerResultInfoAckMessage(ServerResult.WelcomeToS4World));
            await Session.SendAsync(new SBeginAccountInfoAckMessage
            {
                Level = (byte)Level,
                TotalExp = TotalExperience,
                AP = AP,
                PEN = PEN,
                TutorialState = (uint)(_gameOptions.EnableTutorial ? TutorialState : 2),
                Nickname = Account.Nickname
            });

            await Session.SendAsync(new SServerResultInfoAckMessage(ServerResult.WelcomeToS4World2));

            if (Inventory.Count == 0)
            {
                IEnumerable<StartItemEntity> startItems;
                using (var db = _databaseProvider.Open<GameContext>())
                {
                    var securityLevel = (byte)Account.SecurityLevel;
                    startItems = await db.StartItems.Where(x => x.RequiredSecurityLevel <= securityLevel).ToArrayAsync();
                }

                foreach (var startItem in startItems)
                {
                    var item = _gameDataService.ShopItems.Values.First(group =>
                        group.GetItemInfo(startItem.ShopItemInfoId) != null);
                    var itemInfo = item.GetItemInfo(startItem.ShopItemInfoId);
                    var effect = itemInfo.EffectGroup.GetEffect(startItem.ShopEffectId);

                    if (itemInfo == null)
                    {
                        _logger.Warning("Cant find ShopItemInfo for Start item {startItemId} - Forgot to reload the cache?",
                            startItem.Id);
                        continue;
                    }

                    var price = itemInfo.PriceGroup.GetPrice(startItem.ShopPriceId);
                    if (price == null)
                    {
                        _logger.Warning("Cant find ShopPrice for Start item {startItemId} - Forgot to reload the cache?",
                            startItem.Id);
                        continue;
                    }

                    var color = startItem.Color;
                    if (color > item.ColorGroup)
                    {
                        _logger.Warning("Start item {startItemId} has an invalid color {color}", startItem.Id, color);
                        color = 0;
                    }

                    var count = startItem.Count;
                    if (count > 0 && item.ItemNumber.Category <= ItemCategory.Skill)
                    {
                        _logger.Warning("Start item {startItemId} cant have stacks(quantity={count})", startItem.Id, count);
                        count = 0;
                    }

                    if (count < 0)
                        count = 0;

                    Inventory.Create(itemInfo, price, color, effect.Effect, (uint)count);
                }
            }
        }

        public Task SendMoneyUpdate()
        {
            return Session.SendAsync(new SRefreshCashInfoAckMessage(PEN, AP));
        }

        /// <summary>
        /// Sends a message to the game master console
        /// </summary>
        /// <param name="message">The message to send</param>
        public Task SendConsoleMessage(string message)
        {
            return Session.SendAsync(new SAdminActionAckMessage(message));
        }

        /// <summary>
        /// Sends a notice message
        /// </summary>
        /// <param name="message">The message to send</param>
        public Task SendNotice(string message)
        {
            return Session.SendAsync(new SNoticeMessageAckMessage(message));
        }

        public async Task Save(GameContext db)
        {
            if (IsDirty)
            {
                await db.UpdateAsync(new PlayerEntity
                {
                    Id = (long)Account.Id,
                    TutorialState = TutorialState,
                    TotalExperience = (int)TotalExperience,
                    PEN = (int)PEN,
                    AP = (int)AP,
                    Coins1 = (int)Coins1,
                    Coins2 = (int)Coins2,
                    CurrentCharacterSlot = CharacterManager.CurrentSlot
                });

                SetDirtyState(false);
            }

            await LicenseManager.Save(db);
            await Inventory.Save(db);
            await CharacterManager.Save(db);
        }

        public ILogger AddContextToLogger(ILogger logger)
        {
            return logger.ForContext(
                ("AccountId", Account.Id),
                ("HostId", Session.HostId),
                ("EndPoint", Session.RemoteEndPoint.ToString()));
        }

        private static float GetAttributeValueFromItems(Attribute attribute, IEnumerable<PlayerItem> items)
        {
            return items.Where(item => item != null)
                .Select(item => item.GetItemEffect())
                .Where(effect => effect != null)
                .SelectMany(effect => effect.Attributes)
                .Where(attrib => attrib.Attribute == attribute)
                .Sum(attrib => attrib.Value);
        }

        private static float GetAttributeRateFromItems(Attribute attribute, IEnumerable<PlayerItem> items)
        {
            return items.Where(item => item != null)
                .Select(item => item.GetItemEffect())
                .Where(effect => effect != null)
                .SelectMany(effect => effect.Attributes)
                .Where(attrib => attrib.Attribute == attribute)
                .Sum(attrib => attrib.Rate);
        }
    }
}
