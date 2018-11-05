using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Logging;
using Netsphere.Common;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game
{
    public class CharacterManager : ISaveable, IReadOnlyCollection<Character>
    {
        private readonly ILogger _logger;
        private readonly IdGeneratorService _idGeneratorService;
        private readonly GameDataService _gameDataService;
        private readonly Dictionary<byte, Character> _characters;
        private readonly ConcurrentStack<Character> _charactersToRemove;
        // ReSharper disable once NotAccessedField.Local
        private IDisposable _scope;

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
            GameDataService gameDataService)
        {
            _logger = logger;
            _idGeneratorService = idGeneratorService;
            _gameDataService = gameDataService;
            _characters = new Dictionary<byte, Character>();
            _charactersToRemove = new ConcurrentStack<Character>();
        }

        internal void Initialize(Player plr, PlayerEntity entity)
        {
            _scope = _logger.BeginScope("AccountId={AccountId}", plr.Account.Id);
            Player = plr;
            CurrentSlot = entity.CurrentCharacterSlot;

            foreach (var @char in entity.Characters.Select(@char => new Character(this, @char, _gameDataService)))
            {
                if (!_characters.TryAdd(@char.Slot, @char))
                    _logger.LogWarning("Multiple characters on slot={Slot}", @char.Slot);
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
            using (_logger.BeginScope(
                "Method=Create Slot={Slot} Gender={Gender} Hair={Hair} Face={Face} Shirt={Shirt} Pants={Pants} Gloves={Gloves} Shoes={Shoes}",
                slot, gender, hair, face, shirt, pants, gloves, shoes))
            {
                if (Count >= 3)
                    return (null, CharacterCreateResult.LimitReached);

                if (_characters.ContainsKey(slot))
                    return (null, CharacterCreateResult.SlotInUse);

                var defaultHair = _gameDataService.GetDefaultItem(gender, CostumeSlot.Hair, hair);
                if (defaultHair == null)
                {
                    _logger.LogWarning("Invalid hair");
                    return (null, CharacterCreateResult.InvalidDefaultItem);
                }

                var defaultFace = _gameDataService.GetDefaultItem(gender, CostumeSlot.Face, face);
                if (defaultFace == null)
                {
                    _logger.LogWarning("Invalid face");
                    return (null, CharacterCreateResult.InvalidDefaultItem);
                }

                var defaultShirt = _gameDataService.GetDefaultItem(gender, CostumeSlot.Shirt, shirt);
                if (defaultShirt == null)
                {
                    _logger.LogWarning("Invalid shirt");
                    return (null, CharacterCreateResult.InvalidDefaultItem);
                }

                var defaultPants = _gameDataService.GetDefaultItem(gender, CostumeSlot.Pants, pants);
                if (defaultPants == null)
                {
                    _logger.LogWarning("Invalid pants");
                    return (null, CharacterCreateResult.InvalidDefaultItem);
                }

                var defaultGloves = _gameDataService.GetDefaultItem(gender, CostumeSlot.Gloves, gloves);
                if (defaultGloves == null)
                {
                    _logger.LogWarning("Invalid gloves");
                    return (null, CharacterCreateResult.InvalidDefaultItem);
                }

                var defaultShoes = _gameDataService.GetDefaultItem(gender, CostumeSlot.Shoes, shoes);
                if (defaultShoes == null)
                {
                    _logger.LogWarning("Invalid shoes");
                    return (null, CharacterCreateResult.InvalidDefaultItem);
                }

                var character = new Character(this, _idGeneratorService.GetNextId(IdKind.Character),
                    slot, gender, defaultHair, defaultFace, defaultShirt, defaultPants, defaultGloves, defaultShoes);
                _characters.Add(slot, character);

                var charStyle = new CharacterStyle(character.Gender, character.Hair.Variation,
                    character.Face.Variation, character.Shirt.Variation, character.Pants.Variation, character.Slot);
                Player.Session.SendAsync(new SSuccessCreateCharacterAckMessage(character.Slot, charStyle));

                return (character, CharacterCreateResult.Success);
            }
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
            Player.Session.Send(new SSuccessSelectCharacterAckMessage(CurrentSlot));
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

            Player.Session.Send(new SSuccessDeleteCharacterAckMessage(slot));
            return true;
        }

        public async Task Save(GameContext db)
        {
            if (!_charactersToRemove.IsEmpty)
            {
                var idsToRemove = new List<long>();
                while (_charactersToRemove.TryPop(out var characterToRemove))
                    idsToRemove.Append(characterToRemove.Id);

                // TODO Not sure if this actually works
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
                    db.Insert(entity);
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
                    db.Update(entity);
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

        private void SetDtoItems(Character character, PlayerCharacterEntity entity)
        {
            // Weapons
            var items = character.Weapons.GetItems();
            for (var slot = 0; slot <= items.Length; ++slot)
            {
                var item = items[slot];
                var itemId = item != null ? (int?)item.Id : null;

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
            entity.SkillId = items[0] != null ? (int?)items[0].Id : null;

            // Costumes
            items = character.Costumes.GetItems();
            for (var slot = 0; slot <= items.Length; ++slot)
            {
                var item = items[slot];
                var itemId = item != null ? (int?)item.Id : null;

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
                }
            }
        }
    }
}
