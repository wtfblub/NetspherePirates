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
        public ulong ItemUID { get; set; }

        [BlubMember(3)]
        public uint Quantity { get; set; }

        public CapsuleRewardDto()
        { }

        public CapsuleRewardDto(CapsuleRewardType rewardType, uint pen, ulong itemUID, uint quantity)
        {
            RewardType = rewardType;
            PEN = pen;
            ItemUID = itemUID;
            Quantity = quantity;
        }

        public CapsuleRewardDto(uint pen)
        {
            RewardType = CapsuleRewardType.PEN;
            PEN = pen;
            ItemUID = 0;
            Quantity = 0;
        }

        public CapsuleRewardDto(ulong itemUID)
        {
            RewardType = CapsuleRewardType.Item;
            PEN = 0;
            ItemUID = itemUID;
            Quantity = 0;
        }

        public CapsuleRewardDto(ulong itemUID, uint quantity)
        {
            RewardType = CapsuleRewardType.Item;
            PEN = 0;
            ItemUID = itemUID;
            Quantity = quantity;
        }
    }
}
