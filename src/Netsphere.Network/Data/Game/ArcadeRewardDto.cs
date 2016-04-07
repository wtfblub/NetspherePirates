using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    public class ArcadeRewardDto
    {
        [Serialize(0)]
        public uint Unk1 { get; set; }

        [Serialize(1)]
        public uint Unk2 { get; set; }

        [Serialize(2)]
        public uint Unk3 { get; set; }

        [Serialize(3)]
        public ushort Unk4 { get; set; }

        [Serialize(4)]
        public uint Unk5 { get; set; }

        [Serialize(5)]
        public uint Unk6 { get; set; }
    }
}
