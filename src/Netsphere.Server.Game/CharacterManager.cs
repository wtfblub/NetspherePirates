using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Netsphere.Common;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Services;
using Z.EntityFramework.Plus;

namespace Netsphere.Server.Game
{
    public class CharacterManager : ISaveable, IReadOnlyCollection<Character>
    {
        private ILogger _logger;
        private readonly IdGeneratorService _idGeneratorService;
        private readonly GameDataService _gameDataService;
        private readonly ILoggerFactory _loggerFactory;
        private readonly Dictionary<byte, Character> _characters;
        private readonly ConcurrentStack<Character> _charactersToRemove;
        // ReSharper disable once NotAccessedField.Local

        public Player Player { get; private set; }
        public Character CurrentCharacter => GetCharacter(CurrentSlot);
        public byte CurrentSlot { get; private set; }
        public int Count => _characters.Count;

        /// <summary>
        /// Returns the character on the given slot.
        /// Returns null if the character does not exist
        /// </summary>
        public Character this[byte slot] => GetCharacter(slot);

        public CharacterManager(ILogger<CharacterManager> logger, IdGeneratorService idGeneratorService,
            GameDataService gameDataService, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _idGeneratorService = idGeneratorService;
            _gameDataService = gameDataService;
            _loggerFactory = loggerFactory;
            _characters = new Dictionary<byte, Character>();
            _charactersToRemove = new ConcurrentStack<Character>();
        }

        internal void Initialize(Player plr, PlayerEntity entity)
        {
            _logger = plr.AddContextToLogger(_logger);
            Player = plr;
            CurrentSlot = entity.CurrentCharacterSlot;

            foreach (var @char in entity.Characters.Select(@char =>
                new Character(_loggerFactory.CreateLogger<Character>(), this, @char, _gameDataService)))
            {
                if (!_characters.TryAdd(@char.Slot, @char))
                    _logger.Warning("Multiple characters on slot={Slot}", @char.Slot);
            }
        }

        /// <summary>
        /// Returns the character on the given slot.
        /// Returns null if the character does not exist
        /// </summary>
        public Character GetCharacter(byte slot)
        {
            return _characters.GetValueOrDefault(slot);
        }

        /// <summary>
        /// Creates a new character
        /// </summary>
        public (Character character, CharacterCreateResult result) Create(byte slot, CharacterGender gender,
            byte hair, byte face, byte shirt, byte pants, byte gloves, byte shoes)
        {
            var logger = _logger.ForContext(
                ("Method", "Create"),
                ("Slot", slot),
                ("Gender", gender),
                ("Hair", hair),
                ("Face", face),
                ("Shirt", shirt),
                ("Pants", pants),
                ("Gloves", gloves),
                ("Shoes", shoes));

            if (Count >= 3)
                return (null, CharacterCreateResult.LimitReached);

            if (_characters.ContainsKey(slot))
                return (null, CharacterCreateResult.SlotInUse);

            var defaultHair = _gameDataService.GetDefaultItem(gender, CostumeSlot.Hair, hair);
            if (defaultHair == null)
            {
                logger.Warning("Invalid hair");
                return (null, CharacterCreateResult.InvalidDefaultItem);
            }

            var defaultFace = _gameDataService.GetDefaultItem(gender, CostumeSlot.Face, face);
            if (defaultFace == null)
            {
                logger.Warning("Invalid face");
                return (null, CharacterCreateResult.InvalidDefaultItem);
            }

            var defaultShirt = _gameDataService.GetDefaultItem(gender, CostumeSlot.Shirt, shirt);
            if (defaultShirt == null)
            {
                logger.Warning("Invalid shirt");
                return (null, CharacterCreateResult.InvalidDefaultItem);
            }

            var defaultPants = _gameDataService.GetDefaultItem(gender, CostumeSlot.Pants, pants);
            if (defaultPants == null)
            {
                logger.Warning("Invalid pants");
                return (null, CharacterCreateResult.InvalidDefaultItem);
            }

            var defaultGloves = _gameDataService.GetDefaultItem(gender, CostumeSlot.Gloves, gloves);
            if (defaultGloves == null)
            {
                logger.Warning("Invalid gloves");
                return (null, CharacterCreateResult.InvalidDefaultItem);
            }

            var defaultShoes = _gameDataService.GetDefaultItem(gender, CostumeSlot.Shoes, shoes);
            if (defaultShoes == null)
            {
                logger.Warning("Invalid shoes");
                return (null, CharacterCreateResult.InvalidDefaultItem);
            }

            var character = new Character(this, _idGeneratorService.GetNextId(IdKind.Character),
                slot, gender, defaultHair, defaultFace, defaultShirt, defaultPants, defaultGloves, defaultShoes);
            _characters.Add(slot, character);

            var charStyle = new CharacterStyle(character.Gender, character.Slot,
                character.Hair.Variation, character.Face.Variation,
                character.Shirt.Variation, character.Pants.Variation);
            Player.Session.Send(new CSuccessCreateCharacterAckMessage(character.Slot, charStyle));

            return (character, CharacterCreateResult.Success);
        }

