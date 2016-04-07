using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    public class CPTUserDataDto
    {
        [Serialize(0)]
        public float Score { get; set; }

        [Serialize(1)]
        public uint CaptainKill { get; set; }

        [Serialize(2)]
        public uint Domination { get; set; }
    }
}
