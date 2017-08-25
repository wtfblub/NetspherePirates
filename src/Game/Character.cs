using Netsphere.Database.Game;
using Netsphere.Network;
using Netsphere.Resource;

// ReSharper disable once CheckNamespace
namespace Netsphere
{
    internal class Character
    {
        public CharacterManager CharacterManager { get; }

        internal bool ExistsInDatabase { get; set; }
        internal bool NeedsToSave { get; set; }

        public int Id { get; }
        public byte Slot { get; }

        public CharacterGender Gender { get; }
        public DefaultItem Hair { get; }
        public DefaultItem Face { get; }
        public DefaultItem Shirt { get; }
        public DefaultItem Pants { get; }
        public DefaultItem Gloves { get; }
        public DefaultItem Shoes { get; }

        public WeaponManager Weapons { get; }
        public SkillManager Skills { get; }
        public CostumeManager Costumes { get; }

        internal Character(CharacterManager characterManager, PlayerCharacterDto dto)
        {
            CharacterManager = characterManager;

            Weapons = new WeaponManager(this, dto);
            Skills = new SkillManager(this, dto);
            Costumes = new CostumeManager(this, dto);

            var defaultItems = GameServer.Instance.ResourceCache.GetDefaultItems();

            ExistsInDatabase = true;
            Id = dto.Id;
            Slot = dto.Slot;
            Gender = (CharacterGender)dto.Gender;

            Hair = defaultItems.Get(Gender, CostumeSlot.Hair, dto.BasicHair);
            Face = defaultItems.Get(Gender, CostumeSlot.Face, dto.BasicFace);
            Shirt = defaultItems.Get(Gender, CostumeSlot.Shirt, dto.BasicShirt);
            Pants = defaultItems.Get(Gender, CostumeSlot.Pants, dto.BasicPants);
            Gloves = defaultItems.Get(Gender, CostumeSlot.Gloves, 0);
            Shoes = defaultItems.Get(Gender, CostumeSlot.Shoes, 0);
        }

        internal Character(CharacterManager characterManager, byte slot, CharacterGender gender, byte hair, byte face, byte shirt, byte pants)
        {
            CharacterManager = characterManager;

            Weapons = new WeaponManager(this);
            Skills = new SkillManager(this);
            Costumes = new CostumeManager(this);

            Id = CharacterIdGenerator.GetNextId();
            Slot = slot;
            Gender = gender;

            var defaultItems = GameServer.Instance.ResourceCache.GetDefaultItems();
            Hair = defaultItems.Get(Gender, CostumeSlot.Hair, hair);
            Face = defaultItems.Get(Gender, CostumeSlot.Face, face);
            Shirt = defaultItems.Get(Gender, CostumeSlot.Shirt, shirt);
            Pants = defaultItems.Get(Gender, CostumeSlot.Pants, pants);
            Gloves = defaultItems.Get(Gender, CostumeSlot.Gloves, 0);
            Shoes = defaultItems.Get(Gender, CostumeSlot.Shoes, 0);
        }

        public void Equip(PlayerItem item, byte slot)
        {
            switch (item.ItemNumber.Category)
            {
                case ItemCategory.Costume:
                    Costumes.Equip(item, (CostumeSlot)slot);
                    break;

                case ItemCategory.Weapon:
                    Weapons.Equip(item, (WeaponSlot)slot);
                    break;

                case ItemCategory.Skill:
                    Skills.Equip(item, (SkillSlot)slot);
                    break;

                default:
                    throw new CharacterException("Invalid category" + item.ItemNumber.Category);
            }
        }

        public void UnEquip(ItemCategory category, byte slot)
        {
            switch (category)
            {
                case ItemCategory.Costume:
                    Costumes.UnEquip((CostumeSlot)slot);
                    break;

                case ItemCategory.Weapon:
                    Weapons.UnEquip((WeaponSlot)slot);
                    break;

                case ItemCategory.Skill:
                    Skills.UnEquip((SkillSlot)slot);
                    break;

                default:
                    throw new CharacterException("Invalid category" + category);
            }
        }

        public bool CanEquip(PlayerItem item, byte slot)
        {
            switch (item.ItemNumber.Category)
            {
                case ItemCategory.Costume:
                    return Costumes.CanEquip(item, (CostumeSlot)slot);

                case ItemCategory.Weapon:
                    return Weapons.CanEquip(item, (WeaponSlot)slot);

                case ItemCategory.Skill:
                    return Skills.CanEquip(item, (SkillSlot)slot);

                default:
                    return false;
            }
        }
    }
}
