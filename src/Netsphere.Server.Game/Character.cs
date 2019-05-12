using Logging;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Data;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game
{
    public class Character : DatabaseObject
    {
        public CharacterManager CharacterManager { get; }
        public long Id { get; }
        public byte Slot { get; }
        public CharacterGender Gender { get; }
        public DefaultItem Hair { get; }
        public DefaultItem Face { get; }
        public DefaultItem Shirt { get; }
        public DefaultItem Pants { get; }
        public DefaultItem Gloves { get; }
        public DefaultItem Shoes { get; }
        public CharacterInventory Weapons { get; }
        public CharacterInventory Skills { get; }
        public CharacterInventory Costumes { get; }

        internal Character(ILogger<Character> logger, CharacterManager characterManager, PlayerCharacterEntity entity,
            GameDataService gameDataService)
        {
            SetExistsState(true);

            CharacterManager = characterManager;
            Weapons = new CharacterInventory(this, 3, ItemCategory.Weapon);
            Skills = new CharacterInventory(this, 1, ItemCategory.Skill);
            Costumes = new CharacterInventory(this, 7, ItemCategory.Costume);
            Id = entity.Id;
            Slot = entity.Slot;
            Gender = (CharacterGender)entity.Gender;
            Hair = gameDataService.GetDefaultItem(Gender, CostumeSlot.Hair, entity.BasicHair);
            Face = gameDataService.GetDefaultItem(Gender, CostumeSlot.Face, entity.BasicFace);
            Shirt = gameDataService.GetDefaultItem(Gender, CostumeSlot.Shirt, entity.BasicShirt);
            Pants = gameDataService.GetDefaultItem(Gender, CostumeSlot.Pants, entity.BasicPants);
            Gloves = gameDataService.GetDefaultItem(Gender, CostumeSlot.Gloves, 0);
            Shoes = gameDataService.GetDefaultItem(Gender, CostumeSlot.Shoes, 0);

            var plr = characterManager.Player;
            var inventory = CharacterManager.Player.Inventory;
            plr.AddContextToLogger(logger);

            SetInventoryIfNeeded(entity.Weapon1Id, (byte)WeaponSlot.Weapon1, Weapons);
            SetInventoryIfNeeded(entity.Weapon2Id, (byte)WeaponSlot.Weapon2, Weapons);
            SetInventoryIfNeeded(entity.Weapon3Id, (byte)WeaponSlot.Weapon3, Weapons);

            SetInventoryIfNeeded(entity.HairId, (byte)CostumeSlot.Hair, Costumes);
            SetInventoryIfNeeded(entity.FaceId, (byte)CostumeSlot.Face, Costumes);
            SetInventoryIfNeeded(entity.ShirtId, (byte)CostumeSlot.Shirt, Costumes);
            SetInventoryIfNeeded(entity.PantsId, (byte)CostumeSlot.Pants, Costumes);
            SetInventoryIfNeeded(entity.GlovesId, (byte)CostumeSlot.Gloves, Costumes);
            SetInventoryIfNeeded(entity.ShoesId, (byte)CostumeSlot.Shoes, Costumes);
            SetInventoryIfNeeded(entity.AccessoryId, (byte)CostumeSlot.Accessory, Costumes);
            SetInventoryIfNeeded(entity.PetId, (byte)CostumeSlot.Pet, Costumes);

            SetInventoryIfNeeded(entity.SkillId, (byte)SkillSlot.Skill, Skills);

            Weapons.ItemAdded += SendEquip;
            Weapons.ItemRemoved += SendUnEquip;

            Costumes.ItemAdded += SendEquip;
            Costumes.ItemRemoved += SendUnEquip;

            Skills.ItemAdded += SendEquip;
            Skills.ItemRemoved += SendUnEquip;

            void SendEquip(object _, CharacterInventoryEventArgs e)
            {
                plr.Session.Send(new ItemUseItemAckMessage(Slot, e.Slot, e.Item.Id, UseItemAction.Equip));
            }

            void SendUnEquip(object _, CharacterInventoryEventArgs e)
            {
                plr.Session.Send(new ItemUseItemAckMessage(Slot, e.Slot, e.Item.Id, UseItemAction.UnEquip));
            }

            void SetInventoryIfNeeded(long? id, byte itemSlot, CharacterInventory characterInventory)
            {
                var log = logger.ForContext(
                    ("CharacterSlot", Slot),
                    ("ItemSlot", itemSlot),
                    ("ItemId", id));

                if (id == null)
                    return;

                var item = inventory[(ulong)id];
                if (item == null)
                {
                    log.Warning("Character has non-existant item");
                    return;
                }

                var error = characterInventory.Add(itemSlot, item);
                if (error != CharacterInventoryError.OK)
                    log.Warning("Unable to equip item Error={Error}", error);
            }
        }

        internal Character(CharacterManager characterManager, long id, byte slot, CharacterGender gender,
            DefaultItem hair, DefaultItem face, DefaultItem shirt, DefaultItem pants, DefaultItem gloves, DefaultItem shoes)
        {
            CharacterManager = characterManager;
            Weapons = new CharacterInventory(this, 3, ItemCategory.Weapon);
            Skills = new CharacterInventory(this, 1, ItemCategory.Skill);
            Costumes = new CharacterInventory(this, 7, ItemCategory.Costume);
            Id = id;
            Slot = slot;
            Gender = gender;
            Hair = hair;
            Face = face;
            Shirt = shirt;
            Pants = pants;
            Gloves = gloves;
            Shoes = shoes;

            Weapons.ItemAdded += SendEquip;
            Weapons.ItemRemoved += SendUnEquip;

            Costumes.ItemAdded += SendEquip;
            Costumes.ItemRemoved += SendUnEquip;

            Skills.ItemAdded += SendEquip;
            Skills.ItemRemoved += SendUnEquip;

            void SendEquip(object _, CharacterInventoryEventArgs e)
            {
                CharacterManager.Player.Session.Send(new ItemUseItemAckMessage(Slot, e.Slot, e.Item.Id, UseItemAction.Equip));
            }

            void SendUnEquip(object _, CharacterInventoryEventArgs e)
            {
                CharacterManager.Player.Session.Send(new ItemUseItemAckMessage(Slot, e.Slot, e.Item.Id, UseItemAction.UnEquip));
            }
        }

        public CharacterInventoryError Equip(PlayerItem item, byte slot)
        {
            CharacterInventory inventory;
            switch (item.ItemNumber.Category)
            {
                case ItemCategory.Costume:
                    inventory = Costumes;
                    break;

                case ItemCategory.Weapon:
                    inventory = Weapons;
                    break;

                case ItemCategory.Skill:
                    inventory = Skills;
                    break;

                default:
                    return CharacterInventoryError.ItemNotAllowed;
            }

            return inventory.Add(slot, item);
        }

        public CharacterInventoryError UnEquip(ItemCategory category, byte slot)
        {
            CharacterInventory inventory;
            switch (category)
            {
                case ItemCategory.Costume:
                    inventory = Costumes;
                    break;

                case ItemCategory.Weapon:
                    inventory = Weapons;
                    break;

                case ItemCategory.Skill:
                    inventory = Skills;
                    break;

                default:
                    return CharacterInventoryError.ItemNotAllowed;
            }

            return inventory.Remove(slot);
        }
    }
}
