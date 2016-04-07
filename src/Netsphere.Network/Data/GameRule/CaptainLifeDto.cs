using BlubLib.Serialization;

namespace Netsphere.Network.Data.GameRule
{
    public class CaptainLifeDto
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1)]
        public float HP { get; set; }
    }
}
