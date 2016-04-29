using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class TDUserDataDto
    {
        [BlubMember(0)]
        public float TotalScore { get; set; }

        [BlubMember(1)]
        public float TDScore { get; set; }

        [BlubMember(2)]
        public float OffenseScore { get; set; }

        [BlubMember(3)]
        public float DefenseScore { get; set; }

        [BlubMember(4)]
        public float KillScore { get; set; }

        [BlubMember(5)]
        public float RecoveryScore { get; set; }

        [BlubMember(6)]
        public float WinRate { get; set; }
    }
}
