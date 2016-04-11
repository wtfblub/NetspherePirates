using System;
using System.Collections.Generic;
using System.Linq;
using Netsphere.Database.Game;
using Netsphere.Network.Message.Game;

namespace Netsphere
{
    internal class CostumeManager
    {
        private readonly Character _character;
        private readonly PlayerItem[] _items = new PlayerItem[7];

        internal CostumeManager(Character @char, PlayerCharacterDto dto)
        {
            _character = @char;
            var plr = _character.CharacterManager.Player;

            _items[0] = plr.Inventory[(ulong)(dto.Hair?.Id ?? 0)];
            _items[1] = plr.Inventory[(ulong)(dto.Face?.Id ?? 0)];
            _items[2] = plr.Inventory[(ulong)(dto.Shirt?.Id ?? 0)];
            _items[3] = plr.Inventory[(ulong)(dto.Pants?.Id ?? 0)];
            _items[4] = plr.Inventory[(ulong)(dto.Gloves?.Id ?? 0)];
            _items[5] = plr.Inventory[(ulong)(dto.Shoes?.Id ?? 0)];
            _items[6] = plr.Inventory[(ulong)(dto.Accessory?.Id ?? 0)];
        }

        internal CostumeManager(Character @char)
        {
            _character = @char;
        }

        public void Equip(PlayerItem item, CostumeSlot slot)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (!CanEquip(item, slot))
                throw new CharacterException($"Cannot equip item {item.ItemNumber} on slot {slot}");
            
            switch (slot)
            {
                case CostumeSlot.Hair:
                case CostumeSlot.Face:
                case CostumeSlot.Shirt:
                case CostumeSlot.Pants:
                case CostumeSlot.Gloves:
                case CostumeSlot.Shoes:
                case CostumeSlot.Accessory:
                    if (_items[(int)slot] != item)
                    {
                        _character.NeedsToSave = true;
                        _items[(int)slot] = item;
                    }
                    break;

                default:
                    throw new CharacterException("Invalid slot: " + ((byte)slot));
            }

            var plr = _character.CharacterManager.Player;
            plr.Session.Send(new SUseItemAckMessage
            {
                CharacterSlot = _character.Slot,
                ItemId = item.Id,
                Action = UseItemAction.Equip,
                EquipSlot = (byte)slot
            });
        }

        public void UnEquip(CostumeSlot slot)
        {
            var plr = _character.CharacterManager.Player;
            if (plr.Room != null && plr.RoomInfo.State != PlayerState.Lobby) // Cant change items while playing
                throw new CharacterException("Can't change items while playing");

            PlayerItem item;
            switch (slot)
            {
                case CostumeSlot.Hair:
                case CostumeSlot.Face:
                case CostumeSlot.Shirt:
                case CostumeSlot.Pants:
                case CostumeSlot.Gloves:
                case CostumeSlot.Shoes:
                case CostumeSlot.Accessory:
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

        public PlayerItem GetItem(CostumeSlot slot)
        {
            switch (slot)
            {
                case CostumeSlot.Hair:
                case CostumeSlot.Face:
                case CostumeSlot.Shirt:
                case CostumeSlot.Pants:
                case CostumeSlot.Gloves:
                case CostumeSlot.Shoes:
                case CostumeSlot.Accessory:
                    return _items[(int)slot];

                default:
                    throw new CharacterException("Invalid slot: " + slot);
            }
        }

        public IReadOnlyList<PlayerItem> GetItems()
        {
            return _items;
        }

        public bool CanEquip(PlayerItem item, CostumeSlot slot)
        {
            // ReSharper disable once UseNullPropagation
            if (item == null)
                return false;

            if (item.ItemNumber.Category != ItemCategory.Costume)
                return false;

            if (slot > CostumeSlot.Accessory || slot < CostumeSlot.Hair)
                return false;

            if (_items[(int)slot] != null) // Slot needs to be empty
                return false;

            var plr = _character.CharacterManager.Player;
            if (plr.Room != null && plr.RoomInfo.State != PlayerState.Lobby) // Cant change items while playing
                return false;

            foreach (var @char in plr.CharacterManager)
            {
                if (@char.Costumes.GetItems().Any(i => i?.Id == item.Id)) // Dont allow items that are already equipped on a character
                    return false;
            }

            return true;
        }
    }
}