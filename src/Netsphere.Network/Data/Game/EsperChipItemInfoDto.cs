using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    public class EsperChipItemInfoDto
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public ulong Unk2 { get; set; }

        [Serialize(2)]
        public ulong Unk3 { get; set; }

        [Serialize(3)]
        public ulong Unk4 { get; set; }

        [Serialize(4)]
        public ulong Unk5 { get; set; }

        [Serialize(5)]
        public uint Unk6 { get; set; }

        [Serialize(6)]
        public uint Unk7 { get; set; }

        [Serialize(7)]
        public uint Unk8 { get; set; }
    }
}
