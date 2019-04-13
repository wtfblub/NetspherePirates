using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class PlayerInfoShortDto
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public string Nickname { get; set; }

        [BlubMember(2)]
        public int Unk { get; set; }

        [BlubMember(3)]
        public uint TotalExperience { get; set; }

        [BlubMember(4)]
        public bool IsGM { get; set; }

        public PlayerInfoShortDto()
        {
            Nickname = "";
        }
    }
}
