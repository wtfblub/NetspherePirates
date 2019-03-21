using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class TaskDto
    {
        [BlubMember(0)]
        public uint Id { get; set; }

        [BlubMember(1)]
        public byte Slot { get; set; }

        [BlubMember(2)]
        public ushort Progress { get; set; }

        [BlubMember(3)]
        public MissionRewardType RewardType { get; set; }

        [BlubMember(4)]
        public uint Reward { get; set; }
    }
}
