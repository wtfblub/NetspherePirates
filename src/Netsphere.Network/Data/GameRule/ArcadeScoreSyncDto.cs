using BlubLib.Serialization;

namespace Netsphere.Network.Data.GameRule
{
    [BlubContract]
    public class ArcadeScoreSyncDto
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public int Unk1 { get; set; }

        [BlubMember(2)]
        public int Unk2 { get; set; }

        [BlubMember(3)]
        public int Unk3 { get; set; }

        [BlubMember(4)]
        public int Unk4 { get; set; }
    }

    [BlubContract]
    public class ArcadeScoreSyncReqDto
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public int Unk1 { get; set; }

        [BlubMember(2)]
        public int Unk2 { get; set; }

        [BlubMember(3)]
        public int Unk3 { get; set; }
    }
}
