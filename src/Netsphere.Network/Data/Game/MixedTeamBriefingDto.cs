using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    public class MixedTeamBriefingDto
    {
        [Serialize(0)]
        public ulong Unk1 { get; set; }

        [Serialize(1)]
        public byte Unk2 { get; set; }
    }
}
