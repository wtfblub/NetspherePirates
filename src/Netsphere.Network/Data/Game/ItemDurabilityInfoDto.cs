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
        public int Unk1 { get; set; }

        public ItemDurabilityInfoDto()
        {
            Durability = -1;
            Unk1 = -1;
        }

        public ItemDurabilityInfoDto(ulong itemId, int durability, int unk1)
        {
            ItemId = itemId;
            Durability = durability;
            Unk1 = unk1;
        }
    }
}
