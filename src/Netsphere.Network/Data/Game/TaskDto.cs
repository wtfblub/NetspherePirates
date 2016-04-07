using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Game
{
    public class TaskDto
    {
        [Serialize(0)]
        public uint Id { get; set; }

        [Serialize(1)]
        public byte Unk { get; set; }

        [Serialize(2)]
        public ushort Progress { get; set; }

        [Serialize(3, typeof(EnumSerializer))]
        public MissionRewardType RewardType { get; set; }

        [Serialize(4)]
        public uint Reward { get; set; }
    }
}
