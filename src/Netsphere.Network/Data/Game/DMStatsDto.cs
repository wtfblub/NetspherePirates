using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    public class DMStatsDto
    {
        // K/D = ((Kills * 2) + KillAssist) / (Deaths * 2)
        [Serialize(0)]
        public uint Won { get; set; }

        [Serialize(1)]
        public uint Lost { get; set; }

        [Serialize(2)]
        public uint Kills { get; set; }

        [Serialize(3)]
        public uint KillAssists { get; set; }

        [Serialize(4)]
        public uint Unk5 { get; set; } // suicide?

        [Serialize(5)]
        public uint Deaths { get; set; }

        [Serialize(6)]
        public uint Unk7 { get; set; }

        [Serialize(7)]
        public uint Unk8 { get; set; }

        [Serialize(8)]
        public uint Unk9 { get; set; }
    }
}