        /// <summary>
        /// Selects the character on the given slot
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public bool Select(byte slot)
        {
            if (!Contains(slot))
                return false;

            if (CurrentSlot != slot)
                Player.SetDirtyState(true);

            CurrentSlot = slot;
            Player.Session.Send(new CharacterSelectAckMessage(CurrentSlot));
            return true;
        }

        /// <summary>
        /// Removes the character
        /// </summary>
        public void Remove(Character character)
        {
            Remove(character.Slot);
        }

        /// <summary>
        /// Removes the character on the given slot
        /// </summary>
        public bool Remove(byte slot)
        {
            var character = GetCharacter(slot);
            if (character == null)
                return false;

            _characters.Remove(slot);
            if (character.Exists)
                _charactersToRemove.Push(character);

            character.Weapons.Clear();
            character.Skills.Clear();
            character.Costumes.Clear();
            Player.Session.Send(new CharacterDeleteAckMessage(slot));
            return true;
        }

        public async Task Save(GameContext db)
        {
            if (!_charactersToRemove.IsEmpty)
            {
                var idsToRemove = new List<long>();
                while (_charactersToRemove.TryPop(out var characterToRemove))
                    idsToRemove.Add(characterToRemove.Id);

                await db.PlayerCharacters.Where(x => idsToRemove.Contains(x.Id)).DeleteAsync();
            }

            foreach (var character in _characters.Values)
            {
                if (!character.Exists)
                {
                    var entity = new PlayerCharacterEntity
                    {
                        Id = character.Id,
                        PlayerId = (int)Player.Account.Id,
                        Slot = character.Slot,
                        Gender = (byte)character.Gender,
                        BasicHair = character.Hair.Variation,
                        BasicFace = character.Face.Variation,
                        BasicShirt = character.Shirt.Variation,
                        BasicPants = character.Pants.Variation
                    };
                    SetDtoItems(character, entity);
                    db.PlayerCharacters.Add(entity);
                    character.SetExistsState(true);
                }
                else
                {
                    if (!character.IsDirty)
                        continue;

                    var entity = new PlayerCharacterEntity
                    {
                        Id = character.Id,
                        PlayerId = (int)Player.Account.Id,
                        Slot = character.Slot,
                        Gender = (byte)character.Gender,
                        BasicHair = character.Hair.Variation,
                        BasicFace = character.Face.Variation,
                        BasicShirt = character.Shirt.Variation,
                        BasicPants = character.Pants.Variation
                    };
                    SetDtoItems(character, entity);
                    db.PlayerCharacters.Update(entity);
                    character.SetDirtyState(false);
                }
            }
        }

        public bool Contains(byte slot)
        {
            return _characters.ContainsKey(slot);
        }

        public IEnumerator<Character> GetEnumerator()
        {
            return _characters.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static void SetDtoItems(Character character, PlayerCharacterEntity entity)
        {
            // Weapons
            var items = character.Weapons.GetItems();
            for (var slot = 0; slot < items.Length; ++slot)
            {
                var itemId = (long?)items[slot]?.Id;

                switch (slot)
                {
                    case 0:
                        entity.Weapon1Id = itemId;
                        break;

                    case 1:
                        entity.Weapon2Id = itemId;
                        break;

                    case 2:
                        entity.Weapon3Id = itemId;
                        break;
                }
            }

            // Skills
            items = character.Skills.GetItems();
            entity.SkillId = (long?)items[0]?.Id;

            // Costumes
            items = character.Costumes.GetItems();
            for (var slot = 0; slot < items.Length; ++slot)
            {
                var itemId = (long?)items[slot]?.Id;

                switch ((CostumeSlot)slot)
                {
                    case CostumeSlot.Hair:
                        entity.HairId = itemId;
                        break;

                    case CostumeSlot.Face:
                        entity.FaceId = itemId;
                        break;

                    case CostumeSlot.Shirt:
                        entity.ShirtId = itemId;
                        break;

                    case CostumeSlot.Pants:
                        entity.PantsId = itemId;
                        break;

                    case CostumeSlot.Gloves:
                        entity.GlovesId = itemId;
                        break;

                    case CostumeSlot.Shoes:
                        entity.ShoesId = itemId;
                        break;

                    case CostumeSlot.Accessory:
                        entity.AccessoryId = itemId;
                        break;

                    case CostumeSlot.Pet:
                        entity.PetId = itemId;
                        break;
                }
            }
        }
    }
}
