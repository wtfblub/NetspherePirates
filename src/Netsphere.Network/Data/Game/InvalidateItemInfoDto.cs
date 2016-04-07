using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    public class InvalidateItemInfoDto
    {
        [Serialize(0)]
        public ulong ItemId { get; set; }

        [Serialize(1)]
        public uint Unk1 { get; set; }

        [Serialize(2)]
        public uint Unk2 { get; set; }

        [Serialize(3)]
        public byte Unk3 { get; set; }
    }
}
