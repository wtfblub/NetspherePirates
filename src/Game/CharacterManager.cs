using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Netsphere.Database.Game;
using Netsphere.Network;
using Netsphere.Network.Message.Game;
using Netsphere.Resource;
using NLog;
using NLog.Fluent;

namespace Netsphere
{
    internal class CharacterManager : IReadOnlyCollection<Character>
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<byte, Character> _characters = new Dictionary<byte, Character>();

        private readonly ConcurrentStack<Character> _charactersToDelete = new ConcurrentStack<Character>();

        public Player Player { get; }
        public Character CurrentCharacter => GetCharacter(CurrentSlot);
        public byte CurrentSlot { get; private set; }
        public int Count => _characters.Count;

        /// <summary>
        /// Returns the character on the given slot.
        /// Returns null if the character does not exist
        /// </summary>
        public Character this[byte slot] => GetCharacter(slot);

        internal CharacterManager(Player plr, PlayerDto dto)
        {
            Player = plr;
            CurrentSlot = dto.CurrentCharacterSlot;

            foreach (var @char in dto.Characters.Select(@char => new Character(this, @char)))
            {
                if (!_characters.TryAdd(@char.Slot, @char))
                {
                    Logger.Warn()
                        .Account(Player)
                        .Message("Multiple characters on slot {0}", @char.Slot)
                        .Write();
                }
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
        /// <exception cref="CharacterException"></exception>
        public Character Create(byte slot, CharacterGender gender, byte hair, byte face, byte shirt, byte pants)
        {
            if (Count >= 3)
                throw new CharacterException("Character limit reached");

            if (_characters.ContainsKey(slot))
                throw new CharacterException($"Slot {slot} is already in use");

            var defaultItems = GameServer.Instance.ResourceCache.GetDefaultItems();
            if (defaultItems.Get(gender, CostumeSlot.Hair, hair) == null)
                throw new CharacterException($"Hair variation {hair} does not exist");

            if (defaultItems.Get(gender, CostumeSlot.Face, face) == null)
                throw new CharacterException($"Face variation {face} does not exist");

            if (defaultItems.Get(gender, CostumeSlot.Shirt, shirt) == null)
                throw new CharacterException($"Shirt variation {shirt} does not exist");

            if (defaultItems.Get(gender, CostumeSlot.Pants, pants) == null)
                throw new CharacterException($"Pants variation {pants} does not exist");

            var @char = new Character(this, slot, gender, hair, face, shirt, pants);
            _characters.Add(slot, @char);

            var charStyle = new CharacterStyle(@char.Gender, @char.Hair.Variation, @char.Face.Variation,
                @char.Shirt.Variation, @char.Pants.Variation, @char.Slot);
            Player.Session.Send(new SSuccessCreateCharacterAckMessage(@char.Slot, charStyle));

            return @char;
        }

        /// <summary>
        /// Selects the character on the given slot
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Select(byte slot)
        {
            if (!Contains(slot))
                throw new ArgumentException($"Slot {slot} does not exist", nameof(slot));

            if (CurrentSlot != slot)
                Player.NeedsToSave = true;

            CurrentSlot = slot;
            Player.Session.Send(new SSuccessSelectCharacterAckMessage(CurrentSlot));
        }

        /// <summary>
        /// Removes the character
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Remove(Character @char)
        {
            Remove(@char.Slot);
        }

        /// <summary>
        /// Removes the character on the given slot
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Remove(byte slot)
        {
            var @char = GetCharacter(slot);
            if (@char == null)
                throw new ArgumentException($"Slot {slot} does not exist", nameof(slot));

            _characters.Remove(slot);
            if (@char.ExistsInDatabase)
                _charactersToDelete.Push(@char);
            Player.Session.Send(new SSuccessDeleteCharacterAckMessage(slot));
        }

        internal void Save()
        {
            Character charToDelete;
            while (_charactersToDelete.TryPop(out charToDelete))
                GameDatabase.Instance.PlayerCharacters.GetReference(charToDelete.Id).Delete();

            foreach (var @char in _characters.Values)
            {
                PlayerCharacterDto charDto;
                if (!@char.ExistsInDatabase)
                {
                    var plrRef = GameDatabase.Instance.Players.GetReference((int) Player.Account.Id);
                    charDto = plrRef.Characters.Create(@char.Id);
                    @char.ExistsInDatabase = true;

                    charDto.Player = plrRef;
                    charDto.Slot = @char.Slot;
                    charDto.Gender = (byte)@char.Gender;
                    charDto.BasicHair = @char.Hair.Variation;
                    charDto.BasicFace = @char.Face.Variation;
                    charDto.BasicShirt = @char.Shirt.Variation;
                    charDto.BasicPants = @char.Pants.Variation;
                }
                else
                {
                    if (!@char.NeedsToSave)
                        continue;

                    @char.NeedsToSave = false;
                    charDto = GameDatabase.Instance.PlayerCharacters
                        .GetReference(@char.Id);
                }

                PlayerItem item;

                // Weapons
                for (var slot = WeaponSlot.Weapon1; slot <= WeaponSlot.Weapon3; slot++)
                {
                    item = @char.Weapons.GetItem(slot);
                    var itemRef = item != null ?
                        GameDatabase.Instance.PlayerItems.GetReference((int)item.Id)
                        : null;

                    switch (slot)
                    {
                        case WeaponSlot.Weapon1:
                            charDto.Weapon1 = itemRef;
                            break;

                        case WeaponSlot.Weapon2:
                            charDto.Weapon2 = itemRef;
                            break;

                        case WeaponSlot.Weapon3:
                            charDto.Weapon3 = itemRef;
                            break;
                    }
                }

                // Skills
                item = @char.Skills.GetItem(SkillSlot.Skill);
                charDto.Skill = item != null ?
                    GameDatabase.Instance.PlayerItems.GetReference((int)item.Id) : null;

                // Costumes
                for (var slot = CostumeSlot.Hair; slot <= CostumeSlot.Accessory; slot++)
                {
                    item = @char.Costumes.GetItem(slot);
                    var itemRef = item != null ?
                        GameDatabase.Instance.PlayerItems.GetReference((int)item.Id)
                        : null;

                    switch (slot)
                    {
                        case CostumeSlot.Hair:
                            charDto.Hair = itemRef;
                            break;

                        case CostumeSlot.Face:
                            charDto.Face = itemRef;
                            break;

                        case CostumeSlot.Shirt:
                            charDto.Shirt = itemRef;
                            break;

                        case CostumeSlot.Pants:
                            charDto.Pants = itemRef;
                            break;

                        case CostumeSlot.Gloves:
                            charDto.Gloves = itemRef;
                            break;

                        case CostumeSlot.Shoes:
                            charDto.Shoes = itemRef;
                            break;

                        case CostumeSlot.Accessory:
                            charDto.Accessory = itemRef;
                            break;
                    }
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
    }
}