using BlubLib.Serialization;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class NameTagDto
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public int NameTagId { get; set; }

        public NameTagDto()
        {
        }

        public NameTagDto(ulong accountId, int nameTagId)
        {
            AccountId = accountId;
            NameTagId = nameTagId;
        }
    }
}
