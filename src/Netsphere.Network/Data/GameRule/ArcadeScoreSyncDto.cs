using BlubLib.Serialization;

namespace Netsphere.Network.Data.GameRule
{
    public class ArcadeScoreSyncDto
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1)]
        public int Unk1 { get; set; }

        [Serialize(2)]
        public int Unk2 { get; set; }

        [Serialize(3)]
        public int Unk3 { get; set; }

        [Serialize(4)]
        public int Unk4 { get; set; }

        [Serialize(5)]
        public int Unk5 { get; set; }
    }

    public class ArcadeScoreSyncReqDto
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1)]
        public int Unk1 { get; set; }

        [Serialize(2)]
        public int Unk2 { get; set; }

        [Serialize(3)]
        public int Unk3 { get; set; }
    }
}
