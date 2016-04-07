using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    public class DMUserDataDto
    {
        [Serialize(0)]
        public float KillDeath { get; set; }

        [Serialize(1)]
        public float WinRate { get; set; }
    }
}
