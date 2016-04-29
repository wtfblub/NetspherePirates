using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class BRUserDataDto
    {
        [BlubMember(0)]
        public float Score { get; set; }

        [BlubMember(1)]
        public uint CountFirstPlaceKilled { get; set; }

        [BlubMember(2)]
        public uint CountFirstPlace { get; set; }
    }
}
