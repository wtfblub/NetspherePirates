using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class BRStatsDto
    {
        [BlubMember(0)]
        public uint Won { get; set; }

        [BlubMember(1)]
        public uint Lost { get; set; }

        [BlubMember(2)]
        public uint Unk3 { get; set; }

        [BlubMember(3)]
        public uint FirstKilled { get; set; }

        [BlubMember(4)]
        public uint FirstPlace { get; set; }
    }
}
