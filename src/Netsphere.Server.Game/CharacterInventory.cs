using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Netsphere.Server.Game
{
    public class CharacterInventory
    {
        private readonly Character _character;
        private readonly ItemCategory[] _allowedCategories;
        private readonly PlayerItem[] _items;

        public event EventHandler<CharacterInventoryEventArgs> ItemAdded;
        public event EventHandler<CharacterInventoryEventArgs> ItemRemoved;

        private void OnItemAdded(byte slot, PlayerItem item)
        {
            ItemAdded?.Invoke(this, new CharacterInventoryEventArgs(slot, item));
        }

        private void OnItemRemoved(byte slot, PlayerItem item)
        {
            ItemRemoved?.Invoke(this, new CharacterInventoryEventArgs(slot, item));
        }

        public CharacterInventory(Character character, int numSlots, params ItemCategory[] allowedCategories)
        {
            if (numSlots < 0)
                throw new ArgumentOutOfRangeException(nameof(numSlots));

            _character = character;

            _allowedCategories = allowedCategories ?? Array.Empty<ItemCategory>();
            _items = new PlayerItem[numSlots];
        }

        public CharacterInventoryError Add(byte slot, PlayerItem item)
        {
            if (!IsSlotValid(slot))
                return CharacterInventoryError.InvalidSlot;

            if (_items[slot] != null)
                return CharacterInventoryError.SlotAlreadyInUse;

            if (_allowedCategories.All(category => category != item.ItemNumber.Category))
                return CharacterInventoryError.ItemNotAllowed;

            if (item.CharacterInventory != null)
                return CharacterInventoryError.ItemAlreadyInUse;

            _items[slot] = item;
            item.CharacterInventory = this;
            _character.SetDirtyState(true);
            OnItemAdded(slot, item);
            return CharacterInventoryError.OK;
        }

        public CharacterInventoryError Remove(byte slot)
        {
            if (!IsSlotValid(slot))
                return CharacterInventoryError.InvalidSlot;

            var item = _items[slot];
            _items[slot] = null;
            item.CharacterInventory = null;
            _character.SetDirtyState(true);
            OnItemRemoved(slot, item);
            return CharacterInventoryError.OK;
        }

        public (PlayerItem, CharacterInventoryError) GetItem(byte slot)
        {
            return IsSlotValid(slot)
                ? (_items[slot], CharacterInventoryError.OK)
                : (default, CharacterInventoryError.InvalidSlot);
        }

        public PlayerItem[] GetItems()
        {
            return _items.ToArray();
        }

        public bool HasItem(PlayerItem item)
        {
            return _items.Any(x => x.ItemNumber == item.ItemNumber);
        }

        public void Clear()
        {
            for (var i = 0; i < _items.Length; i++)
            {
                var item = _items[i];
                if (item != null)
                {
                    item.CharacterInventory = null;
                    _items[i] = null;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsSlotValid(byte slot)
        {
            return slot < _items.Length;
        }
    }
}
