using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class ChaserStatsDto
    {
        [BlubMember(0)]
        public uint ChasedWon { get; set; }

        [BlubMember(1)]
        public uint ChasedRounds { get; set; }

        [BlubMember(2)]
        public uint ChaserWon { get; set; }

        [BlubMember(3)]
        public uint ChaserRounds { get; set; }

        [BlubMember(4)]
        public uint Unk { get; set; }
    }
}
