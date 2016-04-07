using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    public class BRUserDataDto
    {
        [Serialize(0)]
        public float Score { get; set; }

        [Serialize(1)]
        public uint CountFirstPlaceKilled { get; set; }

        [Serialize(2)]
        public uint CountFirstPlace { get; set; }
    }
}
