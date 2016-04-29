using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class CPTUserDataDto
    {
        [BlubMember(0)]
        public float Score { get; set; }

        [BlubMember(1)]
        public uint CaptainKill { get; set; }

        [BlubMember(2)]
        public uint Domination { get; set; }
    }
}
