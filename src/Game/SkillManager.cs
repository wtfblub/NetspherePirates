using System;
using System.Collections.Generic;
using System.Linq;
using Netsphere.Database.Game;
using Netsphere.Network.Message.Game;

namespace Netsphere
{
    internal class SkillManager
    {
        private readonly Character _character;
        private readonly PlayerItem[] _items = new PlayerItem[2];

        internal SkillManager(Character @char, PlayerCharacterDto dto)
        {
            _character = @char;
            var plr = _character.CharacterManager.Player;

            _items[0] = plr.Inventory[(ulong)(dto.SkillId ?? 0)];
        }

        internal SkillManager(Character @char)
        {
            _character = @char;
        }

        public void Equip(PlayerItem item, SkillSlot slot)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (!CanEquip(item, slot))
                throw new CharacterException($"Cannot equip item {item.ItemNumber} on slot {slot}");
            
            switch (slot)
            {
                case SkillSlot.Skill:
                    if (_items[(int)slot] != item)
                    {
                        _character.NeedsToSave = true;
                        _items[(int)slot] = item;
                    }
                    break;

                default:
                    throw new CharacterException("Invalid slot: " + slot);
            }

            var plr = _character.CharacterManager.Player;
            plr.Session.Send(new SUseItemAckMessage
            {
                CharacterSlot = _character.Slot,
                ItemId = item.Id,
                Action = UseItemAction.Equip,
                EquipSlot = (byte) slot
            });
        }

        public void UnEquip(SkillSlot slot)
        {
            var plr = _character.CharacterManager.Player;
            if (plr.Room != null && plr.RoomInfo.State != PlayerState.Lobby) // Cant change items while playing
                throw new CharacterException("Can't change items while playing");

            PlayerItem item;
            switch (slot)
            {
                case SkillSlot.Skill:
                    item = _items[(int)slot];
                    if (item != null)
                    {
                        _character.NeedsToSave = true;
                        _items[(int)slot] = null;
                    }
                    break;

                default:
                    throw new CharacterException("Invalid slot: " + slot);
            }

            plr.Session.Send(new SUseItemAckMessage
            {
                CharacterSlot = _character.Slot,
                ItemId = item?.Id ?? 0,
                Action = UseItemAction.UnEquip,
                EquipSlot = (byte)slot
            });
        }

        public PlayerItem GetItem(SkillSlot slot)
        {
            switch (slot)
            {
                case SkillSlot.Skill:
                    return _items[(int)slot];

                default:
                    throw new CharacterException("Invalid slot: " + slot);
            }
        }

        public IReadOnlyList<PlayerItem> GetItems()
        {
            return _items;
        }

        public bool CanEquip(PlayerItem item, SkillSlot slot)
        {
            // ReSharper disable once UseNullPropagation
            if (item == null)
                return false;

            if (item.ItemNumber.Category != ItemCategory.Skill)
                return false;

            if (slot != SkillSlot.Skill)
                return false;

            if (_items[(int)slot] != null) // Slot needs to be empty
                return false;

            var plr = _character.CharacterManager.Player;
            if (plr.Room != null && plr.RoomInfo.State != PlayerState.Lobby) // Cant change items while playing
                return false;

            foreach (var @char in plr.CharacterManager)
            {
                if (@char.Skills.GetItems().Any(i => i?.Id == item.Id)) // Dont allow items that are already equipped on a character
                    return false;
            }

            return true;
        }
    }
}