using BlubLib.Serialization;

namespace Netsphere.Network.Data.Club
{
    [BlubContract]
    public class AdminGradeDto
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public int Unk { get; set; }
    }
}
