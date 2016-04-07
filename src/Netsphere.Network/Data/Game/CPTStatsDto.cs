using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    public class CPTStatsDto
    {
        [Serialize(0)]
        public uint Won { get; set; }

        [Serialize(1)]
        public uint Lost { get; set; }

        [Serialize(2)]
        public uint Unk3 { get; set; }

        [Serialize(3)]
        public uint CaptainKilled { get; set; }

        [Serialize(4)]
        public uint Captain { get; set; }
    }
}
