using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class DMUserDataDto
    {
        [BlubMember(0)]
        public float KillDeath { get; set; }

        [BlubMember(1)]
        public float WinRate { get; set; }
    }
}
