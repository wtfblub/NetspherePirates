using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class CapsuleRewardDto
    {
        [BlubMember(0)]
        public CapsuleRewardType RewardType { get; set; }

        [BlubMember(1)]
        public uint PEN { get; set; }

        [BlubMember(2)]
        public ItemNumber ItemNumber { get; set; }

        [BlubMember(3)]
        public ItemPriceType PriceType { get; set; }

        [BlubMember(4)]
        public ItemPeriodType PeriodType { get; set; }

        [BlubMember(5)]
        public uint Period { get; set; }

        [BlubMember(6)]
        public byte Unk { get; set; }

        [BlubMember(7)]
        public uint Effect{ get; set; }
    }
}
