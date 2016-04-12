using System.Collections.Generic;
using System.Linq;
using ExpressMapper.Extensions;
using Netsphere.Database.Game;
using Netsphere.Network;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;
using Netsphere.Network.Message.Game;
using NLog;
using NLog.Fluent;
using Shaolinq;

namespace Netsphere
{
    internal class Player
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private byte _tutorialState;
        private byte _level;
        private uint _totalExperience;
        private uint _pen;
        private uint _ap;
        private uint _coins1;
        private uint _coins2;

        #region Properties

        internal bool NeedsToSave { get; set; }

        public GameSession Session { get; set; }
        public ChatSession ChatSession { get; set; }
        public RelaySession RelaySession { get; set; }

        public PlayerSettingManager Settings { get; }

        public DenyManager DenyManager { get; }
        public Mailbox Mailbox { get; }

        public Account Account { get; set; }
        public LicenseManager LicenseManager { get; }
        public CharacterManager CharacterManager { get; }
        public Inventory Inventory { get; }
        public Channel Channel { get; internal set; }

        public Room Room { get; internal set; }
        public PlayerRoomInfo RoomInfo { get; }

        internal bool SentPlayerList { get; set; }

        public byte TutorialState
        {
            get { return _tutorialState; }
            set
            {
                if (_tutorialState == value)
                    return;
                _tutorialState = value;
                NeedsToSave = true;
            }
        }
        public byte Level
        {
            get { return _level; }
            set
            {
                if (_level == value)
                    return;
                _level = value;
                NeedsToSave = true;
            }
        }
        public uint TotalExperience
        {
            get { return _totalExperience; }
            set
            {
                if (_totalExperience == value)
                    return;
                _totalExperience = value;
                NeedsToSave = true;
            }
        }
        public uint PEN
        {
            get { return _pen; }
            set
            {
                if (_pen == value)
                    return;
                _pen = value;
                NeedsToSave = true;
            }
        }
        public uint AP
        {
            get { return _ap; }
            set
            {
                if (_ap == value)
                    return;
                _ap = value;
                NeedsToSave = true;
            }
        }
        public uint Coins1
        {
            get { return _coins1; }
            set
            {
                if (_coins1 == value)
                    return;
                _coins1 = value;
                NeedsToSave = true;
            }
        }
        public uint Coins2
        {
            get { return _coins2; }
            set
            {
                if (_coins2 == value)
                    return;
                _coins2 = value;
                NeedsToSave = true;
            }
        }

        #endregion

        public Player(GameSession session, Account account, PlayerDto dto)
        {
            Session = session;
            Account = account;
            _tutorialState = dto.TutorialState;
            _level = dto.Level;
            _totalExperience = (uint)dto.TotalExperience;
            _pen = (uint)dto.PEN;
            _ap = (uint)dto.AP;
            _coins1 = (uint)dto.Coins1;
            _coins2 = (uint)dto.Coins2;

            Settings = new PlayerSettingManager(this, dto);
            DenyManager = new DenyManager(this, dto);
            Mailbox = new Mailbox(this, dto);

            LicenseManager = new LicenseManager(this, dto);
            Inventory = new Inventory(this, dto);
            CharacterManager = new CharacterManager(this, dto);

            RoomInfo = new PlayerRoomInfo();
        }

