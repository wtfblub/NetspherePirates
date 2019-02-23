using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class ArenaUserDataDto
    {
        [BlubMember(0)]
        public float WinRate { get; set; }

        [BlubMember(1)]
        public float KillDeathRate { get; set; }

        [BlubMember(2)]
        public float KillDeath { get; set; }

        [BlubMember(3)]
        public float DoubleKillRate { get; set; }

        [BlubMember(4)]
        public float TripleKillrate { get; set; }

        [BlubMember(5)]
        public float ShortestKillTime { get; set; }

        [BlubMember(6)]
        public float LeaderShowdowns { get; set; }

        [BlubMember(7)]
        public float LeaderKills { get; set; }
    }
}
