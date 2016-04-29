using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class ItemDurabilityInfoDto
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }

        [BlubMember(1)]
        public int Durability { get; set; }

        [BlubMember(2)]
        public int Unk2 { get; set; }

        [BlubMember(3)]
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