        /// <summary>
        /// Gains experiences and levels up if the player earned enough experience
        /// </summary>
        /// <param name="amount">Amount of experience to earn</param>
        /// <returns>true if the player leveled up</returns>
        public bool GainExp(uint amount)
        {
            Logger.Debug()
                .Account(this)
                .Message("Gained {0} exp", amount)
                .Write();

            var expTable = GameServer.Instance.ResourceCache.GetExperience();
            var expInfo = expTable.GetValueOrDefault(Level);
            if (expInfo == null)
            {
                Logger.Warn()
                    .Account(this)
                    .Message("Level {0} not found", Level)
                    .Write();

                return false;
            }

            // We cant earn exp when we reached max level
            if (expInfo.ExperienceToNextLevel == 0 || Level >= Config.Instance.Game.MaxLevel)
                return false;

            var leveledUp = false;
            TotalExperience += amount;

            // Did we level up?
            // Using a loop for multiple level ups
            while (expInfo.ExperienceToNextLevel != 0 &&
                expInfo.ExperienceToNextLevel <= (int)(TotalExperience - expInfo.TotalExperience))
            {
                var newLevel = Level + 1;
                expInfo = expTable.GetValueOrDefault(newLevel);

                if (expInfo == null)
                {
                    Logger.Warn()
                        .Account(this)
                        .Message("Can't level up because level {0} not found", newLevel)
                        .Write();

                    break;
                }

                Logger.Debug()
                    .Account(this)
                    .Message("Leveled up to {0}", newLevel)
                    .Write();

                // ToDo level rewards

                Level++;
                leveledUp = true;
            }

            if (!leveledUp)
                return false;

            Channel?.BroadcastChat(new SUserDataAckMessage(this.Map<Player, UserDataDto>()));

            // ToDo Do we need to update inside rooms too?

            // ToDo Do we need this?
            //await Session.SendAsync(new SBeginAccountInfoAckMessage())
            //    .ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Gets the maximum hp for the current character
        /// </summary>
        public float GetMaxHP()
        {
            return GameServer.Instance.ResourceCache.GetGameTempos()["GAMETEMPO_FREE"].ActorDefaultHPMax +
                   GetAttributeValue(Attribute.HP);
        }

        /// <summary>
        /// Gets the total attribute value for the current character
        /// </summary>
        /// <param name="attribute">The attribute to retrieve</param>
        /// <returns></returns>
        public int GetAttributeValue(Attribute attribute)
        {
            if (CharacterManager.CurrentCharacter == null)
                return 0;

            var @char = CharacterManager.CurrentCharacter;
            var value = GetAttributeValueFromItems(attribute, @char.Weapons.GetItems());
            value += GetAttributeValueFromItems(attribute, @char.Skills.GetItems());
            value += GetAttributeValueFromItems(attribute, @char.Costumes.GetItems());

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

            var @char = CharacterManager.CurrentCharacter;
            var value = GetAttributeRateFromItems(attribute, @char.Weapons.GetItems());
            value += GetAttributeRateFromItems(attribute, @char.Skills.GetItems());
            value += GetAttributeRateFromItems(attribute, @char.Costumes.GetItems());

            return value;
        }

        /// <summary>
        /// Sends a message to the game master console
        /// </summary>
        /// <param name="message">The message to send</param>
        public void SendConsoleMessage(string message)
        {
            Session.Send(new SAdminActionAckMessage { Result = 1, Message = message });
        }

        /// <summary>
        /// Sends a notice message
        /// </summary>
        /// <param name="message">The message to send</param>
        public void SendNotice(string message)
        {
            Session.Send(new SNoticeMessageAckMessage(message));
        }

        /// <summary>
        /// Saves all pending changes to the database
        /// </summary>
        public void Save(bool createScope)
        {
            var scope = createScope ? new DataAccessScope() : null;
            try
            {
                if (NeedsToSave)
                {
                    var plrRef = GameDatabase.Instance.Players.GetReference((int)Account.Id);
                    plrRef.TutorialState = TutorialState;
                    plrRef.Level = Level;
                    plrRef.TotalExperience = (int)TotalExperience;
                    plrRef.PEN = (int)PEN;
                    plrRef.AP = (int)AP;
                    plrRef.Coins1 = (int)Coins1;
                    plrRef.Coins2 = (int)Coins2;
                    plrRef.CurrentCharacterSlot = CharacterManager.CurrentSlot;

                    NeedsToSave = false;
                }

                Inventory.Save();
                CharacterManager.Save();
                LicenseManager.Save();
                DenyManager.Save();
                Mailbox.Save();

                scope?.Complete();
            }
            finally
            {
                scope?.Dispose();
            }
        }

        /// <summary>
        /// Disconnects the player
        /// </summary>
        public void Disconnect()
        {
            Session?.Dispose();
        }

        private static int GetAttributeValueFromItems(Attribute attribute, IEnumerable<PlayerItem> items)
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
