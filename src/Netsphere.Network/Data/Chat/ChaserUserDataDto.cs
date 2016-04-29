using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class ChaserUserDataDto
    {
        [BlubMember(0)]
        public float SurvivalProbability { get; set; }

        [BlubMember(1)]
        public float AllKillProbability { get; set; }
    }
}
