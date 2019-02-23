using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class SiegeUserDataDto
    {
        [BlubMember(0)]
        public float WinRate { get; set; }

        [BlubMember(1)]
        public float CaptureScore { get; set; }

        [BlubMember(2)]
        public float BattleScore { get; set; }

        [BlubMember(3)]
        public float MainCoreCaptureScore { get; set; }

        [BlubMember(41)]
        public float ItemObtainScore { get; set; }
    }
}
