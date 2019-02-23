using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class DMUserDataDto
    {
        [BlubMember(0)]
        public float WinRate { get; set; }

        [BlubMember(1)]
        public float KillDeathRate { get; set; }

        [BlubMember(2)]
        public float KillDeath { get; set; }

        [BlubMember(3)]
        public float KillScore { get; set; }

        [BlubMember(4)]
        public float KillAssistScore { get; set; }

        [BlubMember(5)]
        public float RecoveryScore { get; set; }
    }
}
