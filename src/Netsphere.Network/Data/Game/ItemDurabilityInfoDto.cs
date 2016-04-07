using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    public class ItemDurabilityInfoDto
    {
        [Serialize(0)]
        public ulong ItemId { get; set; }

        [Serialize(1)]
        public int Durability { get; set; }

        [Serialize(2)]
        public int Unk2 { get; set; }

        [Serialize(3)]
        public int Unk3 { get; set; }

        public ItemDurabilityInfoDto()
        {
            Durability = -1;
            Unk2 = -1;
            Unk3 = -1;
        }

        public ItemDurabilityInfoDto(ulong itemId, int durability, int unk2, int unk3)
        {
            ItemId = itemId;
            Durability = durability;
            Unk2 = unk2;
            Unk3 = unk3;
        }
    }
}
