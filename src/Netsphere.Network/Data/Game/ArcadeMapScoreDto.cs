using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    public class ArcadeMapScoreDto
    {
        [Serialize(0)]
        public uint Unk1 { get; set; }

        [Serialize(1)]
        public uint Unk2 { get; set; }

        [Serialize(2)]
        public uint Unk3 { get; set; }

        [Serialize(3)]
        public uint Unk4 { get; set; }

        [Serialize(4)]
        public uint Unk5 { get; set; }

        [Serialize(5)]
        public uint Unk6 { get; set; }

        [Serialize(6)]
        public uint Unk7 { get; set; }

        [Serialize(7)]
        public byte Unk8 { get; set; }
    }
}
