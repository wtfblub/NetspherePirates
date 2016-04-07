using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    public class ChaserUserDataDto
    {
        [Serialize(0)]
        public float SurvivalProbability { get; set; }

        [Serialize(1)]
        public float AllKillProbability { get; set; }
    }
}
