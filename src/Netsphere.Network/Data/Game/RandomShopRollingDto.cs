using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class RandomShopRollingDto
    {
        [BlubMember(0)]
        public ItemNumber ItemId { get; set; }

        [BlubMember(1)]
        public ItemPeriodType PeriodType { get; set; }

        [BlubMember(2)]
        public int Period { get; set; }

        [BlubMember(3)]
        public byte Color { get; set; }

        [BlubMember(4)]
        public uint EffectGroupId { get; set; }

        [BlubMember(5)]
        public byte Unk1 { get; set; }

        [BlubMember(6)]
        public uint ShopItemId { get; set; }
    }
}
