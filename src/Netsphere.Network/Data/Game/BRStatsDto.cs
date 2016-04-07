using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    public class BRStatsDto
    {
        [Serialize(0)]
        public uint Won { get; set; }

        [Serialize(1)]
        public uint Lost { get; set; }

        [Serialize(2)]
        public uint Unk3 { get; set; }

        [Serialize(3)]
        public uint FirstKilled { get; set; }

        [Serialize(4)]
        public uint FirstPlace { get; set; }
    }
}
