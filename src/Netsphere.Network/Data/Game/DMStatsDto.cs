using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class DMStatsDto
    {
        // K/D = ((Kills * 2) + KillAssist) / (Deaths * 2)
        [BlubMember(0)]
        public uint Won { get; set; }

        [BlubMember(1)]
        public uint Lost { get; set; }

        [BlubMember(2)]
        public uint Kills { get; set; }

        [BlubMember(3)]
        public uint KillAssists { get; set; }

        [BlubMember(4)]
        public uint Unk5 { get; set; } // suicide?

        [BlubMember(5)]
        public uint Deaths { get; set; }

        [BlubMember(6)]
        public uint Unk7 { get; set; }

        [BlubMember(7)]
        public uint Unk8 { get; set; }

        [BlubMember(8)]
        public uint Unk9 { get; set; }
    }
}
