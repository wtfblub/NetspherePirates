using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    public class TDUserDataDto
    {
        [Serialize(0)]
        public float TotalScore { get; set; }

        [Serialize(1)]
        public float TDScore { get; set; }

        [Serialize(2)]
        public float OffenseScore { get; set; }

        [Serialize(3)]
        public float DefenseScore { get; set; }

        [Serialize(4)]
        public float KillScore { get; set; }

        [Serialize(5)]
        public float RecoveryScore { get; set; }

        [Serialize(6)]
        public float WinRate { get; set; }
    }
}
