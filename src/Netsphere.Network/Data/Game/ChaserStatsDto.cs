using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    public class ChaserStatsDto
    {
        [Serialize(0)]
        public uint ChasedWon { get; set; }

        [Serialize(1)]
        public uint ChasedRounds { get; set; }

        [Serialize(2)]
        public uint ChaserWon { get; set; }

        [Serialize(3)]
        public uint ChaserRounds { get; set; }
    }
}
