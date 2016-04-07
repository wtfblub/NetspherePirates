using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Game
{
    public class CapsuleRewardDto
    {
        [Serialize(0, typeof(EnumSerializer))]
        public CapsuleRewardType RewardType { get; set; }

        [Serialize(1)]
        public uint PEN { get; set; }

        [Serialize(2)]
        public ulong ItemUID { get; set; }

        [Serialize(3)]
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
